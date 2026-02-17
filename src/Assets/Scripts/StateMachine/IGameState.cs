using System;

namespace MonopolyFrenzy.StateMachine
{
    /// <summary>
    /// Interface for game states in the state machine.
    /// </summary>
    public interface IGameState
    {
        /// <summary>
        /// Called when entering this state.
        /// </summary>
        void Enter();
        
        /// <summary>
        /// Called each update cycle while in this state.
        /// </summary>
        void Update();
        
        /// <summary>
        /// Called when exiting this state.
        /// </summary>
        void Exit();
    }
    
    /// <summary>
    /// Context interface providing access to game data for states.
    /// </summary>
    public interface IGameContext
    {
        Core.GameState GameState { get; }
    }
}
