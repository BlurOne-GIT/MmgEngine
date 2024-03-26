using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MmgEngine;

public class Animation<T>
{
    #region Fields
    public T[] Frames { get; }
    public int Position { get; set; }
    private int _frameDelay;
    public bool IsLooped { get; set; }
    public int FrameDuration { get; set; }
    public bool Paused { get; set; } = false;
    #endregion

    public Animation(T[] frames, bool looped, int frameDuration = 1)
    {
        Frames = frames;
        IsLooped = looped;
        Position = Frames.Length - 1;
        FrameDuration = frameDuration - 1;
        _frameDelay = FrameDuration;
    }

    public void Start() => Position = 0;

    public T NextFrame()
    {
        if (Paused)
            return CurrentFrame();

        if (Position >= Frames.Length)
            if (IsLooped)
                Position = 0;
            else
                return Frames[Position-1];

        if (--_frameDelay > 0)
            return Frames[Position];

        _frameDelay = FrameDuration;

        return Frames[Paused ? Position : Position++];
    }

    public T CurrentFrame() => Position < Frames.Length ? Frames[Position] : Frames[Position-1];

    public static Animation<Rectangle> TextureAnimation(Point frameSize, Point bounds, bool looped, int frameDuration)
    {
        var frames = new List<Rectangle>();
        for (int y = 0; y < bounds.Y; y += frameSize.Y)
            for (int x = 0; x < bounds.X; x += frameSize.X)
                frames.Add(new Rectangle(x, y, frameSize.X, frameSize.Y));

        return new Animation<Rectangle>(frames.ToArray(), looped, frameDuration);
    }
}