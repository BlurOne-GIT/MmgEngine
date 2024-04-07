using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MmgEngine;

/// <summary>
/// Abstract class that extends <see cref="Game"/> with the engine's features.
/// Inheritance is not required, but it is recommended to use the engine's features.
/// </summary>
public abstract class EngineGame : Game
{
    protected GraphicsDeviceManager Graphics;
    protected SpriteBatch SpriteBatch;
    protected Matrix ViewportMatrix;
    protected GameState CurrentGameState;

    public EngineGame()
    {
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
    }

    protected override void Initialize()
    {
        Input.Game = this;
        
        OnViewportChanged(this, EventArgs.Empty);
        EngineStatics.ViewportChanged += OnViewportChanged;
        base.Initialize();
    }

    protected override void LoadContent()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        Services.AddService(SpriteBatch);
        
        // TODO: use this.Content to load your game content here
    }

    /// <summary>
    /// Call this method to switch the current game state.
    /// Also called by <see cref="GameState.OnStateSwitched"/>.
    /// </summary>
    /// <param name="newGameState">New game state.</param>
    protected void SwitchGameState(GameState newGameState)
    {
        if (CurrentGameState is not null)
        {
            CurrentGameState.OnStateSwitched -= OnStateSwitched;
            CurrentGameState.UnloadContent();
            CurrentGameState.Dispose();
        }

        CurrentGameState = newGameState;

        CurrentGameState.Initialize();
        CurrentGameState.LoadContent();

        CurrentGameState.OnStateSwitched += OnStateSwitched;
    }

    private void OnStateSwitched(object s, GameState e) => SwitchGameState(e);
    
    private void OnViewportChanged(object s, EventArgs e)
        => ViewportMatrix = Matrix.CreateScale(EngineStatics.Scale.X, EngineStatics.Scale.Y, 1) * 
                            Matrix.CreateTranslation(EngineStatics.Offset.X, EngineStatics.Offset.Y, 0);
}
