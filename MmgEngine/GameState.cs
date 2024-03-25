using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MmgEngine;

public abstract class GameState : DrawableGameComponent
{
    public GameState(Game game) : base(game)
    {
        Input.KeyDown += HandleInput;
        Input.ButtonDown += HandleInput;
    }
    public new virtual void Dispose()
    {
        Input.KeyDown -= HandleInput;
        Input.ButtonDown -= HandleInput;
        foreach (GameComponent gameObject in Components)
            gameObject.Dispose();

        base.Dispose();
    }
    protected readonly GameComponentCollection Components = new();
    public virtual void HandleInput(object s, ButtonEventArgs e) {}
    public virtual void HandleInput(object s, InputKeyEventArgs e) {}
    public event EventHandler<GameState> OnStateSwitched;
    public new abstract void LoadContent();
    public new abstract void UnloadContent();
    protected void SwitchState(GameState gameState)
    {
        OnStateSwitched?.Invoke(this, gameState);
    }
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
 }