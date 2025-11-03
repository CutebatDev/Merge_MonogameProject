using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Merge_MonogameProject.Scripts;

public class GameObject : IUpdatable
{
    public List<Component> Components = new List<Component>();
    

    public T GetComponent<T>() where T : Component
    {
        foreach (var checkComponent in Components)
        {
            if(checkComponent is T returnComp)
                return returnComp;
        }
        return null;
    }

    public void AddComponent<T>(T component) where T : Component
    {
        foreach (var checkComponent in Components)
        {
            if(checkComponent is T returnComp)
                return;
        }
        component.Owner = this;
        Components.Add(component);
    }

    public void Update(GameTime gameTime)
    {
        foreach (var checkComponent in Components)
        {
            if (checkComponent is IUpdatable updateComp)
            {
                updateComp.Update(gameTime);
            }
        }
    }
}   