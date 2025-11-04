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

    static bool isLocked = false; // if true => someone is dragging. prevents two objects from being picked up at once
    static List<MergeObject> mergeObjects = new List<MergeObject>(); // registry of all live pieces for collision checks

    public bool isDragging { get; set; } = false; // whether THIS robot is currently in the user's mouse grip

    // CONSTRUCTOR
    public MergeObject()
    {
        mergeObjects.Add(this);
    }

    // Core heartbeat: called once per frame by the Game loop
    public override void Update(GameTime gameTime)
    {
        // Let the parent Collider do its own refresh first 
        base.Update(gameTime);

        if (!Enabled) return;

        // --- INPUT & DRAG LOGIC --------------------------------------------------------------
        // IMPORTANT: I'm using "stateful hold" logic (Pressed vs Released) without edge detection across frames
        // release anywhere ends drag. Later I can switch to "edge-based" input (pressed this frame vs previous) if I need debouncing.

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
        else if (isDragging)
        {
            position = Mouse.GetState().Position.ToVector2();
        }

        // After any potential movement, scan for neighbors to merge with.
        //  merge only with one (the first found), only equal-level, rectangles must overlap, and neither is being dragged
        List<MergeObject> collisions = CheckCollisions();
        if (collisions != null)
            MergeTo(collisions[0]); // NOTE: order in the list is arbitrary. If multiple overlaps, we just take the first.
                                    // If I need "closest" feel, compute min distance to centers instead of index 0
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
        level++;                                // promote me to next tier (gameplay effect)

        SceneManager.Remove(other);             // consume the other. Assumption: fully removes from update/registry/render.
                                                // If I later observe null refs during iteration or false positives, revisit the registry cleanup.

        scale += Vector2.One * 0.1f;            // tiny grow to "telegraph" success (no tween yet; pure step).
                                                // Future: replace with a short tween + particle burst + SFX "clink".
                                                // Also swap my sprite/art here based on 'level' when I wire that data (SpriteSheet or Atlas).
    }
}
