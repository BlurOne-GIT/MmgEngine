using System;
using Microsoft.Xna.Framework;

namespace MmgEngine;

public static class EngineStatics
{
    public static float PartialScale { get; set; } = 1f;
    public static bool WindowFocused { get; private set; }
    public static void Focus(object s, EventArgs e) => WindowFocused = true;
    public static void UnFocus(object s, EventArgs e) => WindowFocused = false;
    internal static Vector2 Aligner(Alignment alignment) => new(((byte)alignment&0b_0011)/2f, ((byte)alignment>>2)/2f);
}