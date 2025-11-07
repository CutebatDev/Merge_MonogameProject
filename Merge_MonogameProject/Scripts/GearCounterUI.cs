using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

public class GearCounterUI{
    //loaded from content "UIfont"
    private SpriteFont _font;

    // position in the screen (in pixels)
    private Vector2 _pos;

    // number of gears to display
    private int _totalGears;

    public GearCounterUI(Vector2 position)
    {
        _pos = position;
    }

    public void Load(ContentManager content)
    {
        _font = content.Load<SpriteFont>("UIfont");
    }

    public void SetTotal(int newTotal)
    {
        // updatong the total amount of geras
        _totalGears = newTotal;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        string text = $"Gears: {_totalGears}";

        // 1) shadow (offset by 1px), 2) main text
        spriteBatch.DrawString(_font, text, _pos + new Vector2(1, 1), Color.Black);
        spriteBatch.DrawString(_font, text, _pos,                   Color.Gold);
    }

}