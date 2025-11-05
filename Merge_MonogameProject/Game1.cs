using System;                          // [FIX] for Math.Max/Min casts
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Merge_MonogameProject;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;

    private GearCounterUI _gearUI;

    private int _gears = 0;
    private SpriteBatch _spriteBatch;

    private Sprite _backgroundSprite;   // [FIX] keep a handle to rescale on resize
private Texture2D _bgTex;             // raw texture for size/scale math (loaded via Content)


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

        // [FIX] refit background when window resizes
        Window.ClientSizeChanged += (s, e) =>
        {
            if (_backgroundSprite != null)
                FitBackgroundToViewport(useAspectFill: true, _backgroundSprite);
        };
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

        // --- register all content up front (idempotent) ---
        SpriteManager.AddSprite("Scenario",    "Scenario");
        SpriteManager.AddSprite("Evolution_0", "Evolution_0");
        SpriteManager.AddSprite("Evolution_1", "Evolution_1");
        SpriteManager.AddSprite("Evolution_2", "Evolution_2");
        SpriteManager.AddSprite("Evolution_4", "Evolution_4");
        SpriteManager.AddSprite("CreateIcon",  "CreateIcon");

        // --- BACKGROUND first so it stays behind everything else ---
        var background = SceneManager.Create<Sprite>();   // plain sprite, not a button
        background.SetSprite("Scenario");

        bool useAspectFill = true; // cover screen; may crop edges

        // [FIX] compute scale using the actual texture from SpriteManager
        var bgTex = SpriteManager.Get("Scenario");        // Texture2D returned by manager
        FitBackgroundToViewport(useAspectFill, background, bgTex);

        // [NOTE] Your Sprite doesn’t expose sort/layer fields.
        // Draw order is guaranteed here because we created the background FIRST.

        _backgroundSprite = background;

        // --- your original registrations (harmless duplicates) ---
        SpriteManager.AddSprite("Evolution_0", "Evolution_0");
        SpriteManager.AddSprite("Evolution_1", "Evolution_1");
        SpriteManager.AddSprite("Evolution_2", "Evolution_2");
        SpriteManager.AddSprite("Evolution_4", "Evolution_4");
        SpriteManager.AddSprite("Scenario", "Scenario");

        SpriteManager.AddSprite("CreateIcon", "CreateIcon");

        CreateRobotButton spawnButton = SceneManager.Create<CreateRobotButton>();
        spawnButton.SetSprite("CreateIcon");
        spawnButton.scale = new Vector2(0.1f, 0.1f);
        spawnButton.position = new Vector2(50, 50);


_gearUI = new GearCounterUI(new Vector2(20, 20)); // top-left corner
_gearUI.Load(Content);
_gearUI.SetTotal(_gears);

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

        // SceneManager should draw background first since we created it first
        SceneManager.Instance.Draw(_spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);

        _spriteBatch.Begin();
_gearUI.Draw(_spriteBatch);
_spriteBatch.End();

    }

    // ================= helpers =================

    // [FIX] overload that fetches the texture once
private void FitBackgroundToViewport(bool useAspectFill, Sprite background, Texture2D tex)
{
    var vp = GraphicsDevice.Viewport;
    int screenW = vp.Width, screenH = vp.Height;

    if (tex == null) return;

    int texW = tex.Width, texH = tex.Height;

    float sx = (float)screenW / texW;
    float sy = (float)screenH / texH;

    float s = useAspectFill ? (float)Math.Max(sx, sy) : (float)Math.Min(sx, sy);

    background.scale    = new Vector2(s, s);
    background.position = new Vector2(screenW * 0.5f, screenH * 0.5f); // assumes center-origin draw
}


    // keep the previous signature for resize path (we can re-fetch the texture)
private void FitBackgroundToViewport(bool useAspectFill, Sprite background)
{
    var tex = SpriteManager.GetSprite("Scenario").texture;
    FitBackgroundToViewport(useAspectFill, background, tex);
}

}
