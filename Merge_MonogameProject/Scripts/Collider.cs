using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merge_MonogameProject;


public class Collider : Sprite
{
    public int debugBorderThickness = 1;
    public Color debugBorderColor = Color.White;
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
                debugBorderThickness
            ), 
            debugBorderColor);
        
        // left
        _spriteBatch.Draw(texture,
            new Rectangle(
                DestRectangle.X,
                DestRectangle.Y,
                debugBorderThickness,
                DestRectangle.Height
            ), 
            debugBorderColor);
        
        // right
        _spriteBatch.Draw(texture,
            new Rectangle(
                DestRectangle.X + DestRectangle.Width - debugBorderThickness,
                DestRectangle.Y,
                debugBorderThickness,
                DestRectangle.Height
            ), 
            debugBorderColor);
        
        // bottom
        _spriteBatch.Draw(texture,
            new Rectangle(
                DestRectangle.X,
                DestRectangle.Y + DestRectangle.Height - debugBorderThickness,
                DestRectangle.Width,
                debugBorderThickness
            ), 
            debugBorderColor);
        #endif
        base.Draw(_spriteBatch);
    }
}