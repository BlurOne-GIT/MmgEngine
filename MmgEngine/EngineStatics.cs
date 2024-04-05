using System;
using Microsoft.Xna.Framework;

namespace MmgEngine;

public static class EngineStatics
{
    public static event EventHandler ViewportChanged;
    private static Vector2 _scale = Vector2.One;
    private static Vector2 _offset = Vector2.Zero;
    public static Vector2 Scale
    {
        get => _scale;
        set
        {
            _scale = value;
            ViewportChanged?.Invoke(_scale, EventArgs.Empty);
        }
    }
    public static Vector2 Offset
    {
        get => _offset;
        set
        {
            _offset = value;
            ViewportChanged?.Invoke(_offset, EventArgs.Empty);
        }
    }
    public static bool WindowFocused { get; private set; }
    public static void Focus(object s, EventArgs e) => WindowFocused = true;
    public static void UnFocus(object s, EventArgs e) => WindowFocused = false;
    internal static Vector2 Aligner(Alignment alignment) => new(((byte)alignment&0b_0011)/2f, ((byte)alignment>>2)/2f);
}