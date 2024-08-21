using System;
using Microsoft.Xna.Framework;

namespace MmgEngine;

public class SwitchingGameStateEventArgs<TGameState>(TGameState? oldGameState, TGameState? newGameState) : EventArgs
    where TGameState : GameState
{
    public TGameState? OldGameState { get; } = oldGameState;
    public TGameState? NewGameState { get; } = newGameState;
}

public class GameStateManager(GameComponentCollection gameComponentCollection)
    : GameStateManager<GameState>(gameComponentCollection);

public class GameStateManager<TGameState>(GameComponentCollection gameComponentCollection)
    where TGameState : GameState
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
                gameComponentCollection.Remove(_gameState);
                _gameState.OnStateSwitched -= OnStateSwitched;
                _gameState.Dispose();
            }
            
            _gameState = value;
            
            if (_gameState is null) return;
            
            gameComponentCollection.Add(_gameState);
            
            _gameState.OnStateSwitched += OnStateSwitched;
        }
    }

    private TGameState? _gameState;

    private void OnStateSwitched(object? s, GameState e) => GameState = (TGameState)e;
}