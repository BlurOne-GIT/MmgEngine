using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MmgEngine;

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
        Window.KeyDown += Input.StoreKeyDown;
        Window.KeyUp += Input.StoreKeyUp;

        Activated += EngineStatics.Focus;
        Deactivated += EngineStatics.UnFocus;
        EngineStatics.ViewportChanged += OnViewportChanged;
        EngineStatics.Focus(null, null);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        Services.AddService(SpriteBatch);
        
        // TODO: use this.Content to load your game content here
    }

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
    {
        ViewportMatrix = Matrix.CreateScale(EngineStatics.Scale.X, EngineStatics.Scale.Y, 1) *
                         Matrix.CreateTranslation(EngineStatics.Offset.X, EngineStatics.Offset.Y, 0);
    }
}
