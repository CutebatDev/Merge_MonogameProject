using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Merge_MonogameProject.Scripts;

public interface ICollider
{
    public List<ICollider> Colliders {get;}
    public Rectangle Rect();
    
    public void OnCollisionEnter(ICollider other);
    public void OnCollision(ICollider other);
    public void OnCollisionExit(ICollider other);
}