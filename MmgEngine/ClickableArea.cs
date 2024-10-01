using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MmgEngine;

[Flags]
public enum MouseButtons : byte
{
    None = 0,
    LeftButton = 1,
    RightButton = 2,
    MiddleButton = 4,
    XButton1 = 8,
    XButton2 = 16,
    All = LeftButton | RightButton | MiddleButton | XButton1 | XButton2
}

/// <!--summary>
/// Class that expands upon <see cref="Microsoft.Xna.Framework.Input.Mouse"/>> and <see cref="Microsoft.Xna.Framework.Input"/> to handle mouse input with better events and properties.
/// </summary-->
/// <summary>
/// A rectangle that emits events when clicking on it.
/// </summary>
public class ClickableArea : HoverableArea
{
    #region Events
    public event EventHandler<MouseButtons>? ButtonDown;
    public event EventHandler<MouseButtons>? ButtonUp;
    
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

    #region Fields
    private readonly OutsideBehaviour _outsideBehaviour;
    private MouseButtons _currentButtons;
    private MouseButtons _previousButtons;
    private MouseButtons _maskIn = MouseButtons.All;
    #endregion

    #region Properties
    public bool LeftButton => _currentButtons.HasFlag(MouseButtons.LeftButton);
    public bool RightButton => _currentButtons.HasFlag(MouseButtons.RightButton);
    public bool MiddleButton => _currentButtons.HasFlag(MouseButtons.MiddleButton);
    public bool XButton1 => _currentButtons.HasFlag(MouseButtons.XButton1);
    public bool XButton2 => _currentButtons.HasFlag(MouseButtons.XButton2);
    #endregion
    
    [Flags]
    public enum OutsideBehaviour
    {
        None = 0b00, // Set to false when exiting action box, with event
        HoldOut = 0b01, // Don't update entirely while outside the action box
        EventIn = 0b10  // Fire event on reentering if clicked
    }
    
    public ClickableArea(Game game, Rectangle actionBox, Alignment alignment = Alignment.TopLeft, OutsideBehaviour outsideBehaviour = OutsideBehaviour.HoldOut)
        : base(game, actionBox, alignment)
    {
        PreSetValues();
        if (!(_outsideBehaviour = outsideBehaviour).HasFlag(OutsideBehaviour.EventIn))
            Hovered += OnHovered;
    }

    private void OnHovered(object? sender, EventArgs e) => _maskIn = ~GetMouseButtonsFromState(Mouse.GetState()) & MouseButtons.All;

    protected override void OnEnabledChanged(object sender, EventArgs args)
    {
        if (Enabled)
            PreSetValues();
        base.OnEnabledChanged(sender, args);
    }

    private void PreSetValues()
    {
        var mouseState = Mouse.GetState();
        MousePoint = ((mouseState.Position.ToVector2() - EngineStatics.Offset) / EngineStatics.Scale).ToPoint();
        _currentButtons = GetMouseButtonsFromState(mouseState);
    }

    private static MouseButtons GetMouseButtonsFromState(MouseState mouseState)
        => (MouseButtons)
           (
               (int)mouseState.LeftButton        |
               (int)mouseState.RightButton  << 1 |
               (int)mouseState.MiddleButton << 2 |
               (int)mouseState.XButton1     << 3 |
               (int)mouseState.XButton2     << 4
           );

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        if (!Game.IsActive) return;

        if (!Hovering && _currentButtons is MouseButtons.None)
            return;
        
        _previousButtons = _currentButtons;
        if (Hovering)
            _currentButtons = GetMouseButtonsFromState(Mouse.GetState());
        else if (_outsideBehaviour.HasFlag(OutsideBehaviour.HoldOut))
            _currentButtons &= GetMouseButtonsFromState(Mouse.GetState());
        else
            _currentButtons = MouseButtons.None;

        if (!_outsideBehaviour.HasFlag(OutsideBehaviour.EventIn))
        {
            _maskIn |= ~_currentButtons;
            _currentButtons &= _maskIn;
        }
        
        var pressedButtons = _currentButtons & ~_previousButtons;
        var releasedButtons = _previousButtons & ~_currentButtons;
        
        if (pressedButtons is not MouseButtons.None)
        {
            ButtonDown?.Invoke(this, pressedButtons);
            InvokeSpecificButtonEvents(pressedButtons, true);
        }

        if (releasedButtons is MouseButtons.None) return;
        
        ButtonUp?.Invoke(this, releasedButtons);
        InvokeSpecificButtonEvents(releasedButtons, false);
    }
    
    private void InvokeSpecificButtonEvents(MouseButtons buttons, bool isPressed)
    {
        if (buttons.HasFlag(MouseButtons.LeftButton))
            (isPressed ? LeftButtonDown : LeftButtonUp)?.Invoke(this, EventArgs.Empty);
        if (buttons.HasFlag(MouseButtons.RightButton))
            (isPressed ? RightButtonDown : RightButtonUp)?.Invoke(this, EventArgs.Empty);
        if (buttons.HasFlag(MouseButtons.MiddleButton))
            (isPressed ? MiddleButtonDown : MiddleButtonUp)?.Invoke(this, EventArgs.Empty);
        if (buttons.HasFlag(MouseButtons.XButton1))
            (isPressed ? XButton1Down : XButton1Up)?.Invoke(this, EventArgs.Empty);
        if (buttons.HasFlag(MouseButtons.XButton2))
            (isPressed ? XButton2Down : XButton2Up)?.Invoke(this, EventArgs.Empty);
    }

    protected override void Dispose(bool disposing)
    {
        Hovered -= OnHovered;
        base.Dispose(disposing);
    }
}