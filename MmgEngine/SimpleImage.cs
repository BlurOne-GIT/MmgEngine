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
        this.Texture = texture;
        Position = position;
        this.DrawOrder = layer;
        this.Visible = visible;
        this._anchor = anchor;
        Animation = animation;
        this.Color = color ?? Color.White;
        Opacity = opacity;
        Rotation = rotation;
        Scale = scale;
        RelocatePivot();
    }

    private void RelocatePivot()
    {   
        if (Animation is null)
            _pivot = _anchor switch
            {
                Alignment.TopLeft => Vector2.Zero,
                Alignment.TopCenter => new Vector2(Texture.Width / 2, 0f),
                Alignment.TopRight => new Vector2(Texture.Width, 0f),
                Alignment.CenterLeft => new Vector2(0f, Texture.Height / 2),
                Alignment.Center => new Vector2(Texture.Width / 2, Texture.Height / 2),
                Alignment.CenterRight => new Vector2(Texture.Width, Texture.Height / 2),
                Alignment.BottomLeft => new Vector2(0f, Texture.Height),
                Alignment.BottomCenter => new Vector2(Texture.Width / 2, Texture.Height),
                Alignment.BottomRight => new Vector2(Texture.Width, Texture.Height),
                _ => Vector2.Zero
            };
        else
            _pivot = _anchor switch
            {
                Alignment.TopLeft => Vector2.Zero,
                Alignment.TopCenter => new Vector2(Animation.CurrentFrame().Width / 2, 0f),
                Alignment.TopRight => new Vector2(Animation.CurrentFrame().Width, 0f),
                Alignment.CenterLeft => new Vector2(0f, Animation.CurrentFrame().Height / 2),
                Alignment.Center => new Vector2(Animation.CurrentFrame().Width / 2, Animation.CurrentFrame().Height / 2),
                Alignment.CenterRight => new Vector2(Animation.CurrentFrame().Width, Animation.CurrentFrame().Height / 2),
                Alignment.BottomLeft => new Vector2(0f, Animation.CurrentFrame().Height),
                Alignment.BottomCenter => new Vector2(Animation.CurrentFrame().Width / 2, Animation.CurrentFrame().Height),
                Alignment.BottomRight => new Vector2(Animation.CurrentFrame().Width, Animation.CurrentFrame().Height),
                _ => Vector2.Zero
            };
    }

    public override void Draw(GameTime gameTime)
    {
        SpriteBatch spriteBatch = Game.Services.GetService<SpriteBatch>();
        spriteBatch.Draw(
            Texture,
            Position * TheLightbulb.Configs.PartialScale,
            Animation?.NextFrame(),
            Color * Opacity,
            _rotation,
            _pivot,
            Scale * TheLightbulb.Configs.PartialScale,
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

public enum Alignment
{
    TopLeft,
    TopCenter,
    TopRight,
    CenterLeft,
    Center,
    CenterRight,
    BottomLeft,
    BottomCenter,
    BottomRight
}