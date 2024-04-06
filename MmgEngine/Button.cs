using System;
using Microsoft.Xna.Framework;

namespace MmgEngine;

/// <summary>
/// A rectangle that emits events when clicking on it. Can have a <see cref="SimpleImage"/> and a <see cref="HoverDetector"/>.
/// </summary>
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
    public Vector2 Position
    {
        get => new(_actionBox.X, _actionBox.Y);
        set
        {
            _actionBox.Location = (value - _actionBox.Size.ToVector2() * EngineStatics.Aligner(Alignment.TopLeft)).ToPoint();
            if (HoverDetector != null) HoverDetector.Position = value;
        }
    }

    public Vector2 Size
    {
        get => new(_actionBox.Width, _actionBox.Height);
        set
        {
            _actionBox.Size = value.ToPoint();
            _actionBox.Location -= (_actionBox.Size.ToVector2() * EngineStatics.Aligner(_alignment)).ToPoint();
            if (HoverDetector != null) HoverDetector.Size = value;
        }
    }
    #endregion

    #region Fields
    private Rectangle _actionBox;
    private readonly Alignment _alignment;
    #endregion

    //Constructor
    public Button(Game game, Rectangle actionBox, SimpleImage texture = null, Alignment alignment = Alignment.TopLeft, bool hasHover = false) : base(game)
    {
        _alignment = alignment;
        actionBox.Location -= (actionBox.Size.ToVector2() * EngineStatics.Aligner(alignment)).ToPoint();
        _actionBox = actionBox;
        
        if (hasHover)
            HoverDetector = new HoverDetector(game, actionBox, alignment){Enabled = Enabled};
        
        Image = texture;
        Visible = texture is not null;
        if (texture is not null)
            DrawOrder = texture.DrawOrder;

        UpdateActionBox(this, EventArgs.Empty);
        EngineStatics.ViewportChanged += UpdateActionBox;
        Input.ButtonDown += Check;
    }

    #region Methods
    public override void Draw(GameTime gameTime) 
    {
        if (Image.Visible) Image.Draw(gameTime);
    }

    private void Check(object s, ButtonEventArgs e)
    {
        if (!Enabled || !_actionBox.Contains(e.Position))
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

    private void UpdateActionBox(object s, EventArgs e)
    {
        _actionBox = new Rectangle(
            (_actionBox.Size.ToVector2() * EngineStatics.Scale + EngineStatics.Offset).ToPoint(),
            (_actionBox.Size.ToVector2() * EngineStatics.Scale).ToPoint()
        );
    }

    protected override void Dispose(bool disposing)
    {
        Input.ButtonDown -= Check;
        EngineStatics.ViewportChanged -= UpdateActionBox;
        HoverDetector?.Dispose();
        base.Dispose(disposing);
    }
    #endregion
}