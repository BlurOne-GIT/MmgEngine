using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MmgEngine;

public class ButtonEventArgs : EventArgs
{
    public ButtonEventArgs(string button, Point position) { Button = button; Position = position; }
    public string Button { get; }
    public Point Position { get; }
}

public static class Input
{
    #region Events
    public static event EventHandler<ButtonEventArgs> ButtonDown;
    public static event EventHandler<ButtonEventArgs> ButtonUp;
    #endregion

    #region Fields
    private static bool _leftButton;
    private static bool _middleButton;
    private static bool _rightButton;
    private static bool _xButton1;
    private static bool _xButton2;
    private static Point _position;
    private static bool[] _keys = new bool[255];
    #endregion

    #region Properties
    public static bool ShowCursor { get; set; }
    public static Point MousePoint { get; set; }
    private static bool LeftButton { set => CheckMouseInput(ref _leftButton, value); }
    private static bool MiddleButton { set => CheckMouseInput(ref _middleButton, value); }
    private static bool RightButton { set => CheckMouseInput(ref _rightButton, value); }
    private static bool XButton1 { set => CheckMouseInput(ref _xButton1, value); }
    private static bool XButton2 { set => CheckMouseInput(ref _xButton2, value); }
    #endregion

    #region Methods
    public static void UpdateMouseInput(MouseState mouseState)
    {
        LeftButton = Convert.ToBoolean(mouseState.LeftButton);
        MiddleButton = Convert.ToBoolean(mouseState.MiddleButton);
        RightButton = Convert.ToBoolean(mouseState.RightButton);
        XButton1 = Convert.ToBoolean(mouseState.XButton1);
        XButton2 = Convert.ToBoolean(mouseState.XButton2);
        _position = mouseState.Position;
    }

    public static void StoreKeyDown(object s, InputKeyEventArgs e)
        => _keys[(int)e.Key] = true;
    
    public static void StoreKeyUp(object s, InputKeyEventArgs e)
        => _keys[(int)e.Key] = false;

    private static void CheckMouseInput(ref bool button, bool value, [CallerMemberName] string name = null)
    {
        if (button == value)
            return;
        
        button = value;
        if (!EngineStatics.WindowFocused || MousePoint.X < 0 || MousePoint.Y < 0 || MousePoint.X > 800 || MousePoint.Y > 800)
            return;
        
        (value ? ButtonDown : ButtonUp)?.Invoke(null, new ButtonEventArgs(name, _position));
    }
    #endregion
}