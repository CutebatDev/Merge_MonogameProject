using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
// [ADDED] Needed for SpriteEffects flip flag
using Microsoft.Xna.Framework.Graphics;

namespace Merge_MonogameProject;

// NOTE FOR FUTURE ME:
// This MergeObject is a draggable merge-piece
// Implements IUpdateable so MonoGame/Game loop will call Update each frame.
// Goal right now: minimal interaction loop -> drag with mouse, drop, if overlapping same-level sibling -> merge (consume other, level++)
public class MergeObject : Collider, IUpdateable
{
    // current evolution tier for this piece
    public int level = 0;
    private static MouseState _prevMouse;

    private Vector2 _baseScale;     // scale to return to after the pop
private float   _popTime = 0f;  // seconds remaining in the pop
private float   _popDur  = 0.12f; // total pop duration
    private float _popAmp = 0.08f; // pop strength (e.g., +8% at peak)
// Call this to trigger a pop that goes up then back to base
private void TriggerPop(float amp = 0.08f, float dur = 0.12f)
{
    // If a pop is NOT already running, capture the true baseline.
    if (_popTime <= 0f)
        _baseScale = scale;

    _popAmp  = amp;
    _popDur  = dur;
    _popTime = dur; // (re)start timer without changing baseline mid-pop
}



    static bool isLocked = false; // if true => someone is dragging. prevents two objects from being picked up at once
    static List<MergeObject> mergeObjects = new List<MergeObject>(); // registry of all live pieces for collision checks

    public bool isDragging { get; set; } = false; // whether THIS robot is currently in the user's mouse grip

    public RobotTag robotTag { get; set; }
    
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
        base.Update(gameTime); // keep as-is (drag hit-tests use DestRectangle)

        if (!Enabled) return;

        // ---------------------------
        // DRAG INPUT (unchanged flow)
        // ---------------------------
        if (!isLocked && !isDragging && Mouse.GetState().LeftButton == ButtonState.Pressed &&
            DestRectangle.Contains(Mouse.GetState().Position))
        {
            // START DRAG:
            // I enter dragging mode and also set the global lock so no other piece can be grabbed mid-drag.
            isDragging = true;
            isLocked = true;
            // [ADDED] If we start dragging mid-hop, pause/clean the pulse immediately
            _pulseTweenActive = false;
            _pulseTweenElapsed = 0f;
            scale = _baseScale;

            // UX mental model: as soon as player clicks the piece, it "snaps" to the hand.
        }
        // release anywhere on mouse-up
        if (isDragging && Mouse.GetState().LeftButton == ButtonState.Released)
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
/*
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
    */

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
    
    // ----- CLICK REWARD (Right Mouse Button) -----
MouseState cur = Mouse.GetState();
bool rightJustPressed = (cur.RightButton == ButtonState.Pressed &&
_prevMouse.RightButton == ButtonState.Released);

if (rightJustPressed && DestRectangle.Contains(cur.Position))
{
    int lvl = (robotTag != null) ? robotTag.Level : level;
    EconomyManager.Instance.AwardClick(lvl);

    // trigger a tiny pop (up then down)
    TriggerPop(0.08f, 0.12f);
}

// ----- POP ANIMATION (ease up-and-back with sine) -----
float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
if (_popTime > 0f)
{
    _popTime -= dt;
    float u = 1f - (_popTime / _popDur);       // 0 -> 1 over the pop
    float eased = MathF.Sin(u * MathF.PI);     
    float s = 1f + _popAmp * eased;            // scale factor
    scale = _baseScale * s;

    if (_popTime <= 0f)
        scale = _baseScale;                    // restore exact original
}

    _prevMouse = cur; // store for next frame
    }

    public List<MergeObject> CheckCollisions()
    {
        List<MergeObject> collisions = new List<MergeObject>();

        foreach (var other in mergeObjects)
        {
            if (other == this || !other.Enabled) continue;

            //  levels equal -> only equals combine to upgrade (classic merge rule).
            if (DestRectangle.Intersects(other.DestRectangle) &&
                !other.isDragging && !isDragging &&
                level == other.level)
            {
                collisions.Add(other);
            }
        }
        if (collisions.Count > 0)
            return collisions;
        return null;
    }

    // Perform the actual merge with a specific other piece.
    // Current behavior:
    //  - robot absorb the other: mrobot level++ (It becomes the upgraded form)
    //  - other is removed from SceneManager 
    //  - visual feedback: I scale up a tiny bit (0.1f) so the player feels the "upgrade pop"
    //  - SceneManager.Remove(other) MUST also ensure 'other' is not left inside mergeObjects
    public void MergeTo(MergeObject other)
    {
        // 1) Guards: both must exist, not dragging, same level, and have tags
        if (other == null || robotTag == null || other.robotTag == null) return;
        if (isDragging || other.isDragging) return;
        if (level != other.level) return;

        int current = level;                      // same as robotTag.Level right now
        int next = RobotEvolutions.NextLevel(current);

        // 2) If there is no next evolution defined, do nothing (or just pop FX)
        var nextEvo = RobotEvolutions.Get(next);
        if (nextEvo == null) return;

        // 3) ECONOMY: remove both parents from counts
        robotTag.Dispose();
        other.robotTag.Dispose();

        // 4) VISUAL/EFFECT: upgrade THIS piece into the child
        level = next;                                      // keep local in sync
        robotTag = new RobotTag(next);                     // economy counts the child
        SetSprite(nextEvo.spriteName);                     // swap art to next evo
        scale += Vector2.One * 0.1f;                       // your “pop” feedback

        // Place at midpoint (optional; you already position via drag)
        position = new Vector2(
            (this.position.X + other.position.X) * 0.5f,
            (this.position.Y + other.position.Y) * 0.5f
        );

        // 5) Remove the other piece from scene and our registry
        SceneManager.Remove(other);
        mergeObjects.Remove(other);                        // ensure it leaves the local list too
        scale += Vector2.One * 0.1f;

        // [ADDED] Keep base scale in sync so puff multiplies correctly after growth
        _baseScale = scale;
    }
}
