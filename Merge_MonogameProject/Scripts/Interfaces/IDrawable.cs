using Microsoft.Xna.Framework.Graphics;

namespace Merge_MonogameProject.Scripts;

public interface IDrawable : IUpdateable
{
    void Draw(SpriteBatch spriteBatch);
}