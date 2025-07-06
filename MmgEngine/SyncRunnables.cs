using System;
using System.Collections.Generic;
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
        End();
        action();
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

public class LoopedAction(Game game, Action<ulong, TimeSpan> action, Func<ulong, TimeSpan, bool>? condition = null, Action? callback = null) : GameComponent(game), ISyncRunnable
{
    private ulong _frames;
    private TimeSpan _elapsedTime = TimeSpan.Zero;

    public LoopedAction(Game game, Action<ulong, TimeSpan> action, ulong frames, Action? callback = null)
        : this(game, action, (f, _) => f <= frames, callback) {}

    public LoopedAction(Game game, Action<ulong, TimeSpan> action, TimeSpan time, Action? callback = null)
        : this(game, action, (_, t) => t <= time, callback) {}
    
    public override void Update(GameTime gameTime)
    {
        ++_frames;
        _elapsedTime += gameTime.ElapsedGameTime;
        if (condition is not null && !condition(_frames, _elapsedTime))
        {
            Run();
            return;
        }
        
        action(_frames, _elapsedTime);
        base.Update(gameTime);
    }

    public void End()
    {
        Game.Components.Remove(this);
        _frames = 0;
        _elapsedTime = TimeSpan.Zero;
    }

    public void Run()
    {
        End();
        callback?.Invoke();
    }
}

public abstract class YieldingAction<T>(Game game, IEnumerator<T> action) : GameComponent(game), ISyncRunnable
{
    protected readonly IEnumerator<T> Enumerator = action;
    
    public void End()
    {
        Game.Components.Remove(this);
        Enumerator.Reset();
    }

    public abstract void Run();
}

public class TimeYieldingAction(Game game, IEnumerator<TimeSpan> action) : YieldingAction<TimeSpan>(game, action)
{
    private TimeSpan _delay = TimeSpan.Zero;

    public override void Update(GameTime gameTime)
    {
        if (_delay > TimeSpan.Zero)
        {
            _delay -= gameTime.ElapsedGameTime;
            return;
        }

        if (!Enumerator.MoveNext())
        {
            End();
            return;
        }

        _delay = Enumerator.Current;
    }

    public override void Run() => _delay = TimeSpan.Zero;
}

public class FrameYieldingAction(Game game, IEnumerator<ulong> action) : YieldingAction<ulong>(game, action)
{
    private ulong _delay;

    public override void Update(GameTime gameTime)
    {
        if (_delay-- > 0)
            return;

        if (!Enumerator.MoveNext())
        {
            End();
            return;
        }
        
        _delay = Enumerator.Current;
    }
    
    public override void Run() => _delay = 0;
}