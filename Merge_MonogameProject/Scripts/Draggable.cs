using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Merge_MonogameProject.Scripts;

public class MergeObject : Collider, IUpdateable
{
    public int size = 0;
    
    static bool isLocked = false;
    static List<MergeObject> mergeObjects = new List<MergeObject>();
    public bool isDragging {get;set;} = false;
    public MergeObject(string spriteName) : base(spriteName)
    {
        mergeObjects.Add(this);
    }

    public void Update(GameTime gameTime)
    {
        // move drag logic in to another function
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
        other.Enabled = false;
        scale += Vector2.One * 0.1f;
    }
}