using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Merge_MonogameProject.Scripts;

public class Sprite : IDrawable
{
    public Vector2 position = Vector2.Zero;
    public float rotation = 0.0f;
    public Vector2 scale = Vector2.One;
    public float depthLayer = 0.0f;
    
    protected Texture2D _texture;

    public SpriteEffects effects = SpriteEffects.None;
    
    protected Vector2 _origin;
    
    protected Spritesheet _spritesheet;

    protected Rectangle? sourceRectangle = null;

    public Rectangle DestRectangle;
    
    public Sprite(string spriteName)
    {
        _spritesheet = SpriteManager.GetSprite(spriteName);
        _texture = _spritesheet.texture;

        _origin = new Vector2(_texture.Width * 0.5f, _texture.Height * 0.5f);
    }
    
    protected Rectangle GetDestRectangle(Rectangle rect)
    {
        int width = (int)(rect.Width * scale.X);
        int height = (int)(rect.Height * scale.Y);

        int pos_x = (int)(position.X - _origin.X * scale.X);
        int pos_y  = (int)(position.Y - _origin.Y * scale.Y);

        return new Rectangle(pos_x, pos_y, width, height);
    }



    public virtual void Update(GameTime gameTime)
    {
        DestRectangle = GetDestRectangle(_texture.Bounds);
    }
    
    public virtual void Draw(SpriteBatch _spriteBatch)
    {
        
        _spriteBatch.Draw(
            _texture,
            position,
            sourceRectangle,
            Color.White,
            MathHelper.ToRadians(rotation),
            _origin,
            scale,
            effects,
            depthLayer
        );
    }
}