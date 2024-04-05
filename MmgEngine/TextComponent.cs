using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MmgEngine;

public class TextComponent : DrawableGameComponent
{
    #region Fields
    private readonly SpriteFont _font;
    private readonly Alignment _anchor;
    private string _text;
    private float _rotation;
    private Vector2 _pivot;
    #endregion

    #region Properties
    public Vector2 Position { get; set; }
    public Color Color { get; set; } = Color.White;
    public float Opacity { get; set; } = 1f;
    public float Rotation { get => MathHelper.ToDegrees(_rotation); set => _rotation = MathHelper.ToRadians(value); }
    public float Scale { get; set; } = 1f;
    public string Text { get => _text; set {_text = value; RelocatePivot();} }
    public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;

    #endregion

    public TextComponent(Game game, SpriteFont font, string defaultText, Vector2 position, int layer, Alignment anchor = Alignment.TopLeft) : base(game)
    {
        _font = font;
        Position = position;
        DrawOrder = layer;
        _anchor = anchor;
        Text = defaultText;
    }

    private void RelocatePivot() => _pivot = _font.MeasureString(_text) * EngineStatics.Aligner(_anchor);

    public override void Draw(GameTime gameTime)
    {
        var spriteBatch = Game.Services.GetService<SpriteBatch>();
        spriteBatch.DrawString
        (
            _font,
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
}