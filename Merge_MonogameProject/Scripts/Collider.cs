using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merge_MonogameProject;


public class Collider : Sprite
{
    public int thickness = 0;
    public Color color = Color.White;
    public bool isTrigger = false;
    public bool Enabled = true;
    
    public delegate void EventDelegate(Object obj);
    public event EventDelegate OnCollision;
    public event EventDelegate OnTrigger;
    public Collider()
    {
        
    }

    public bool Intersects(Collider other)
    {
        return DestRectangle.Intersects(other.DestRectangle);
    }

    public void NotifyCollision(object obj)
    {
        if(isTrigger)
            OnTrigger?.Invoke(obj);
        else
            OnCollision?.Invoke(obj);
    }
    
  
    public override void Draw(SpriteBatch _spriteBatch)
    {
         #if DEBUG
        // Draw outline
        // top
        _spriteBatch.Draw(texture,
            new Rectangle(
                DestRectangle.X,
                DestRectangle.Y,
                DestRectangle.Width,
                thickness
            ), 
            color);
        
        // left
        _spriteBatch.Draw(texture,
            new Rectangle(
                DestRectangle.X,
                DestRectangle.Y,
                thickness,
                DestRectangle.Height
            ), 
            color);
        
        // right
        _spriteBatch.Draw(texture,
            new Rectangle(
                DestRectangle.X + DestRectangle.Width - thickness,
                DestRectangle.Y,
                thickness,
                DestRectangle.Height
            ), 
            color);
        
        // bottom
        _spriteBatch.Draw(texture,
            new Rectangle(
                DestRectangle.X,
                DestRectangle.Y + DestRectangle.Height - thickness,
                DestRectangle.Width,
                thickness
            ), 
            color);
        #endif
        base.Draw(_spriteBatch);
    }
}