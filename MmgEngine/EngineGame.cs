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
    protected GameStateManager GameStateManager { get; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public EngineGame()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        GameStateManager = new GameStateManager(Components);
    }

    protected override void Initialize()
    {
        var mouseHelper = new MouseHelper(this, false);
        Components.Add(mouseHelper);
        Services.AddService(mouseHelper);
        
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
    
    private void OnViewportChanged(object? s, EventArgs e)
        => ViewportMatrix = Matrix.CreateScale(EngineStatics.Scale.X, EngineStatics.Scale.Y, 1) * 
                            Matrix.CreateTranslation(EngineStatics.Offset.X, EngineStatics.Offset.Y, 0);
}
