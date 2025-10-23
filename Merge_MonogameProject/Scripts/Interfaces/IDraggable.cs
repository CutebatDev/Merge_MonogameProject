using Microsoft.Xna.Framework;

namespace Merge_MonogameProject.Scripts;

public interface IDraggable
{
    public bool IsMouseHovering {get;set;}
    public bool IsDragging {get;set;}
    
    public void OnDragStart(Vector2 mousePosition);
    public void OnDrag(Vector2 delta);
    public void OnDragEnd(Vector2 mousePosition);
}