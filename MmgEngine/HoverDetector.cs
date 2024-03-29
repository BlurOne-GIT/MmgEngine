using System;
using Microsoft.Xna.Framework;

namespace MmgEngine;

public class HoverDetector : GameComponent
{
    public event EventHandler Hovered;
    public event EventHandler Unhovered;
    public Rectangle ActionBox { get; private set; }
    public bool Hovering { get; private set; }

    public HoverDetector(Game game, Rectangle actionBox, Alignment alignment = Alignment.TopLeft, bool enabled = true) : base(game)
    {
        actionBox.Location -= (actionBox.Size.ToVector2() * EngineStatics.Aligner(alignment)).ToPoint();
        
        ActionBox = new Rectangle(
            (int)(actionBox.X * EngineStatics.PartialScale),
            (int)(actionBox.Y * EngineStatics.PartialScale),
            (int)(actionBox.Width * EngineStatics.PartialScale),
            (int)(actionBox.Height * EngineStatics.PartialScale)
        );
        Enabled = enabled;
    }

    public override void Update(GameTime gameTime)
    {
        if (!Game.IsActive)
            return;

        if (ActionBox.Contains(Input.MousePoint))
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
}