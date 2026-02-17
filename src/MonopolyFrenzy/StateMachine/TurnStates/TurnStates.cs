using System;

namespace MonopolyFrenzy.StateMachine
{
    /// <summary>
    /// Roll dice state - player rolls dice for their turn.
    /// </summary>
    public class RollDiceState : IGameState
    {
        private readonly IGameContext _context;
        
        public RollDiceState(IGameContext context)
        {
            _context = context;
        }
        
        public void Enter()
        {
            // Prepare for dice roll
        }
        
        public void Update()
        {
            // Wait for dice roll command
        }
        
        public void Exit()
        {
            // Dice rolled, prepare for movement
        }
    }
    
    /// <summary>
    /// Move piece state - player's piece moves based on dice roll.
    /// </summary>
    public class MovePieceState : IGameState
    {
        private readonly IGameContext _context;
        
        public MovePieceState(IGameContext context)
        {
            _context = context;
        }
        
        public void Enter()
        {
            // Begin movement animation/logic
        }
        
        public void Update()
        {
            // Handle movement
        }
        
        public void Exit()
        {
            // Movement complete
        }
    }
    
    /// <summary>
    /// Take turn action state - player performs actions at their landing space.
    /// </summary>
    public class TakeTurnActionState : IGameState
    {
        private readonly IGameContext _context;
        
        public TakeTurnActionState(IGameContext context)
        {
            _context = context;
        }
        
        public void Enter()
        {
            // Determine available actions at current space
        }
        
        public void Update()
        {
            // Handle turn actions (buy property, pay rent, draw card, etc.)
        }
        
        public void Exit()
        {
            // Actions complete
        }
    }
    
    /// <summary>
    /// End turn state - finalize turn and prepare for next player.
    /// </summary>
    public class EndTurnState : IGameState
    {
        private readonly IGameContext _context;
        
        public EndTurnState(IGameContext context)
        {
            _context = context;
        }
        
        public void Enter()
        {
            // Begin turn ending process
        }
        
        public void Update()
        {
            // Finalize turn
        }
        
        public void Exit()
        {
            // Ready for next player's turn
        }
    }
    
    /// <summary>
    /// Turn state machine for managing turn phases.
    /// </summary>
    public class TurnStateMachine
    {
        private readonly IGameContext _context;
        
        /// <summary>
        /// Gets the current turn state.
        /// </summary>
        public IGameState CurrentState { get; private set; }
        
        public TurnStateMachine(IGameContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        /// <summary>
        /// Initializes the turn state machine.
        /// </summary>
        public void Initialize()
        {
            // Start with roll dice state
            CurrentState = new RollDiceState(_context);
        }
        
        /// <summary>
        /// Transitions to a new turn state.
        /// </summary>
        /// <typeparam name="T">The type of state to transition to.</typeparam>
        public void TransitionTo<T>() where T : IGameState, new()
        {
            CurrentState?.Exit();
            CurrentState = Activator.CreateInstance(typeof(T), _context) as IGameState;
            CurrentState?.Enter();
        }
        
        /// <summary>
        /// Updates the current turn state.
        /// </summary>
        public void Update()
        {
            CurrentState?.Update();
        }
    }
}
