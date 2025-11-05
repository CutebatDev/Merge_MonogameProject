using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
    // CONSTRUCTOR
    public MergeObject()
    {
        mergeObjects.Add(this);
    }

    // Core heartbeat: called once per frame by the Game loop
public override void Update(GameTime gameTime)
{
    base.Update(gameTime);
    if (!Enabled) return;

if (!isLocked                    // nobody else is dragging (global mutex)
            && !isDragging
            && Mouse.GetState().LeftButton == ButtonState.Pressed  // the mouse is down now
            && DestRectangle.Contains(Mouse.GetState().Position))  // cursor is inside me (hit test via my AABB)
        {
            // START DRAG:
            // I enter dragging mode and also set the global lock so no other piece can be grabbed mid-drag.
            isDragging = true;
            isLocked = true;

            // UX mental model: as soon as player clicks the piece, it "snaps" to the hand.
        }
        // release anywhere on mouse-up
        if (isDragging && Mouse.GetState().LeftButton == ButtonState.Released)
        {
            isDragging = false;   // stop following the cursor
            isLocked = false;   // free the global mutex so other pieces can be grabbed
                                // (optional) snap/settle logic here
        }

        else if (isDragging)
        {
            position = Mouse.GetState().Position.ToVector2();
        }    // ... (unchanged)

    // ----- merge scan (your code) -----
    List<MergeObject> collisions = CheckCollisions();
    if (collisions != null)
        MergeTo(collisions[0]);

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
    }




}
