using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merge_MonogameProject.Scripts;

public class SpriteComponent : Component, IDrawable
{
    public bool Enabled { get; set; }
    public Texture2D Texture { get; set; }
    public Vector2 Position { get; set; }
    public Rectangle? Origin { get; set; }
    public Color Color { get; set; }
    public float Rotation { get; set; }
    public Vector2 Pivot { get; set; }
    public Vector2 Scale { get; set; }
    public SpriteEffects Effects { get; set; }
    public float LayerDepth { get; set; }
    public Color Tint { get; set; } = Color.White;
    
    public void Draw(SpriteBatch spriteBatch)
    {
        var transform = Owner.GetComponent<TransformComponent>();
        spriteBatch.Draw(Texture, transform.Position, Tint);
    }

}