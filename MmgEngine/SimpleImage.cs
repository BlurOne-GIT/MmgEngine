using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MmgEngine;

/// <summary>
/// Simple image that draws a <see cref="Texture2D"/> on the screen.
/// </summary>
public class SimpleImage : DrawableGameComponent
{
    #region Fields
    protected Texture2D Texture;
    private readonly Alignment _anchor;
    private float _rotation;
    private Vector2 _pivot;
    #endregion

    #region Properties
    public Vector2 Position { get; set; }
    public Animation<Rectangle> Animation { get; private set; }
    public Color Color { get; set; } = Color.White;
    public float Opacity { get; set; } = 1f;
    public float Rotation { get => MathHelper.ToDegrees(_rotation); set => _rotation = MathHelper.ToRadians(value); }
    public Vector2 Scale { get; set; } = Vector2.One;
    public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;
    #endregion

    public SimpleImage(Game game, Texture2D texture, Vector2 position, int layer, Alignment anchor = Alignment.TopLeft, Animation<Rectangle> animation = null) : base(game)
    {
        Texture = texture;
        Position = position;
        DrawOrder = layer;
        _anchor = anchor;
        Animation = animation;
        RelocatePivot();
    }

    private void RelocatePivot()
    {
        var frame = Animation?.CurrentFrame() ?? Texture.Bounds;
        _pivot = frame.Size.ToVector2() * EngineStatics.Aligner(_anchor);
    }

    public override void Draw(GameTime gameTime)
    {
        var spriteBatch = Game.Services.GetService<SpriteBatch>();
        spriteBatch.Draw(
            Texture,
            Position,
            Animation?.NextFrame(),
            Color * Opacity,
            _rotation,
            _pivot,
            Scale,
            SpriteEffects,
            DrawOrder * 0.1f
        );
    }

    public void ChangeTexture(Texture2D texture) {
        Animation = null;
        Texture = texture;
        RelocatePivot();
    }

    public void ChangeAnimatedTexture(Texture2D texture, Animation<Rectangle> animation)
    {
        Texture = texture;
        Animation = animation ?? Animation;
        RelocatePivot();
    }

    public void ChangeAnimation(Animation<Rectangle> animation)
    {
        Animation = animation;
        RelocatePivot();
    }
}