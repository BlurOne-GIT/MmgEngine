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
    XButton2 = 16
}

/// <summary>
/// Class that expands upon <see cref="Microsoft.Xna.Framework.Input.Mouse"/>> and <see cref="Microsoft.Xna.Framework.Input"/> to handle mouse input with better events and properties.
/// </summary>
public class MouseHelper : GameComponent
{
    #region Events
    public event EventHandler<MouseButtons>? ButtonDown;
    public event EventHandler<MouseButtons>? ButtonUp;
    #endregion

    #region Fields
    private readonly bool _allowOutsideHolding;
    private bool _leftButton;
    private bool _middleButton;
    private bool _rightButton;
    private bool _xButton1;
    private bool _xButton2;
    private MouseButtons _pressed;
    private MouseButtons _released;
    #endregion

    #region Properties
    public bool LeftButton { get => _leftButton; private set => CheckMouseInput(ref _leftButton, value, MouseButtons.LeftButton); }
    public bool MiddleButton { get => _middleButton; private set => CheckMouseInput(ref _middleButton, value, MouseButtons.MiddleButton); }
    public bool RightButton { get => _rightButton; private set => CheckMouseInput(ref _rightButton, value, MouseButtons.RightButton); }
    public bool XButton1 { get => _xButton1; private set => CheckMouseInput(ref _xButton1, value, MouseButtons.XButton1); }
    public bool XButton2 { get => _xButton2; private set => CheckMouseInput(ref _xButton2, value, MouseButtons.XButton2); }
    protected virtual Rectangle ActionBox => Game.Window.ClientBounds with { Location = Point.Zero };
    #endregion
    
    public static Point MousePoint { get; private set; }
    public static Vector2 MouseVector => MousePoint.ToVector2();
    private static MouseState _lastMouseState;
    private static long _lastGameTime = -1L;
    
    public MouseHelper(Game game, bool allowOutsideHolding) : base(game)
    {
        _allowOutsideHolding = allowOutsideHolding;
        PreSetValues();
    }

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
        _leftButton = Convert.ToBoolean(mouseState.LeftButton);
        _middleButton = Convert.ToBoolean(mouseState.MiddleButton);
        _rightButton = Convert.ToBoolean(mouseState.RightButton);
        _xButton1 = Convert.ToBoolean(mouseState.XButton1);
        _xButton2 = Convert.ToBoolean(mouseState.XButton2);
    }

    public override void Update(GameTime gameTime)
    {
        if (gameTime.TotalGameTime.Ticks != _lastGameTime)
        {
            _lastGameTime = gameTime.TotalGameTime.Ticks;
            _lastMouseState = Mouse.GetState();
            MousePoint = ((_lastMouseState.Position.ToVector2() - EngineStatics.Offset) / EngineStatics.Scale).ToPoint();
        }
        _pressed = _released = MouseButtons.None;
        LeftButton = Convert.ToBoolean(_lastMouseState.LeftButton);
        MiddleButton = Convert.ToBoolean(_lastMouseState.MiddleButton);
        RightButton = Convert.ToBoolean(_lastMouseState.RightButton);
        XButton1 = Convert.ToBoolean(_lastMouseState.XButton1);
        XButton2 = Convert.ToBoolean(_lastMouseState.XButton2);
        if (_pressed != MouseButtons.None)
            ButtonDown?.Invoke(this, _pressed);
        if (_released != MouseButtons.None)
            ButtonUp?.Invoke(this, _released);
        base.Update(gameTime);
    }
    
    private void CheckMouseInput(ref bool refValue, bool value, MouseButtons button)
    {
        if (!Game.IsActive)
            return;

        if (!ActionBox.Contains(MousePoint))
        {
            if (!_allowOutsideHolding)
                value = false;
            else if (value)
                return;
        }

        if (refValue == value)
            return;
        
        refValue = value;
        
        if (refValue)
            _pressed |= button;
        else
            _released |= button;
    }
}