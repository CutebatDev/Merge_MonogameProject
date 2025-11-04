using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Merge_MonogameProject;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    
    public static float ScreenCenterWidth;
    public static float ScreenCenterHeight;
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        _graphics.IsFullScreen = false;
        _graphics.PreferredBackBufferWidth = 800;
        _graphics.PreferredBackBufferHeight = 600;
    }

    protected override void Initialize()
    {
        
        ScreenCenterWidth = GraphicsDevice.Viewport.Width * 0.5f;
        ScreenCenterHeight = GraphicsDevice.Viewport.Height * 0.5f;
        
        base.Initialize();
    }
    
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        var spriteManager = new SpriteManager(Content);

        SpriteManager.AddSprite("Evolution_0", "Evolution_0");
        
        // MergeObject mergeObject1 = SceneManager.Create<MergeObject>();
        // mergeObject1.SetSprite("Evolution_0");
        // mergeObject1.scale = new Vector2(0.15f, 0.15f);
        // mergeObject1.position = new Vector2(100, 100);
        //
        // MergeObject mergeObject2 = SceneManager.Create<MergeObject>();
        // mergeObject2.SetSprite("Evolution_0");
        // mergeObject2.scale = new Vector2(0.15f, 0.15f);
        // mergeObject2.position = new Vector2(300, 100);
        
        UiButton testButton = SceneManager.Create<UiButton>();
        testButton.SetSprite("Evolution_0");
        testButton.position = new Vector2(100, 100);
        testButton.scale = new Vector2(0.15f, 0.05f);
        

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