using System;
using Microsoft.Xna.Framework;

namespace MmgEngine;

/// <summary>
/// Rectangle that emits events when the mouse is hovering over it.
/// </summary>
public class HoverDetector : GameComponent
{
    public event EventHandler Hovered;
    public event EventHandler Unhovered;

    public Vector2 Position
    {
        get => _actionBox.Location.ToVector2() + _actionBox.Size.ToVector2() * EngineStatics.Aligner(Alignment.TopLeft);
        set => _actionBox.Location = (value - _actionBox.Size.ToVector2() * EngineStatics.Aligner(_alignment)).ToPoint();
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
    
    private Rectangle _actionBox;
    private readonly Alignment _alignment;
    public bool Hovering { get; private set; }

    public HoverDetector(Game game, Rectangle actionBox, Alignment alignment = Alignment.TopLeft) : base(game)
    {
        _alignment = alignment;
        actionBox.Location -= (actionBox.Size.ToVector2() * EngineStatics.Aligner(alignment)).ToPoint();
        _actionBox = actionBox;
    }
    
    public override void Update(GameTime gameTime)
    {
        if (!Game.IsActive)
            return;

        var contains = _actionBox.Contains(Input.MousePoint);
        
        if (contains == Hovering)
            return;
        
        Hovering = contains;
        
        (Hovering ? Hovered : Unhovered)?.Invoke(this, EventArgs.Empty);
    }
}