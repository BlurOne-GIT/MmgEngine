using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MmgEngine;

public abstract class GameState : DrawableGameComponent
{
    public GameState(Game game) : base(game)
    {
        Game.Window.KeyDown += HandleInput;
        Input.ButtonDown += HandleInput;
        Components.ComponentAdded += InitializeComponent;
        Components.ComponentRemoved += DisposeComponent;
    }

    protected override void Dispose(bool disposing)
    {
        Game.Window.KeyDown -= HandleInput;
        Input.ButtonDown -= HandleInput;
        Components.ComponentAdded -= InitializeComponent;
        Components.Clear();
        Components.ComponentRemoved -= DisposeComponent;
        base.Dispose(disposing);
    }
    
    protected readonly GameComponentCollection Components = new();
    
    /// <summary>
    /// Handle button clicks.
    /// </summary>
    public virtual void HandleInput(object sender, ButtonEventArgs eventArgs) {}
    
    /// <summary>
    /// Handle key presses.
    /// </summary>
    public virtual void HandleInput(object sender, InputKeyEventArgs eventArgs) {}
    
    public event EventHandler<GameState> OnStateSwitched;
    
    public new abstract void LoadContent();
    
    public new abstract void UnloadContent();
    
    /// <summary>
    /// Change the current game state.
    /// </summary>
    /// <param name="gameState">New game state.</param>
    protected void SwitchState(GameState gameState) => OnStateSwitched?.Invoke(this, gameState);

    public override void Update(GameTime gameTime)
    {
        foreach (GameComponent gameObject in Components.OrderBy(a => (a as GameComponent)!.UpdateOrder))
            if (gameObject.Enabled) gameObject.Update(gameTime);
    }
    
    public override void Draw(GameTime gameTime)
    {
        var c = Components.Where(a => a is DrawableGameComponent { Visible: true }).OrderBy(a => (a as DrawableGameComponent)!.DrawOrder);
        foreach (DrawableGameComponent gameObject in c)
            gameObject.Draw(gameTime);
    }

    private static void InitializeComponent(object s, GameComponentCollectionEventArgs e) => e.GameComponent.Initialize();

    private static void DisposeComponent(object s, GameComponentCollectionEventArgs e)
    {
        if (e.GameComponent is IDisposable disposable)
            disposable.Dispose();
    }
}