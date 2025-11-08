using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Merge_MonogameProject;

public class FloatingText : StringSprite
{
    public float duration = 1.0f;
    
    private float elapsedTime = 0f;
    private Vector2 startPosition;
    private Vector2 targetPosition;
    private Color startColor;
    private bool isActive = true;
    private float moveDistance = 50f;
    
    // Add slight random offsets to prevent overlapping
    private static Random random = new Random();

    public FloatingText()
    {
        
    }

    public FloatingText(SpriteFont font, string displayText, Vector2 startPos, Color textColor, float animationDuration = 1.0f, float upwardDistance = 50f)
    {
        Initialize(font, displayText, startPos, textColor, animationDuration, upwardDistance);
    }

    public void Start(SpriteFont font, string displayText, Vector2 startPos, Color textColor, float animationDuration = 1.0f, float upwardDistance = 50f)
    {
        Initialize(font, displayText, startPos, textColor, animationDuration, upwardDistance);
        elapsedTime = 0f;
        isActive = true;
    }

    private void Initialize(SpriteFont font, string displayText, Vector2 startPos, Color textColor, float animationDuration, float upwardDistance)
    {
        spriteFont = font;
        text = displayText;
        
        // Add small random offset to prevent perfect overlap
        Vector2 randomOffset = new Vector2(
            (float)(random.NextDouble() - 0.5) * 30f, // ±15 pixels horizontally
            (float)(random.NextDouble() - 0.5) * 20f  // ±10 pixels vertically
        );
        
        startPosition = startPos + randomOffset;
        position = startPosition;
        targetPosition = new Vector2(startPosition.X, startPosition.Y - upwardDistance);
        color = textColor;
        startColor = textColor;
        duration = animationDuration;
        moveDistance = upwardDistance;
    }

    public bool IsActive => isActive;

    public override void Update(GameTime gameTime)
    {
        if (!isActive) return;

        elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        if (elapsedTime >= duration)
        {
            isActive = false;
            return;
        }

        float progress = elapsedTime / duration;
        float easedProgress = EaseOut(progress);
        
        position = Vector2.Lerp(startPosition, targetPosition, easedProgress);
        
        float alpha = 1.0f - progress;
        color = new Color(startColor.R, startColor.G, startColor.B, (byte)(startColor.A * alpha));
        
        base.Update(gameTime);
    }

    private float EaseOut(float t)
    {
        return 1 - (1 - t) * (1 - t);
    }

    public override void Draw(SpriteBatch _spriteBatch)
    {
        if (isActive)
        {
            base.Draw(_spriteBatch);
        }
    }
}