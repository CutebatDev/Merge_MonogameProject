using System;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Merge_MonogameProject.Scripts;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteManager _spriteManager;
    private static List<IDrawable> _drawables = new List<IDrawable>();
    private static List<IUpdateable> _updateables= new List<IUpdateable>();
    Texture2D evo0;   // simple names for study
	Texture2D evo1;
	Texture2D evo2;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _graphics.IsFullScreen = false;
        _graphics.PreferredBackBufferWidth = 1920;
        _graphics.PreferredBackBufferHeight = 1080;
    }

    protected override void Initialize()
    {
        ScreenCenterWidth = GraphicsDevice.Viewport.Width * 0.5f;
        ScreenCenterHeight = GraphicsDevice.Viewport.Height * 0.5f;
        
        base.Initialize();
    }
    // FONT GOES HERE

    public static float ScreenCenterWidth;
    public static float ScreenCenterHeight;

    
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _spriteManager = new SpriteManager(Content);

        // LOAD SPRITES HERE
        // load by the name in mgcb (without .png)
        evo0 = Content.Load<Texture2D>("Evolution_0");
        evo1 = Content.Load<Texture2D>("Evolution_1");
        evo2 = Content.Load<Texture2D>("Evolution_2");
        
        // TODO: use this.Content to load your game content here
    }
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        SceneManager.Instance.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        
        SceneManager.Instance.Draw(_spriteBatch);
        
        _spriteBatch.End();
        
        base.Draw(gameTime);
    }
}