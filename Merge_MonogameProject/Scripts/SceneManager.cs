using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merge_MonogameProject.Scripts;

public class SceneManager
{
    
    private static List<IUpdateable>_updatables = new List<IUpdateable>();
    private static List<IDrawable> _drawables = new List<IDrawable>();
    // TODO CHANGE TO GameObjects
    
    
    private static SceneManager _instance = null;

    public static SceneManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new SceneManager();
            return _instance;
        }
    }
    
    
    public static T Create<T>() where T : IUpdateable, new()
    {
        T obj = new T();
        Add(obj);
        return obj;
    }

    public static void Add<T>(T obj) where T : IUpdateable
    {
        _updatables.Add(obj);
        if(obj is IDrawable drawable)
            _drawables.Add(drawable);
    }

    public static void Remove<T>(T obj) where T : IUpdateable
    {
        if(_updatables.Contains(obj))
            _updatables.Remove(obj);
        if(obj is IDrawable drawable && _drawables.Contains(drawable))
            _drawables.Remove(drawable);
    }

    public void Update(GameTime deltatime)
    {
        foreach (var updateVar in _updatables)
            updateVar.Update(deltatime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var drawVar in _drawables)
            drawVar.Draw(spriteBatch);
    }
}