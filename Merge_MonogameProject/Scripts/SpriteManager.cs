using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Merge_MonogameProject.Scripts;

public class SpriteManager
{
    static Dictionary<string, Spritesheet> _spritesheets = new Dictionary<string, Spritesheet>();
    
    static ContentManager _content;

    public SpriteManager(ContentManager content)
    {
        _content = content;
    }

    public static void AddSprite(string spriteName, string filePath, int columns = 1, int rows = 1)
    {
        _spritesheets[spriteName] = new Spritesheet();
        
        _spritesheets[spriteName].texture = _content.Load<Texture2D>(filePath);
        _spritesheets[spriteName].rows = rows;
        _spritesheets[spriteName].columns = columns;
    }

    public static Spritesheet GetSprite(string spriteName)
    {
        // add validation
        return _spritesheets[spriteName];
    }
}