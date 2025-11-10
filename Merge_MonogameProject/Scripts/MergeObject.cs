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
public class MergeObject : Collider
{
    // current evolution tier for this piece
    public int level = 0;
    private MouseState _prevMouse;

    // Pop animation state
    private Vector2 _baseScale = Vector2.One;
    private float _popTime = 0f;
    private float _popDur = 0.12f;
    private float _popAmp = 0.08f;

    // Static drag lock and registry
    static bool isLocked = false;
    static List<MergeObject> mergeObjects = new List<MergeObject>();

    public bool isDragging { get; set; } = false;
    public RobotTag robotTag { get; set; }

    // PulseMove configuration
    public static Rectangle PulseMoveBounds = new Rectangle(0, 0, 1280, 720);
    public bool PulseMoveEnabled = true;
    public float PulseMoveInterval = 3.0f;
    public float PulseMoveRadius = 30.0f;
    public float PulseTweenDuration = 0.20f;

    // PulseMove state
    private static readonly System.Random _rng = new System.Random();
    private bool _pulseTweenActive = false;
    private float _timeUntilNextPulse = 0f;
    private float _pulseTweenElapsed = 0f;
    private Vector2 _startPos;
    private Vector2 _targetPos;
    private bool _baseScaleInitialized = false;
    private const float PuffPeak = 1.2f;

    public MergeObject()
    {
        mergeObjects.Add(this);
        _timeUntilNextPulse = (float)(_rng.NextDouble() * PulseMoveInterval);
    }

    private void TriggerPop(float amp = 0.08f, float dur = 0.12f)
    {
        _popAmp = amp;
        _popDur = dur;
        _popTime = dur;
    }

    public override void Update(GameTime gameTime)
    {
        if (!Enabled) return;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        MouseState currentMouse = Mouse.GetState();

        // Initialize base scale once
        if (!_baseScaleInitialized && scale != Vector2.One)
        {
            _baseScale = scale;
            _baseScaleInitialized = true;
        }

        // Handle drag input
        HandleDragInput(currentMouse);

        // Handle pulse movement when not dragging
        if (PulseMoveEnabled && !isDragging)
        {
            HandlePulseMovement(dt);
        }

        // Handle pop animation
        HandlePopAnimation(dt);

        // Handle click rewards
        HandleClickRewards(currentMouse);

        // Check for merges
        List<MergeObject> collisions = CheckCollisions();
        if (collisions != null)
            MergeTo(collisions[0]);

        _prevMouse = currentMouse;
        base.Update(gameTime);
    }

    private void HandleDragInput(MouseState currentMouse)
    {
        // Start drag
        if (!isLocked && !isDragging && 
            currentMouse.LeftButton == ButtonState.Pressed &&
            DestRectangle.Contains(currentMouse.Position))
        {
            isDragging = true;
            isLocked = true;
            _pulseTweenActive = false;
            _pulseTweenElapsed = 0f;
            scale = _baseScale;
        }

        // End drag
        if (isDragging && currentMouse.LeftButton == ButtonState.Released)
        {
            isDragging = false;
            isLocked = false;
            _pulseTweenActive = false;
            _pulseTweenElapsed = 0f;
            scale = _baseScale;
            _timeUntilNextPulse = PulseMoveInterval;
        }

        // Update position while dragging
        if (isDragging)
        {
            position = ClampToRect(currentMouse.Position.ToVector2(), PulseMoveBounds);
        }
    }

    private void HandlePulseMovement(float dt)
    {
        if (_pulseTweenActive)
        {
            _pulseTweenElapsed += dt;
            float t = Math.Min(_pulseTweenElapsed / PulseTweenDuration, 1f);

            // Linear position interpolation
            position = Vector2.Lerp(_startPos, _targetPos, t);

            // Triangle puff scaling
            float puff = t < 0.5f 
                ? 1.0f + (PuffPeak - 1.0f) * (t / 0.5f)
                : 1.0f + (PuffPeak - 1.0f) * ((1.0f - t) / 0.5f);

            scale = _baseScale * puff;

            // Finish tween
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
            _timeUntilNextPulse -= dt;
            if (_timeUntilNextPulse <= 0f)
            {
                StartPulseMovement();
            }
        }
    }

    private void StartPulseMovement()
    {
        float angle = (float)(_rng.NextDouble() * MathHelper.TwoPi);
        float r = (float)Math.Sqrt(_rng.NextDouble()) * PulseMoveRadius;
        Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * r;

        Vector2 candidate = ClampToRect(position + offset, PulseMoveBounds);

        // Update sprite facing based on movement direction
        float dx = candidate.X - position.X;
        if (dx > 0f) effects = SpriteEffects.FlipHorizontally;
        else if (dx < 0f) effects = SpriteEffects.None;

        _startPos = position;
        _targetPos = candidate;
        _pulseTweenElapsed = 0f;
        _pulseTweenActive = true;
    }

    private void HandlePopAnimation(float dt)
    {
        if (_popTime > 0f)
        {
            _popTime -= dt;
            float u = 1f - (_popTime / _popDur);
            float eased = MathF.Sin(u * MathF.PI);
            float s = 1f + _popAmp * eased;
            scale = _baseScale * s;

            if (_popTime <= 0f)
                scale = _baseScale;
        }
    }

    private void HandleClickRewards(MouseState currentMouse)
    {
        bool rightJustPressed = (currentMouse.RightButton == ButtonState.Pressed &&
                               _prevMouse.RightButton == ButtonState.Released);

        if (rightJustPressed && DestRectangle.Contains(currentMouse.Position))
        {
            int lvl = (robotTag != null) ? robotTag.Level : level;
            EconomyManager.Instance.AwardClick(lvl, Mouse.GetState().Position.ToVector2());
            TriggerPop(0.08f, 0.12f);
            SoundManager.Instance.PlaySfx();
        }
    }

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

            if (DestRectangle.Intersects(other.DestRectangle) &&
                !other.isDragging && !isDragging &&
                level == other.level)
            {
                collisions.Add(other);
            }
        }

        return collisions.Count > 0 ? collisions : null;
    }

    public void MergeTo(MergeObject other)
    {
        if (other == null || robotTag == null || other.robotTag == null) return;
        if (isDragging || other.isDragging) return;
        if (level != other.level) return;

        int current = level;
        int next = RobotEvolutions.NextLevel(current);

        var nextEvo = RobotEvolutions.Get(next);
        if (nextEvo == null) return;

        // Remove both parents from economy
        robotTag.Dispose();
        other.robotTag.Dispose();

        // Upgrade this piece
        level = next;
        robotTag = new RobotTag(next);
        SetSprite(nextEvo.spriteName);

        // Position at midpoint
        position = new Vector2(
            (this.position.X + other.position.X) * 0.5f,
            (this.position.Y + other.position.Y) * 0.5f
        );

        // Visual feedback
        scale += Vector2.One * 0.1f;
        _baseScale = scale; // Keep base scale in sync

        // Remove the other piece
        SceneManager.Remove(other);
        mergeObjects.Remove(other);
        SoundManager.Instance.PlaySfx();
    }
}