using System;
using Microsoft.Xna.Framework;

namespace MmgEngine;

public static class EngineStatics
{
    /// <summary>
    /// Event called when the <see cref="Scale"/> or <see cref="Offset"/> is changed.
    /// </summary>
    public static event EventHandler ViewportChanged;
    private static Vector2 _scale = Vector2.One;
    private static Vector2 _offset = Vector2.Zero;
    /// <summary>
    /// Scale to be used by <see cref="EngineGame.ViewportMatrix"/> to Scale the drawn content.
    /// Also used by <see cref="Button"/> and <see cref="HoverDetector"/> to scale the action box.
    /// </summary>
    public static Vector2 Scale
    {
        get => _scale;
        set
        {
            _scale = value;
            ViewportChanged?.Invoke(_scale, EventArgs.Empty);
        }
    }
    /// <summary>
    /// Not affected by <see cref="Scale"/>.
    /// Offset to be used by <see cref="EngineGame.ViewportMatrix"/> to offset the drawn content.
    /// Also used by <see cref="Button"/> and <see cref="HoverDetector"/> to scale the action box.
    /// </summary>
    public static Vector2 Offset
    {
        get => _offset;
        set
        {
            _offset = value;
            ViewportChanged?.Invoke(_offset, EventArgs.Empty);
        }
    }
    internal static Vector2 Aligner(Alignment alignment) => new(((byte)alignment&0b_0011)/2f, ((byte)alignment>>2)/2f);
}