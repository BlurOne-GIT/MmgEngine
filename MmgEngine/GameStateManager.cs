using System;
using Microsoft.Xna.Framework;

namespace MmgEngine;

public class SwitchingGameStateEventArgs<TGameState> : EventArgs where TGameState : GameState
{
    public SwitchingGameStateEventArgs(TGameState? oldGameState, TGameState? newGameState)
    {
        OldGameState = oldGameState;
        NewGameState = newGameState;
    }

    public TGameState? OldGameState { get; }
    public TGameState? NewGameState { get; }
}

public class GameStateManager<TGameState> where TGameState : GameState
{
    public event EventHandler<SwitchingGameStateEventArgs<TGameState>>? Switched;
    
    public TGameState? GameState
    {
        get => _gameState;
        set
        {
            Switched?.Invoke(this, new SwitchingGameStateEventArgs<TGameState>(_gameState, value));
            if (_gameState is not null)
            {
                _components.Remove(_gameState);
                _gameState.OnStateSwitched -= OnStateSwitched;
                _gameState.Dispose();
            }
            
            _gameState = value;
            
            if (_gameState is null) return;
            
            _components.Add(_gameState);
            
            _gameState.OnStateSwitched += OnStateSwitched;
        }
    }

    private readonly GameComponentCollection _components;
    private TGameState? _gameState;

    public GameStateManager(GameComponentCollection gameComponentCollection)
        => _components = gameComponentCollection;

    private void OnStateSwitched(object? s, GameState e) => GameState = (TGameState)e;
}