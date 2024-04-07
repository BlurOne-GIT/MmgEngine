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
        Components.ComponentAdded += OnComponentAdded;
        Components.ComponentRemoved += OnComponentRemoved;
    }

    protected override void Dispose(bool disposing)
    {
        Game.Window.KeyDown -= HandleInput;
        Input.ButtonDown -= HandleInput;
        Components.ComponentAdded -= OnComponentAdded;
        Components.Clear();
        Components.ComponentRemoved -= OnComponentRemoved;
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

    private void OnComponentAdded(object s, GameComponentCollectionEventArgs e) => Game.Components.Add(e.GameComponent);

    private void OnComponentRemoved(object s, GameComponentCollectionEventArgs e)
    {
        Game.Components.Remove(e.GameComponent);
        if (e.GameComponent is IDisposable disposable)
            disposable.Dispose();
    }
}