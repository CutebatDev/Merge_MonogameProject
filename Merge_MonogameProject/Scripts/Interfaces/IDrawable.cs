using Microsoft.Xna.Framework.Graphics;

namespace Merge_MonogameProject;

public interface IDrawable : IUpdateable
{
    void Draw(SpriteBatch spriteBatch);
}