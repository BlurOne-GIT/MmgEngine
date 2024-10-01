using Microsoft.Xna.Framework;

namespace MmgEngine;

/// <summary>
/// A <see cref="ClickableArea"/> that always represents the whole game window.
/// </summary>
public class ClickableWindow(Game game)
    : ClickableArea(game, game.Window.ClientBounds with {Location = Point.Zero}, Alignment.TopLeft, OutsideBehaviour.None)
{
    protected override Rectangle ActionBox => Game.Window.ClientBounds with { Location = Point.Zero };
}