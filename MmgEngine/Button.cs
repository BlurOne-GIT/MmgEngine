using System;
using Microsoft.Xna.Framework;

namespace MmgEngine;

/// <summary>
/// A rectangle that emits events when clicking on it. Can have a <see cref="SimpleImage"/> and a <see cref="HoverDetector"/>.
/// </summary>
public class Button : GameComponent
{
    #region Events
    public event EventHandler<ButtonEventArgs>? Clicked; 
    public event EventHandler<ButtonEventArgs>? LeftClicked;
    public event EventHandler<ButtonEventArgs>? MiddleClicked;
    public event EventHandler<ButtonEventArgs>? RightClicked;
    public event EventHandler<ButtonEventArgs>? XButton1Clicked;
    public event EventHandler<ButtonEventArgs>? XButton2Clicked;
    #endregion

    #region Properties
    public Vector2 Position
    {
        get => _actionBox.Location.ToVector2() + _actionBox.Size.ToVector2() * EngineStatics.Aligner(Alignment.TopLeft);
        set => _actionBox.Location = (value - _actionBox.Size.ToVector2() * EngineStatics.Aligner(Alignment.TopLeft)).ToPoint();
    }

    public Vector2 Size
    {
        get => _actionBox.Size.ToVector2();
        set
        {
            var oldExternalLocation = Position;
            _actionBox.Size = value.ToPoint();
            _actionBox.Location = (oldExternalLocation - _actionBox.Size.ToVector2() * EngineStatics.Aligner(_alignment)).ToPoint();
        }
    }
    #endregion

    #region Fields
    private Rectangle _actionBox;
    private readonly Alignment _alignment;
    #endregion

    //Constructor
    public Button(Game game, Rectangle actionBox, Alignment alignment = Alignment.TopLeft) : base(game)
    {
        _alignment = alignment;
        actionBox.Location -= (actionBox.Size.ToVector2() * EngineStatics.Aligner(alignment)).ToPoint();
        _actionBox = actionBox;
        
        Input.ButtonDown += Check;
    }

    #region Methods
    private void Check(object? s, ButtonEventArgs e)
    {
        if (!Enabled || !_actionBox.Contains(e.Position))
            return;
        
        switch (e.Button)
        {
            case "LeftButton":
                LeftClicked?.Invoke(this, e);
                break;
            case "MiddleButton":
                MiddleClicked?.Invoke(this, e);
                break;
            case "RightButton":
                RightClicked?.Invoke(this, e);
                break;
            case "XButton1":
                XButton1Clicked?.Invoke(this, e);
                break;
            case "XButton2":
                XButton2Clicked?.Invoke(this, e);
                break;
        }
        Clicked?.Invoke(this, e);
    }

    protected override void Dispose(bool disposing)
    {
        Input.ButtonDown -= Check;
        base.Dispose(disposing);
    }
    #endregion
}