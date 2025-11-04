using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
// [ADDED] Needed for SpriteEffects flip flag
using Microsoft.Xna.Framework.Graphics;

namespace Merge_MonogameProject;

public class MergeObject : Collider, IUpdateable
{
    public int level = 0;
    
    static bool isLocked = false;
    static List<MergeObject> mergeObjects = new List<MergeObject>();
    public bool isDragging {get;set;} = false;

    // -------------------------
    // [ADDED] PulseMove config
    // -------------------------
    public static Rectangle PulseMoveBounds = new Rectangle(0, 0, 1280, 720); // set from game init to your FOV
    public bool PulseMoveEnabled   = true;   // quick toggle
    public float PulseMoveInterval = 3.0f;   // seconds between hops
    public float PulseMoveRadius   = 30.0f;   // max hop distance
    public float PulseTweenDuration = 0.20f; // hop duration (linear)

    // -------------------------
    // [ADDED] PulseMove state
    // -------------------------
    private static readonly System.Random _rng = new System.Random();
    private bool  _pulseTweenActive = false;
    private float _timeUntilNextPulse = 0f;  // randomized on spawn
    private float _pulseTweenElapsed = 0f;
    private Vector2 _startPos;
    private Vector2 _targetPos;

    // [ADDED] Puff scale support (triangle curve around a base scale)
    private Vector2 _baseScale;
    private const float PuffPeak = 1.2f;     // peak scale factor at t=0.5

    // CONSTRUCTOR
    public MergeObject()
    {
        mergeObjects.Add(this);

        // [ADDED] Randomize initial phase so not all hop together
        _timeUntilNextPulse = (float)(_rng.NextDouble() * PulseMoveInterval);

        // [ADDED] Capture base scale for clean puffing (merge grows will update this too)
        _baseScale = scale;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime); // keep as-is (drag hit-tests use DestRectangle)

        if (!Enabled) return;

        // ---------------------------
        // DRAG INPUT (unchanged flow)
        // ---------------------------
        if (!isLocked && !isDragging && Mouse.GetState().LeftButton == ButtonState.Pressed &&
            DestRectangle.Contains(Mouse.GetState().Position))
        {
            isDragging = true;
            isLocked = true;
            // [ADDED] If we start dragging mid-hop, pause/clean the pulse immediately
            _pulseTweenActive = false;
            _pulseTweenElapsed = 0f;
            scale = _baseScale;
        }
        else if (isDragging && Mouse.GetState().LeftButton == ButtonState.Released)
        {
            isDragging = false;
            isLocked = false;

            // [ADDED] On release, restart the interval cleanly (no instant hop surprise)
            _pulseTweenActive = false;
            _pulseTweenElapsed = 0f;
            scale = _baseScale;
            _timeUntilNextPulse = PulseMoveInterval;
        }
        else if (isDragging)
        {
// NEW — keep the robot’s CENTER inside the view, accounting for origin & scale
position = ClampToRect(Mouse.GetState().Position.ToVector2(), PulseMoveBounds);
            // [ADDED] While dragging, do NOT advance pulsemove timers/tween
        }

        // --------------------------------
        // [ADDED] PulseMove (linear hop)
        // --------------------------------
        if (PulseMoveEnabled && !isDragging)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_pulseTweenActive)
            {
                // advance tween
                _pulseTweenElapsed += dt;
                float t = _pulseTweenElapsed / PulseTweenDuration;
                if (t > 1f) t = 1f;

                // linear position (no easing)
                position = Vector2.Lerp(_startPos, _targetPos, t);

                // triangle puff 1.0 -> 1.2 -> 1.0
                float puff;
                if (t < 0.5f)
                {
                    float k = t / 0.5f;           // 0..1
                    puff = 1.0f + (PuffPeak - 1.0f) * k;
                }
                else
                {
                    float k = (1.0f - t) / 0.5f;  // 1..0
                    puff = 1.0f + (PuffPeak - 1.0f) * k;
                }
                scale = _baseScale * puff;

                // finish
                if (_pulseTweenElapsed >= PulseTweenDuration)
                {
                    position = _targetPos;
                    scale = _baseScale;
                    _pulseTweenActive = false;
                    _pulseTweenElapsed = 0f;
                    _timeUntilNextPulse = PulseMoveInterval;
                }
            }
            else
            {
                // idle waiting
                _timeUntilNextPulse -= dt;
                if (_timeUntilNextPulse <= 0f)
                {
                    // random offset uniformly in disk (sqrt for area-uniform radius)
                    float angle = (float)(_rng.NextDouble() * MathHelper.TwoPi);
                    float r = (float)System.Math.Sqrt(_rng.NextDouble()) * PulseMoveRadius;
                    Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * r;

                    // candidate target
                    Vector2 candidate = position + offset;

                    // clamp to FOV so robots never leave screen
                    candidate = ClampToRect(candidate, PulseMoveBounds);

                    // decide facing from FINAL delta (what the player actually sees)
                    float dx = candidate.X - position.X;
                    if (dx > 0f) effects = SpriteEffects.FlipHorizontally; // art faces left; flip when going right
                    else if (dx < 0f) effects = SpriteEffects.None;        // keep facing left

                    // start tween
                    _startPos = position;
                    _targetPos = candidate;
                    _pulseTweenElapsed = 0f;
                    _pulseTweenActive = true;
                }
            }
        }

        // ---------------------------
        // MERGE CHECK (unchanged)
        // ---------------------------
        List<MergeObject> collisions = CheckCollisions();
        if(collisions != null)
            MergeTo(collisions[0]);

        // [ADDED] After PulseMove may have changed position/scale,
        // update rectangles once more so hit-tests match this frame.
        base.Update(gameTime);
    }

    // [ADDED] Small helper for bounds
    private static Vector2 ClampToRect(Vector2 p, Rectangle rect)
    {
        float x = MathHelper.Clamp(p.X, rect.Left, rect.Right);
        float y = MathHelper.Clamp(p.Y, rect.Top, rect.Bottom);
        return new Vector2(x, y);
    }

    public List<MergeObject> CheckCollisions()
    {
        List<MergeObject> collisions = new List<MergeObject>();
        foreach (var other in mergeObjects)
        {
            if (other == this || !other.Enabled) continue;
            if(DestRectangle.Intersects(other.DestRectangle) && !other.isDragging && !isDragging && level == other.level)
                collisions.Add(other);
        }

        if (collisions.Count > 0)
            return collisions;
        return null;
    }
    
    public void MergeTo(MergeObject other)
    {
        level++;
        SceneManager.Remove(other);
        scale += Vector2.One * 0.1f;

        // [ADDED] Keep base scale in sync so puff multiplies correctly after growth
        _baseScale = scale;
    }
}
