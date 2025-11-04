using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merge_MonogameProject;

public class Spritesheet
{
    public int columns { get; set; }
    public int rows { get; set; }
    
    public Texture2D texture {
        get;
        set;
    }


    public Rectangle this[int x, int y]
    {
        get
        {
            return new Rectangle(
                (int)(texture.Width * (float)x / columns),
                (int)(texture.Height * (float)y / rows),
                (int)(texture.Width * 1.0f / columns),
                (int)(texture.Height * 1.0f / rows));
        }
    }

}