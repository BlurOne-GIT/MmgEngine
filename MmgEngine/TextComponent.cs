using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MmgEngine;

/// <summary>
/// Text component that can be drawn on the screen.
/// </summary>
public class TextComponent : DrawableGameComponent
{
    #region Fields

    private readonly Alignment _anchor;
    private string _text;
    private float _rotation;
    private Vector2 _pivot;
    #endregion

    #region Properties
    public SpriteFont Font { get; }
    public Vector2 Position { get; set; }
    public Color Color { get; set; } = Color.White;
    public float Opacity { get; set; } = 1f;
    public float Rotation { get => MathHelper.ToDegrees(_rotation); set => _rotation = MathHelper.ToRadians(value); }
    public Vector2 Scale { get; set; } = Vector2.One;
    public string Text { get => _text; set { _text = value; RelocatePivot(); } }
    public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;
    public Vector2 TextSize => Font.MeasureString(_text);

    #endregion

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public TextComponent(Game game, SpriteFont font, string defaultText, Vector2 position, int layer, Alignment anchor = Alignment.TopLeft) : base(game)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        Font = font;
        Position = position;
        DrawOrder = layer;
        _anchor = anchor;
        Text = defaultText;
    }

    private void RelocatePivot() => _pivot = Font.MeasureString(_text) * EngineStatics.Aligner(_anchor);

    public override void Draw(GameTime gameTime) =>
        Game.Services.GetService<SpriteBatch>().DrawString(
            Font,
            Text,
            Position,
            Color * Opacity,
            _rotation,
            _pivot,
            Scale,
            SpriteEffects,
            DrawOrder * 0.1f
        );
}