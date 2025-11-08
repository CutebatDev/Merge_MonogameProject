using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Merge_MonogameProject;

public class EconomyManager
{
    public static EconomyManager Instance { get; } = new EconomyManager();
    
    // Font for floating text - set this in LoadContent or initialization
    public static SpriteFont FloatingTextFont { get; set; }
    
    //list of robots per level
    private readonly Dictionary<int, int> _counts = new();
    // per-level time accumulators (in seconds)
    private readonly Dictionary<int, float> _accum = new();
    
    // Active floating texts for cleanup
    private readonly List<FloatingText> _activeFloatingTexts = new();

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
    public void Update(GameTime time)
    {
        // dt = the time since last frame in seconds
        float dt = (float)time.ElapsedGameTime.TotalSeconds;
        foreach (var kv in _counts)
        {
            int level = kv.Key;
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
        
        // Clean up inactive floating texts
        CleanupInactiveFloatingTexts();
    }
    
    //manual click reward with floating text
    public void AwardClick(int level, Vector2? clickPosition = null)
    {
        var spec = RobotEvolutions.Get(level);
        if (spec == null) return;
        
        MoneyBank.Add(spec.clickReward);
        
        // Create floating text if font is available and position is provided
        if (FloatingTextFont != null && clickPosition.HasValue)
        {
            ShowFloatingReward(spec.clickReward, clickPosition.Value);
        }
    }
    
    private void ShowFloatingReward(long amount, Vector2 position)
    {
        var floatingText = SceneManager.Create<FloatingText>();
        string rewardText = $"+${amount:N0}";
        
        floatingText.Start(
            FloatingTextFont, 
            rewardText, 
            position, 
            Color.Gold, 
            1.5f,  // 1.5 seconds duration
            60f    // Move up 60 pixels
        );
        
        _activeFloatingTexts.Add(floatingText);
    }
    
    private void CleanupInactiveFloatingTexts()
    {
        for (int i = _activeFloatingTexts.Count - 1; i >= 0; i--)
        {
            if (!_activeFloatingTexts[i].IsActive)
            {
                SceneManager.Remove(_activeFloatingTexts[i]);
                _activeFloatingTexts.RemoveAt(i);
            }
        }
    }
}