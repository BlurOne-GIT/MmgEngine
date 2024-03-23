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

    public TextComponent(Game game, SpriteFont font, string defaultText, Vector2 position, int layer, bool visible = true, Alignment anchor = Alignment.CenterLeft, Color? color = null, float opacity = 1f, float rotation = 0f, float scale = 1f) : base(game)
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

    private void RelocatePivot()
    {
        _pivot = _anchor switch
        {
            Alignment.TopLeft => Vector2.Zero,
            Alignment.TopCenter => new Vector2(_font.MeasureString(_text).X / 2, 0f),
            Alignment.TopRight => new Vector2(_font.MeasureString(_text).X, 0f),
            Alignment.CenterLeft => new Vector2(0f, _font.MeasureString(_text).Y / 2),
            Alignment.Center => new Vector2(_font.MeasureString(_text).X / 2, _font.MeasureString(_text).Y / 2),
            Alignment.CenterRight => new Vector2(_font.MeasureString(_text).X, _font.MeasureString(_text).Y / 2),
            Alignment.BottomLeft => new Vector2(0f, _font.MeasureString(_text).Y),
            Alignment.BottomCenter => new Vector2(_font.MeasureString(_text).X / 2, _font.MeasureString(_text).Y),
            Alignment.BottomRight => new Vector2(_font.MeasureString(_text).X, _font.MeasureString(_text).Y),
            _ => Vector2.Zero
        };
    }

    public override void Draw(GameTime gameTime)
    {
        SpriteBatch spriteBatch = Game.Services.GetService<SpriteBatch>();
        spriteBatch.DrawString
        (
            _font,
            Text,
            Position * TheLightbulb.Configs.PartialScale,
            Color * Opacity,
            _rotation,
            _pivot,
            Scale * TheLightbulb.Configs.PartialScale,
            SpriteEffects.None,
            DrawOrder * 0.1f
        );
    }
}