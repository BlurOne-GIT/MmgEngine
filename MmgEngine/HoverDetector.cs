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
        
        UpdateActionBox(this, EventArgs.Empty);
        
        EngineStatics.ViewportChanged += UpdateActionBox;
    }

    private void UpdateActionBox(object s, EventArgs e)
    {
        _actionBox = new Rectangle(
            (_actionBox.Location.ToVector2() * EngineStatics.Scale + EngineStatics.Offset).ToPoint(),
            (_actionBox.Size.ToVector2() * EngineStatics.Scale).ToPoint()
        );
    }
    
    public override void Update(GameTime gameTime)
    {
        if (!Game.IsActive)
            return;

        if (_actionBox.Contains(Input.MousePoint))
        {
            if (Hovering) return;
            Hovering = true;
            Hovered?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            if (!Hovering) return;
            Hovering = false;
            Unhovered?.Invoke(this, EventArgs.Empty);
        }
    }

    protected override void Dispose(bool disposing)
    {
        EngineStatics.ViewportChanged -= UpdateActionBox;
        base.Dispose(disposing);
    }
}