using System;
using Microsoft.Xna.Framework;

namespace MmgEngine;

public interface ISyncRunnable
{
    public void Cancel() => End();
    protected void End();
    public void RunNow() => Run();
    protected void Run();
}

public abstract class DelayedAction(Game game, Action action) : GameComponent(game), ISyncRunnable
{
    public virtual void End() => Game.Components.Remove(this);

    public void Run()
    {
        action();
        End();
    }
}

public class TimeDelayedAction : DelayedAction
{
    private TimeSpan _delay;
    private readonly TimeSpan _originalDelay;

    public TimeDelayedAction(Game game, TimeSpan delay, Action action) : base(game, action) => _delay = _originalDelay = delay;

    public override void Update(GameTime gameTime)
    {
        _delay -= gameTime.ElapsedGameTime;
        if (_delay <= TimeSpan.Zero)
            Run();
    }

    public override void End()
    {
        base.End();
        _delay = _originalDelay;
    }
}

public class FrameDelayedAction : DelayedAction
{
    private ulong _delay;
    private readonly ulong _originalDelay;

    public FrameDelayedAction(Game game, ulong delay, Action action) : base(game, action) => _delay = _originalDelay = delay;

    public override void Update(GameTime gameTime)
    {
        _delay--;
        if (_delay is 0)
            Run();
    }

    public override void End()
    {
        base.End();
        _delay = _originalDelay;
    }
}