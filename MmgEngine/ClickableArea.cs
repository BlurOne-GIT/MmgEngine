using System;
using Microsoft.Xna.Framework;

namespace MmgEngine;

/// <summary>
/// A rectangle that emits events when clicking on it.
/// </summary>
public class ClickableArea : MouseHelper
{
    #region Events
    public event EventHandler? LeftButtonDown;
    public event EventHandler? MiddleButtonDown;
    public event EventHandler? RightButtonDown;
    public event EventHandler? XButton1Down;
    public event EventHandler? XButton2Down;
    public event EventHandler? LeftButtonUp;
    public event EventHandler? MiddleButtonUp;
    public event EventHandler? RightButtonUp;
    public event EventHandler? XButton1Up;
    public event EventHandler? XButton2Up;
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

    protected override Rectangle ActionBox => _actionBox;

    #region Fields
    private Rectangle _actionBox;
    private readonly Alignment _alignment;
    #endregion

    //Constructor
    public ClickableArea(Game game, Rectangle actionBox, bool allowOutsideHolding, Alignment alignment = Alignment.TopLeft)
        : base(game, allowOutsideHolding)
    {
        _alignment = alignment;
        actionBox.Location -= (actionBox.Size.ToVector2() * EngineStatics.Aligner(alignment)).ToPoint();
        _actionBox = actionBox;
        
        ButtonDown += OnButtonDown;
        ButtonUp += OnButtonUp;
    }

    private void OnButtonDown(object? sender, MouseButtons e)
    {
        if (e.HasFlag(MouseButtons.LeftButton))
            LeftButtonDown?.Invoke(this, EventArgs.Empty);
        if (e.HasFlag(MouseButtons.RightButton))
            RightButtonDown?.Invoke(this, EventArgs.Empty);
        if (e.HasFlag(MouseButtons.MiddleButton))
            MiddleButtonDown?.Invoke(this, EventArgs.Empty);
        if (e.HasFlag(MouseButtons.XButton1))
            XButton1Down?.Invoke(this, EventArgs.Empty);
        if (e.HasFlag(MouseButtons.XButton2))
            XButton2Down?.Invoke(this, EventArgs.Empty);
    }

    private void OnButtonUp(object? sender, MouseButtons e)
    {
        if (e.HasFlag(MouseButtons.LeftButton))
            LeftButtonUp?.Invoke(this, EventArgs.Empty);
        if (e.HasFlag(MouseButtons.RightButton))
            RightButtonUp?.Invoke(this, EventArgs.Empty);
        if (e.HasFlag(MouseButtons.MiddleButton))
            MiddleButtonUp?.Invoke(this, EventArgs.Empty);
        if (e.HasFlag(MouseButtons.XButton1))
            XButton1Up?.Invoke(this, EventArgs.Empty);
        if (e.HasFlag(MouseButtons.XButton2))
            XButton2Up?.Invoke(this, EventArgs.Empty);
    }

    protected override void Dispose(bool disposing)
    {
        ButtonDown -= OnButtonDown;
        ButtonUp -= OnButtonUp;
        base.Dispose(disposing);
    }
}