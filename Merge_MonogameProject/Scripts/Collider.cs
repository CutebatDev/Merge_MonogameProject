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
        base.Draw(_spriteBatch);
    }
}