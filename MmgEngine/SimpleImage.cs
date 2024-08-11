using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MmgEngine;

/// <summary>
/// Simple image that draws a <see cref="Texture2D"/> on the screen.
/// </summary>
public class SimpleImage : DrawableGameComponent
{
    #region Fields
    private readonly Alignment _anchor;
    private float _rotation;
    private Vector2 _pivot;
    private Texture2D _texture;
    private Rectangle? _defaultSource;
    private Animation<Rectangle>? _animation;
    #endregion

    #region Properties
    public Rectangle CurrentSource => NullableCurrentSource ?? Texture.Bounds;
    private Rectangle? NullableCurrentSource => Animation?.CurrentFrame() ?? DefaultSource;
    
    public Texture2D Texture
    {
        get => _texture;
        set
        {
            _texture = value;
            RelocatePivot();
        }
    }
    
    public Rectangle? DefaultSource
    {
        get => _defaultSource;
        set { _defaultSource = value; RelocatePivot(); }
    }
    
    public Animation<Rectangle>? Animation
    {
        get => _animation;
        set
        {
            _animation = value; 
            RelocatePivot();
        }
    }
    
    public Vector2 Position { get; set; }
    public Color Color { get; set; } = Color.White;
    public float Opacity { get; set; } = 1f;
    public float Rotation { get => MathHelper.ToDegrees(_rotation); set => _rotation = MathHelper.ToRadians(value); }
    public Vector2 Scale { get; set; } = Vector2.One;
    public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;
    #endregion

    public SimpleImage(Game game, string texturePath, Vector2 position, int layer, Alignment anchor = Alignment.TopLeft)
        : this(game, game.Content.Load<Texture2D>(texturePath), position, layer, anchor) { }
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SimpleImage(Game game, Texture2D texture, Vector2 position, int layer, Alignment anchor = Alignment.TopLeft)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        : base(game)
    {
        _anchor = anchor;
        Texture = texture;
        Position = position;
        DrawOrder = layer;
    }

    private void RelocatePivot()
    {
        _pivot = CurrentSource.Size.ToVector2() * EngineStatics.Aligner(_anchor);
    }

    public override void Draw(GameTime gameTime)
    {
        Animation?.NextFrame();
        var spriteBatch = Game.Services.GetService<SpriteBatch>();
        spriteBatch.Draw(
            Texture,
            Position,
            NullableCurrentSource,
            Color * Opacity,
            _rotation,
            _pivot,
            Scale,
            SpriteEffects,
            DrawOrder * 0.1f
        );
    }

    /// <summary>
    /// For inheritance purposes.
    /// Draws another texture based on this image's parameters (can be modified).
    /// </summary>
    protected void DrawAnotherTexture(
        Texture2D texture,
        Vector2 positionOffset,
        int drawOrder,
        Rectangle? sourceRectangle = null,
        float opacityMultiplier = 1f,
        float rotationOffset = 0f,
        Vector2 pivot = default,
        Vector2 scaleMultiplier = default,
        SpriteEffects? spriteEffectsOverride = null
        )
    {
        if (scaleMultiplier == default) scaleMultiplier = Vector2.One;
        var spriteBatch = Game.Services.GetService<SpriteBatch>();
        spriteBatch.Draw(
            texture,
            Position + positionOffset,
            sourceRectangle,
            Color * Opacity * opacityMultiplier,
            _rotation + rotationOffset,
            pivot,
            Scale * scaleMultiplier,
            spriteEffectsOverride ?? SpriteEffects,
            drawOrder * 0.1f
        );
    }
}