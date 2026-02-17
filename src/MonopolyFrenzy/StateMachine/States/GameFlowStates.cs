using System;

namespace MonopolyFrenzy.StateMachine
{
    /// <summary>
    /// Main menu state - initial state when game starts.
    /// </summary>
    public class MainMenuState : IGameState
    {
        private readonly IGameContext _context;
        
        public MainMenuState(IGameContext context)
        {
            _context = context;
        }
        
        public void Enter()
        {
            // Initialize or reset to main menu
        }
        
        public void Update()
        {
            // Handle main menu input/logic
        }
        
        public void Exit()
        {
            // Clean up main menu
        }
    }
    
    /// <summary>
    /// Game setup state - configuring game settings and adding players.
    /// </summary>
    public class GameSetupState : IGameState
    {
        private readonly IGameContext _context;
        
        public GameSetupState(IGameContext context)
        {
            _context = context;
        }
        
        public void Enter()
        {
            // Initialize game setup
            _context.GameState?.Initialize();
        }
        
        public void Update()
        {
            // Handle game setup logic
        }
        
        public void Exit()
        {
            // Finalize game setup
        }
    }
    
    /// <summary>
    /// Playing state - active gameplay in progress.
    /// </summary>
    public class PlayingState : IGameState
    {
        private readonly IGameContext _context;
        
        public PlayingState(IGameContext context)
        {
            _context = context;
        }
        
        public void Enter()
        {
            // Start gameplay
            if (_context.GameState != null && _context.GameState.CanStartGame)
            {
                _context.GameState.StartGame();
            }
        }
        
        public void Update()
        {
            // Handle turn logic and game progression
        }
        
        public void Exit()
        {
            // Clean up active gameplay
        }
    }
    
    /// <summary>
    /// Game over state - game has ended, showing results.
    /// </summary>
    public class GameOverState : IGameState
    {
        private readonly IGameContext _context;
        
        public GameOverState(IGameContext context)
        {
            _context = context;
        }
        
        public void Enter()
        {
            // Show game over screen
            _context.GameState?.EndGame();
        }
        
        public void Update()
        {
            // Handle game over screen logic
        }
        
        public void Exit()
        {
            // Clean up game over state
        }
    }
}
