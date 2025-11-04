using System.Diagnostics;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Merge_MonogameProject;

public class Sprite : IDrawable
{
    /// <summary>
    /// Draw Parameters
    /// </summary>
    public Texture2D texture;
    public Vector2 position = Vector2.Zero;
    protected Rectangle? sourceRectangle = null;
    public Color color = Color.White;
    public float rotation = 0.0f;
    public Vector2 origin;
    public Vector2 scale = Vector2.One;
    public SpriteEffects effects = SpriteEffects.None;
    public float depthLayer = 0.0f;
    
    
    protected Spritesheet _spritesheet;
    public Rectangle DestRectangle;
    
    public Sprite()
    {

    }

    public void SetSprite(string spriteName)
    {
        _spritesheet = SpriteManager.GetSprite(spriteName);
        texture = _spritesheet.texture;

        origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
    }
    
    protected Rectangle GetDestRectangle(Rectangle rect)
    {
        int width = (int)(rect.Width * scale.X);
        int height = (int)(rect.Height * scale.Y);

        int pos_x = (int)(position.X - origin.X * scale.X);
        int pos_y  = (int)(position.Y - origin.Y * scale.Y);
        
        return new Rectangle(pos_x, pos_y, width, height);
    }



    public virtual void Update(GameTime gameTime)
    {
        DestRectangle = GetDestRectangle(texture.Bounds);
    }
    
    public virtual void Draw(SpriteBatch _spriteBatch)
    {
        if(texture != null)
            _spriteBatch.Draw(
                texture,
                position,
                sourceRectangle,
                color,
                MathHelper.ToRadians(rotation),
                origin,
                scale,
                effects,
                depthLayer
            );
    }
}