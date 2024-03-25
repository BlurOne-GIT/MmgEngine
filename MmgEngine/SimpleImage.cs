using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MmgEngine;

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
    public Color Color { get; set; }
    public float Opacity { get; set; }
    public float Rotation { get => MathHelper.ToDegrees(_rotation); set => _rotation = MathHelper.ToRadians(value); }
    public float Scale { get; set; }
    #endregion

    public SimpleImage(Game game, Texture2D texture, Vector2 position, int layer, bool visible = true, Alignment anchor = Alignment.Center, Animation<Rectangle> animation = null, Color? color = null, float opacity = 1f, float rotation = 0f, float scale = 1f) : base(game)
    {
        Texture = texture;
        Position = position;
        DrawOrder = layer;
        Visible = visible;
        _anchor = anchor;
        Animation = animation;
        Color = color ?? Color.White;
        Opacity = opacity;
        Rotation = rotation;
        Scale = scale;
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
            Position * EngineStatics.PartialScale,
            Animation?.NextFrame(),
            Color * Opacity,
            _rotation,
            _pivot,
            Scale * EngineStatics.PartialScale,
            SpriteEffects.None,
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