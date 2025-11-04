using System;
using System.Numerics;

namespace Merge_MonogameProject;

public class RobotSpawnManager
{
    public static int ScreenSizeX = 800;
    public static int ScreenSizeY = 600;
    
    public static void SpawnRobot()
    {
        MergeObject mergeObject1 = SceneManager.Create<MergeObject>();
        mergeObject1.SetSprite("Evolution_0");
        mergeObject1.scale = new Vector2(0.15f, 0.15f);
        int spawnPosX = new Random().Next(100, ScreenSizeX - 100);
        int spawnPosY = new Random().Next(100, ScreenSizeY - 100);
        mergeObject1.position = new Vector2(spawnPosX, spawnPosY);
    }
}