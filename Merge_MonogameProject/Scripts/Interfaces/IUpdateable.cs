using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merge_MonogameProject.Scripts;

public interface IUpdateable
{
    void Update(GameTime gameTime);
}