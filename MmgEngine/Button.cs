using System;
using Microsoft.Xna.Framework;

namespace MmgEngine;

public class Button : DrawableGameComponent
{
    #region Events
    public event EventHandler LeftClicked;
    public event EventHandler MiddleClicked;
    public event EventHandler RightClicked;
    public event EventHandler XButton1Clicked;
    public event EventHandler XButton2Clicked;
    #endregion

    #region Properties
    public SimpleImage Image { get; private set; }
    public HoverDetector HoverDetector { get; private set; }
    public Rectangle ActionBox { get; private set; }
    #endregion

    //Constructor
    public Button(Game game, Rectangle actionBox, SimpleImage texture = null, Alignment anchor = Alignment.Center, bool enabled = true, bool hasHover = false) : base(game)
    {
        actionBox.Location -= (actionBox.Size.ToVector2() * EngineStatics.Aligner(anchor)).ToPoint();
        
        if (hasHover)
            HoverDetector = new HoverDetector(game, actionBox, Alignment.TopLeft);
        
        ActionBox = new Rectangle(
            (int)(actionBox.X * EngineStatics.PartialScale),
            (int)(actionBox.Y * EngineStatics.PartialScale),
            (int)(actionBox.Width * EngineStatics.PartialScale),
            (int)(actionBox.Height * EngineStatics.PartialScale)
        );
        Image = texture;
        Enabled = enabled;
        Visible = texture is not null;
        if (texture is not null)
            DrawOrder = texture.DrawOrder;

        //ResetRectangle(null, null);
        //Configs.ResolutionChanged += ResetRectangle;
        Input.ButtonDown += Check;
        
    }

    #region Methods

    public override void Draw(GameTime gameTime) 
    {
        if (Image.Visible)
            Image.Draw(gameTime);
    }

    private void Check(object s, ButtonEventArgs e)
    {
        if (!Enabled || !ActionBox.Contains(e.Position))
            return;
        
        switch (e.Button)
        {
            case "LeftButton":
                LeftClicked?.Invoke(this, EventArgs.Empty);
                break;
            case "MiddleButton":
                MiddleClicked?.Invoke(this, EventArgs.Empty);
                break;
            case "RightButton":
                RightClicked?.Invoke(this, EventArgs.Empty);
                break;
            case "XButton1":
                XButton1Clicked?.Invoke(this, EventArgs.Empty);
                break;
            case "XButton2":
                XButton2Clicked?.Invoke(this, EventArgs.Empty);
                break;
        }
    }

    public override void Update(GameTime gameTime)
    {
        if (HoverDetector is not null && HoverDetector.Enabled)
            HoverDetector.Update(gameTime);
        base.Update(gameTime);
    }

    //private void ResetRectangle(object s, EventArgs e) => rectangle = new Rectangle((int)_position.X * Configs.Scale + Configs.XOffset, (int)_position.Y * Configs.Scale + Configs.YOffset, _size.X * Configs.Scale, _size.Y * Configs.Scale);

    public new void Dispose()
    {
        //Configs.ResolutionChanged -= ResetRectangle;
        Input.ButtonDown -= Check;
        base.Dispose();
    }
    #endregion
}