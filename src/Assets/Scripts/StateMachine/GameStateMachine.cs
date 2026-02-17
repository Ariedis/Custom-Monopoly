using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MonopolyFrenzy.StateMachine
{
    /// <summary>
    /// State machine for managing game flow and turn progression.
    /// Implements the State Pattern for clean game flow control.
    /// </summary>
    public class GameStateMachine
    {
        private readonly Dictionary<Type, IGameState> _states;
        private readonly IGameContext _context;
        
        /// <summary>
        /// Gets the current active state.
        /// </summary>
        public IGameState CurrentState { get; private set; }
        
        /// <summary>
        /// Event fired when exiting a state.
        /// </summary>
        public event Action<IGameState> OnStateExit;
        
        /// <summary>
        /// Event fired when entering a state.
        /// </summary>
        public event Action<IGameState> OnStateEnter;
        
        /// <summary>
        /// Event fired when a state transition is logged.
        /// </summary>
        public event Action<IGameState, IGameState> OnTransitionLogged;
        
        /// <summary>
        /// Initializes a new instance of the GameStateMachine class.
        /// </summary>
        /// <param name="context">The game context.</param>
        public GameStateMachine(IGameContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _states = new Dictionary<Type, IGameState>();
            
            // Initialize with MainMenuState as default
            var mainMenuState = new MainMenuState(_context);
            _states[typeof(MainMenuState)] = mainMenuState;
            CurrentState = mainMenuState;
        }
        
        /// <summary>
        /// Initializes the state machine with all game states.
        /// </summary>
        public void Initialize()
        {
            _states.Clear();
            
            // Register all game flow states
            _states[typeof(MainMenuState)] = new MainMenuState(_context);
            _states[typeof(GameSetupState)] = new GameSetupState(_context);
            _states[typeof(PlayingState)] = new PlayingState(_context);
            _states[typeof(GameOverState)] = new GameOverState(_context);
            
            // Set initial state
            CurrentState = _states[typeof(MainMenuState)];
        }
        
        /// <summary>
        /// Transitions to a new state.
        /// </summary>
        /// <typeparam name="T">The type of state to transition to.</typeparam>
        /// <exception cref="InvalidOperationException">Thrown if the state type is not registered.</exception>
        public void TransitionTo<T>() where T : IGameState
        {
            var newStateType = typeof(T);
            
            if (!_states.ContainsKey(newStateType))
            {
                throw new InvalidOperationException($"State {newStateType.Name} is not registered in the state machine");
            }
            
            var newState = _states[newStateType];
            
            // Exit current state
            if (CurrentState != null)
            {
                CurrentState.Exit();
                OnStateExit?.Invoke(CurrentState);
            }
            
            // Log transition
            OnTransitionLogged?.Invoke(CurrentState, newState);
            
            // Enter new state
            var previousState = CurrentState;
            CurrentState = newState;
            CurrentState.Enter();
            OnStateEnter?.Invoke(CurrentState);
        }
        
        /// <summary>
        /// Updates the current state.
        /// </summary>
        public void Update()
        {
            CurrentState?.Update();
        }
        
        /// <summary>
        /// Gets a state of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of state to get.</typeparam>
        /// <returns>The state instance, or null if not found.</returns>
        public T GetState<T>() where T : IGameState
        {
            var stateType = typeof(T);
            
            if (_states.ContainsKey(stateType))
            {
                return (T)_states[stateType];
            }
            
            return default(T);
        }
        
        /// <summary>
        /// Checks if a transition to the specified state type is valid.
        /// </summary>
        /// <typeparam name="T">The state type to check.</typeparam>
        /// <returns>True if the transition is valid, false otherwise.</returns>
        public bool CanTransitionTo<T>() where T : IGameState
        {
            var targetStateType = typeof(T);
            var currentStateType = CurrentState?.GetType();
            
            // Define valid transitions
            if (currentStateType == typeof(MainMenuState))
            {
                return targetStateType == typeof(GameSetupState);
            }
            else if (currentStateType == typeof(GameSetupState))
            {
                return targetStateType == typeof(PlayingState) || 
                       targetStateType == typeof(MainMenuState);
            }
            else if (currentStateType == typeof(PlayingState))
            {
                return targetStateType == typeof(GameOverState) ||
                       targetStateType == typeof(MainMenuState);
            }
            else if (currentStateType == typeof(GameOverState))
            {
                return targetStateType == typeof(MainMenuState);
            }
            
            return false;
        }
    }
}
