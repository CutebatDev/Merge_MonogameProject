using Microsoft.Xna.Framework;

namespace Merge_MonogameProject.Scripts;

public class TransformComponent : Component
{
    public Vector2 Position { get; set; }
    public float Rotation { get; set; }
    public Vector2 Scale { get; set; } = Vector2.One;

}

