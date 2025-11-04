using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Merge_MonogameProject;

public class SpriteManager
{
    // cache: id -> spritesheet (holds Texture2D + grid info)
    static readonly Dictionary<string, Spritesheet> _spritesheets = new();

    // single ContentManager (set once)
    static ContentManager _content;

    public SpriteManager(ContentManager content)
    {
        // keep the first valid content reference; ignore subsequent news
        if (_content == null) _content = content;
    }

    public static void AddSprite(string spriteName, string filePath, int columns = 1, int rows = 1)
    {
        var sheet = new Spritesheet
        {
            texture = _content.Load<Texture2D>(filePath),
            rows = rows,
            columns = columns
        };

        _spritesheets[spriteName] = sheet;
    }

    public static Spritesheet GetSprite(string spriteName)
    {
        // NOTE: assuming it exists (you already AddSprite in LoadContent)
        return _spritesheets[spriteName];
    }

    // >>> This is the wrapper we want everywhere that asks for a Texture2D <<<
    public static Texture2D Get(string spriteName)
    {
        return _spritesheets[spriteName].texture;
    }

    // (removed the old internal Get(...) that threw NotImplementedException)
}
