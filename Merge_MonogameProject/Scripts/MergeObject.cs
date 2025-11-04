using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Merge_MonogameProject;

public class MergeObject : Collider, IUpdateable
{
    public int level = 0;
    
    static bool isLocked = false;
    static List<MergeObject> mergeObjects = new List<MergeObject>();
    public bool isDragging {get;set;} = false;
    
    // CONSTRUCTOR
    public MergeObject()
    {
        mergeObjects.Add(this);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        if (!Enabled) return;
        if (!isLocked && !isDragging && Mouse.GetState().LeftButton == ButtonState.Pressed &&
            DestRectangle.Contains(Mouse.GetState().Position))
        {
            isDragging = true;
            isLocked = true;
        }
        else if (isDragging && Mouse.GetState().LeftButton == ButtonState.Released)
        {
            isDragging = false;
            isLocked = false;
        }
        else if (isDragging)
        {
            position = Mouse.GetState().Position.ToVector2();
        }
        List<MergeObject> collisions = CheckCollisions();
        if(collisions != null)
            MergeTo(collisions[0]);
        
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
    }
}