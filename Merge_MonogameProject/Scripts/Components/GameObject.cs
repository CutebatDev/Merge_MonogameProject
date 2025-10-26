using System.Collections.Generic;

namespace Merge_MonogameProject.Scripts;

public class GameObject
{
    public List<Component> Components = new List<Component>();
    

    public T GetComponent<T>() where T : Component
    {
        // TODO THIS
        foreach (var checkComponent in Components)
        {
            if(checkComponent is T returnComp)
                return returnComp;
        }
        return null;
    }

    public void AddComponent<T>(T component) where T : Component
    {
        // TODO THIS
        
        foreach (var checkComponent in Components)
        {
            if(checkComponent is T returnComp)
                return;
        }
        component.Owner = this;
        Components.Add(component);
    }
}   