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
    public Color Color { get; set; }
    public float Opacity { get; set; }
    public float Rotation { get => MathHelper.ToDegrees(_rotation); set => _rotation = MathHelper.ToRadians(value); }
    public float Scale { get; set; }
    public string Text { get => _text; set {_text = value; RelocatePivot();} }

    #endregion

    public TextComponent(Game game, SpriteFont font, string defaultText, Vector2 position, int layer, bool visible = true, Alignment anchor = Alignment.TopLeft, Color? color = null, float opacity = 1f, float rotation = 0f, float scale = 1f) : base(game)
    {
        _font = font;
        Position = position;
        DrawOrder = layer;
        Visible = visible;
        _anchor = anchor;
        Color = color ?? Color.White;
        Opacity = opacity;
        Rotation = rotation;
        Scale = scale;
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
            SpriteEffects.None,
            Scale,
            DrawOrder * 0.1f
        );
    }
}