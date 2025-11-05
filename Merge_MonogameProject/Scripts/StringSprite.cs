using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merge_MonogameProject;

public class StringSprite : IDrawable
{
    /// <summary>
    /// Draw Parameters
    /// </summary>
    public SpriteFont spriteFont;
    public string text;
    public Vector2 position = Vector2.Zero;
    public Color color = Color.White;
    public float rotation = 0.0f;
    public Vector2 origin;
    public Vector2 scale = Vector2.One;
    public SpriteEffects effects = SpriteEffects.None;
    public float depthLayer = 0.0f;
    
    public Rectangle DestRectangle;

    public StringSprite()
    {
        
    }

    public virtual Rectangle GetDestRectangle()
    {
        Vector2 stringSize = spriteFont.MeasureString(text);
        return new Rectangle((int)position.X, (int)position.Y, (int)stringSize.X, (int)stringSize.Y);
    }
    
    public virtual void Update(GameTime gameTime)
    {
        DestRectangle = GetDestRectangle();
    }
    
    public virtual void Draw(SpriteBatch _spriteBatch)
    {
        if(text != "" && spriteFont != null)
            _spriteBatch.DrawString(
                spriteFont,
                text,
                position,
                color,
                MathHelper.ToRadians(rotation),
                origin,
                scale,
                effects,
                depthLayer
            );
    }
    
}