using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MmgEngine;

public class Animation<T>
{
    #region Fields
    private readonly T[] _frames;
    private int _pos;
    private int _frameDelay;
    private readonly bool _isLooped;
    private readonly int _frameDuration;
    public bool Paused { get; set; } = false;
    #endregion

    public Animation(T[] frames, bool looped, int frameDuration = 1)
    {
        _frames = frames;
        _isLooped = looped;
        _pos = _frames.Length - 1;
        _frameDuration = frameDuration - 1;
        _frameDelay = _frameDuration;
    }

    public void Start() => _pos = 0;

    public T NextFrame()
    {
        if (Paused)
            return CurrentFrame();

        if (_pos > _frames.Length - 1)
        {
            if (_isLooped)
                _pos = 0;
            else
                return _frames[_pos-1];
        }

        if (_frameDelay-- > 0)
            return _frames[_pos];

        _frameDelay = _frameDuration;

        return _frames[Paused ? _pos : _pos++];
    }

    public T CurrentFrame()
    {
        return _pos < _frames.Length ? _frames[_pos] : _frames[_pos-1];
    }

    public T[] GetFrames() => _frames;

    public static Animation<Rectangle> TextureAnimation(Point frameSize, Point bounds, bool looped, int frameDuration)
    {
        var frames = new List<Rectangle>();
        for (int y = 0; y < bounds.Y; y += frameSize.Y)
        {
            for (int x = 0; x < bounds.X; x += frameSize.X)
            {
                frames.Add(new Rectangle(x, y, frameSize.X, frameSize.Y));
            }
        }

        return new Animation<Rectangle>(frames.ToArray(), looped, frameDuration);
    }
}