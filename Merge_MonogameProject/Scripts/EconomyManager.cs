using Microsoft.Xna.Framework;
using System.Collections.Generic;

public class EconomyManager
{
    public static EconomyManager Instance { get; } = new EconomyManager();
    //list of robots per level
    private readonly Dictionary<int, int> _counts = new();
    // per-level time accumulators (in seconds)
    private readonly Dictionary<int, float> _accum = new();

    private EconomyManager() { }

    //register when a robot spawn
    public void Register(int level)
    {
        _counts.TryGetValue(level, out var c);
        _counts[level] = c + 1;
        if (!_accum.ContainsKey(level)) _accum[level] = 0f;
    }

    // for when a robot is removed
    public void Unregister(int level)
    {
        if (_counts.TryGetValue(level, out var c))
        {
            c = System.Math.Max(0, c - 1);
            if (c == 0) { _counts.Remove(level); _accum.Remove(level); }
            else _counts[level] = c;
        }
    }

    // auto tick rewarding
    // I didn´t understand all its logic 🫨
    public void Update(GameTime time)
    {
        // dt = the time since last frame in seconds
        float dt = (float)time.ElapsedGameTime.TotalSeconds;
        foreach (var kv in _counts)
        {
            // which robot level is processing
            int level = kv.Key;
            // how may robot of this level exist
            int count = kv.Value;
            if (count <= 0) continue;

            var spec = RobotEvolutions.Get(level);
            if (spec == null) continue;

            _accum[level] += dt;

            while (_accum[level] >= spec.periodSeconds)
            {
                _accum[level] -= spec.periodSeconds;
                MoneyBank.Add((long)spec.autoReward * count);
            }
        }
    }
    
    //manual click reward
    public void AwardClick(int level)
    {
        var spec = RobotEvolutions.Get(level);
        if (spec == null) return;
        MoneyBank.Add(spec.clickReward);
    }
}