// RobotSpawnManager.cs
using System;
using Microsoft.Xna.Framework; // <- correct Vector2 for MonoGame

namespace Merge_MonogameProject
{
    public static class RobotSpawnManager
    {
        public static int ScreenSizeX = 800;
        public static int ScreenSizeY = 600;

        // Keep one RNG so consecutive spawns don't cluster.
        private static readonly Random rng = new Random();

        // Spawns ONE robot at the given level (default = 0).
        public static void SpawnRobot(int level = 0)
        {
            if (MoneyBank.Spend(10))
            {
                // 1) Lookup data from Robots.json
                var evo = RobotEvolutions.Get(level);
                if (evo == null) return; // guard: level not defined

                // 2) Create the piece and set the correct sprite
                var piece = SceneManager.Create<MergeObject>();
                piece.SetSprite(evo.spriteName);

                // 3) Register with the economy so it starts earning
                piece.robotTag = new RobotTag(level);

                // 4) Simple size + random position
                piece.scale = new Vector2(0.15f, 0.15f);
                int x = rng.Next(100, ScreenSizeX - 100);
                int y = rng.Next(100, ScreenSizeY - 100);
                piece.position = new Vector2(x, y);
            }
            
        }
    }
}
