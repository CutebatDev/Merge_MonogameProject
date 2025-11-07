using System;
 // to use File.ReadAllText
using System.IO;                
 // to use OrderBy / ToDictionary
using System.Linq;              
 // forJsonSerializer
using System.Text.Json;
using System.Collections.Generic;
using MonoGame.Framework.Devices.Sensors;

// to connect Robots.json schema
public class RobotEvolution
{
    public int level { get; set; }
    public string spriteName { get; set; }

    public int clickReward { get; set; }
    public int autoReward { get; set; }
    public float periodSeconds { get; set; }
}

// loads all evolutions and give access by level
public static class RobotEvolutions
{
    //all evolutions sorted by level
    public static List<RobotEvolution> AllEvolutions { get; private set; } = new();
    //maping level -> evolution
    public static Dictionary<int, RobotEvolution> ByLevel { get; private set; } = new();
    //load from file
    public static void LoadFromFile(string path)
    {
        // reading json text from disk
        string json = File.ReadAllText(path);
        // transforming json text into objects
        var list = JsonSerializer.Deserialize<List<RobotEvolution>>(json);
        // sort by level
        AllEvolutions = list.OrderBy(e => e.level).ToList();
        ByLevel = AllEvolutions.ToDictionary(e => e.level, e => e);
    }

    //get data for a level and return null if not found
    public static RobotEvolution Get(int level) =>
        ByLevel.TryGetValue(level, out var evo) ? evo : null;

    //merging two levels
    public static int NextLevel(int level) => level + 1;
}