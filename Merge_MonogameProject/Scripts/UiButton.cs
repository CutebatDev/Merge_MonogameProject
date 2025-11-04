using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Merge_MonogameProject;

public class UiButton : Collider
{
    private bool _isClicked = false;
    private bool _isHover = false;
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        if (!Enabled) return;
        
        // tracking clicks
        if(_isHover && !_isClicked && Mouse.GetState().LeftButton == ButtonState.Pressed)
            OnClick();
        else if(_isHover && _isClicked && Mouse.GetState().LeftButton == ButtonState.Released)
            OnRelease();
        
        // setting hover
        if(DestRectangle.Contains(Mouse.GetState().Position) && !_isHover)
            OnHover();
        else if (!DestRectangle.Contains(Mouse.GetState().Position) && _isHover)
            ResetButton();
        
        
        
    }


    private void OnHover()
    {
        Debug.WriteLine("Hover");
        _isHover = true;
        color = Color.DarkGray;
    }
    private void OnClick()
    {
        Debug.WriteLine("Clicked");
        _isClicked = true;
        color = Color.Gray;
    }
    private void OnRelease()
    {
        Debug.WriteLine("Released");
        ResetButton();
    }

    private void ResetButton()
    {
        Debug.WriteLine("Reset");
        color = Color.White;
        _isClicked = false;
        _isHover = false;
    }
}