using System;
using System.Collections;
using System.Collections.Generic;
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
    
    /// <summary>
    /// Change the current game state.
    /// </summary>
    /// <param name="gameState">New game state.</param>
    protected void SwitchState(GameState gameState) => OnStateSwitched?.Invoke(this, gameState);

    private void OnComponentAdded(object s, GameComponentCollectionEventArgs e)
    {
        if (!Enabled && e.GameComponent is GameComponent { Enabled: true } gameComponent)
        {
            _previousEnabled.Add(gameComponent);
            gameComponent.Enabled = false;
        }

        if (!Visible && e.GameComponent is DrawableGameComponent { Visible: true } drawable)
        {
            _previousVisible.Add(drawable);
            drawable.Visible = false;
        }
        
        Game.Components.Add(e.GameComponent);
    }

    private void OnComponentRemoved(object s, GameComponentCollectionEventArgs e)
    {
        Game.Components.Remove(e.GameComponent);
        if (e.GameComponent is IDisposable disposable)
            disposable.Dispose();
    }
    
    private readonly HashSet<GameComponent> _previousEnabled = new();
    private readonly HashSet<DrawableGameComponent> _previousVisible = new();
    
    protected override void OnEnabledChanged(object sender, EventArgs args)
    {
        if (Enabled)
        {
            foreach (var component in Components)
                if (component is GameComponent gameComponent && _previousEnabled.Remove(gameComponent))
                    gameComponent.Enabled = true;
        }
        else
        {
            foreach (var component in Components)
                if (component is GameComponent { Enabled: true } gameComponent)
                {
                    _previousEnabled.Add(gameComponent);
                    gameComponent.Enabled = false;
                }
        }
        
        base.OnEnabledChanged(sender, args);
    }

    protected override void OnVisibleChanged(object sender, EventArgs args)
    {
        if (Visible)
        {
            foreach (var component in Components)
                if (component is DrawableGameComponent drawable && _previousVisible.Remove(drawable))
                    drawable.Visible = true;
        }
        else
        {
            foreach (var component in Components)
                if (component is DrawableGameComponent { Visible: true } drawable)
                {
                    _previousVisible.Add(drawable);
                    drawable.Visible = false;
                }
        }
        
        
        base.OnVisibleChanged(sender, args);
    }
}