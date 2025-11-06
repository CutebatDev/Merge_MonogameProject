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
    public int size = 0;
    
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
    private bool _baseScaleInitialized = false;
    private const float PuffPeak = 1.2f;     // peak scale factor at t=0.5

    // CONSTRUCTOR
    public MergeObject()
    {
        mergeObjects.Add(this);

        // Randomize initial phase so not all hop together
        _timeUntilNextPulse = (float)(_rng.NextDouble() * PulseMoveInterval);

    }

public override void Update(GameTime gameTime)
{
    // Lazy-init the remembered scale only after scene has assigned a real one.
    if (!_baseScaleInitialized && scale != Vector2.One)
    {
        _baseScale = scale;
        _baseScaleInitialized = true;
    }

    if (!Enabled)
    {
        base.Update(gameTime);   // single call on this early return path
        return;
    }

    // Helper local to avoid touching an uninitialized base
    Vector2 baseS = _baseScaleInitialized ? _baseScale : scale;

    // ---------------------------
    // DRAG INPUT
    // ---------------------------
    var mouse = Mouse.GetState();
    if (!isLocked && !isDragging && mouse.LeftButton == ButtonState.Pressed &&
        DestRectangle.Contains(mouse.Position))
    {
        isDragging = true;
        isLocked = true;
        _pulseTweenActive = false;
        _pulseTweenElapsed = 0f;
        scale = baseS;                   // safe use
    }
    else if (isDragging && mouse.LeftButton == ButtonState.Released)
    {
        isDragging = false;
        isLocked = false;
        _pulseTweenActive = false;
        _pulseTweenElapsed = 0f;
        scale = baseS;                   // safe use
        _timeUntilNextPulse = PulseMoveInterval;
    }
    else if (isDragging)
    {
        position = ClampToRect(mouse.Position.ToVector2(), PulseMoveBounds);
        // while dragging, do not advance pulsemove timers
    }

    // ---------------------------
    // PULSE MOVE
    // ---------------------------
    if (PulseMoveEnabled && !isDragging)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_pulseTweenActive)
        {
            _pulseTweenElapsed += dt;
            float t = _pulseTweenElapsed / PulseTweenDuration;
            if (t > 1f) t = 1f;

            position = Vector2.Lerp(_startPos, _targetPos, t);

            // triangle puff 1.0 -> PuffPeak -> 1.0
            float k = (t < 0.5f) ? (t / 0.5f) : ((1f - t) / 0.5f);
            float puff = 1.0f + (PuffPeak - 1.0f) * k;
            scale = baseS * puff;        // multiply around remembered size

            if (_pulseTweenElapsed >= PulseTweenDuration)
            {
                position = _targetPos;
                scale = baseS;           // restore exact base
                _pulseTweenActive = false;
                _pulseTweenElapsed = 0f;
                _timeUntilNextPulse = PulseMoveInterval;
            }
        }
        else
        {
            _timeUntilNextPulse -= dt;
            if (_timeUntilNextPulse <= 0f)
            {
                float angle = (float)(_rng.NextDouble() * MathHelper.TwoPi);
                float r = (float)System.Math.Sqrt(_rng.NextDouble()) * PulseMoveRadius;
                Vector2 offset = new((float)System.Math.Cos(angle), (float)System.Math.Sin(angle));
                offset *= r;

                Vector2 candidate = ClampToRect(position + offset, PulseMoveBounds);

                float dx = candidate.X - position.X;
                effects = (dx > 0f) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                _startPos = position;
                _targetPos = candidate;
                _pulseTweenElapsed = 0f;
                _pulseTweenActive = true;
            }
        }
    }

    // ---------------------------
    // MERGE CHECK
    // ---------------------------
    var collisions = CheckCollisions();
    if (collisions != null)
        MergeTo(collisions[0]);

    // Single call so rectangles reflect final position/scale this frame
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
            if(DestRectangle.Intersects(other.DestRectangle) && !other.isDragging && !isDragging && size == other.size)
                collisions.Add(other);
        }

        if (collisions.Count > 0)
            return collisions;
        return null;
    }
    
    public void MergeTo(MergeObject other)
    {
        size++;
        SceneManager.Remove(other);
        scale += Vector2.One * 0.1f;

        // [ADDED] Keep base scale in sync so puff multiplies correctly after growth
        _baseScale = scale;
    }
}
