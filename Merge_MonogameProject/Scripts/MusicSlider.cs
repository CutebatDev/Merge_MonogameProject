using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Merge_MonogameProject;

public class MusicSlider : IUpdatable, IDrawable

{
    public float volume = 1.0f;
    private Rectangle sliderBar = new Rectangle(100, 100, 200, 10);
    private UiButton sliderHandle;
    private Texture2D pixel;

    public MusicSlider()
    {
        pixel = SpriteManager.Get("pixel");
        
        
        sliderHandle = new UiButton();
        sliderHandle.SetSprite("pixel");
        sliderHandle.scale = new Vector2(10, 50);
        // set handle starting pos
    }

    public void SetPosition(Vector2 newPos)
    {
        sliderBar.X = (int)newPos.X;
        sliderBar.Y = (int)newPos.Y;
        sliderHandle.position = new Vector2(
            sliderBar.X + (int)(volume * sliderBar.Width) - 5, // handle centered
            sliderBar.Y - 5
        );

    }
    
    public void Update(GameTime gameTime)
    {
        MouseState mouse = Mouse.GetState();

        // Click and drag slider
        if (mouse.LeftButton == ButtonState.Pressed &&
            sliderBar.Contains(mouse.Position))
        {
            float relativeX = Math.Clamp(mouse.X - sliderBar.X, 0, sliderBar.Width);
            volume = relativeX / sliderBar.Width;
            
            SoundManager.Instance.ChangeVolume(volume);
        }

        sliderHandle.position = new Vector2(
            sliderBar.X + (int)(volume * sliderBar.Width) - 5, // handle centered
            sliderBar.Y - 5
        );

        sliderHandle.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(pixel, sliderBar, Color.Gray);
        
        sliderHandle.Draw(spriteBatch);
    }
}