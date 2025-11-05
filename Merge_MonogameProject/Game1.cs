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
        // Your existing center computations
        ScreenCenterWidth  = GraphicsDevice.Viewport.Width  * 0.5f;
        ScreenCenterHeight = GraphicsDevice.Viewport.Height * 0.5f;

        // [ADDED] Set initial PulseMove bounds to match the current viewport
        MergeObject.PulseMoveBounds = new Rectangle(
            0, 0,
            GraphicsDevice.Viewport.Width,
            GraphicsDevice.Viewport.Height
        );

        // [ADDED] Keep bounds (and your screen-center helpers) in sync on window resize
        Window.ClientSizeChanged += (_, __) =>
        {
            MergeObject.PulseMoveBounds = new Rectangle(
                0, 0,
                GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height
            );

            ScreenCenterWidth  = GraphicsDevice.Viewport.Width  * 0.5f; // keep your helpers updated
            ScreenCenterHeight = GraphicsDevice.Viewport.Height * 0.5f;
        };

        base.Initialize();
    }
    
    
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        var spriteManager = new SpriteManager(Content);

        SpriteManager.AddSprite("Evolution_0", "Evolution_0");
        SpriteManager.AddSprite("Evolution_1", "Evolution_1");
        SpriteManager.AddSprite("Evolution_2", "Evolution_2");
        SpriteManager.AddSprite("Evolution_4", "Evolution_4");
        SpriteManager.AddSprite("CreateIcon", "CreateIcon");
        SpriteFont font = Content.Load<SpriteFont>("Fonts/OpenSans");
        
        
        CreateRobotButton spawnButton = SceneManager.Create<CreateRobotButton>();
        spawnButton.SetSprite("CreateIcon");
        spawnButton.scale = new Vector2(0.1f, 0.1f);
        spawnButton.position = new Vector2(50, 50);
        
        StringSprite title = SceneManager.Create<StringSprite>();
        title.spriteFont = font;
        title.text = "Merge";
        title.origin = title.spriteFont.MeasureString(title.text) / 2;
        title.position = new Vector2(ScreenCenterWidth, 100);

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
