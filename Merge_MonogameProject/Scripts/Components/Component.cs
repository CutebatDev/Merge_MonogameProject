using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merge_MonogameProject.Scripts;

public abstract class Component
{
    public GameObject Owner { get; set; }
    public bool IsEnabled { get; set; } = true;
    
    public virtual void Initialize() { }
    public virtual void Destroy() { }

}