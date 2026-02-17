using System;
using MonopolyFrenzy.Core;
using Newtonsoft.Json;

namespace MonopolyFrenzy.Commands
{
    /// <summary>
    /// Command to roll dice and update game state.
    /// </summary>
    public class RollDiceCommand : ICommand
    {
        private readonly GameState _gameState;
        private readonly Player _player;
        private readonly Random _random;
        private int _die1;
        private int _die2;
        private int _previousPosition;
        
        /// <summary>
        /// Gets the first die result.
        /// </summary>
        public int Die1 => _die1;
        
        /// <summary>
        /// Gets the second die result.
        /// </summary>
        public int Die2 => _die2;
        
        /// <summary>
        /// Gets the total of both dice.
        /// </summary>
        public int Total => _die1 + _die2;
        
        /// <summary>
        /// Gets whether doubles were rolled.
        /// </summary>
        public bool IsDoubles => _die1 == _die2;
        
        /// <summary>
        /// Initializes a new instance of the RollDiceCommand class.
        /// </summary>
        /// <param name="gameState">The current game state.</param>
        /// <param name="player">The player rolling the dice.</param>
        /// <param name="random">Random number generator (optional, for testing).</param>
        public RollDiceCommand(GameState gameState, Player player, Random random = null)
        {
            _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _random = random ?? new Random();
        }
        
        /// <summary>
        /// Initializes with predetermined dice values (for testing).
        /// </summary>
        public RollDiceCommand(GameState gameState, Player player, int die1, int die2)
        {
            _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _die1 = die1;
            _die2 = die2;
            _random = null;
        }
        
        public CommandResult Execute()
        {
            try
            {
                // Roll dice if not predetermined
                if (_random != null)
                {
                    _die1 = _random.Next(1, 7);
                    _die2 = _random.Next(1, 7);
                }
                
                // Validate dice values
                if (_die1 < 1 || _die1 > 6 || _die2 < 1 || _die2 > 6)
                    return CommandResult.Failed("Invalid dice values");
                
                return CommandResult.Successful(new
                {
                    Die1 = _die1,
                    Die2 = _die2,
                    Total = Total,
                    IsDoubles = IsDoubles
                });
            }
            catch (Exception ex)
            {
                return CommandResult.Failed($"Failed to roll dice: {ex.Message}");
            }
        }
        
        public void Undo()
        {
            // Rolling dice cannot be undone in standard rules
            // The state changes from dice roll (movement, etc.) should be undone separately
        }
        
        public string SerializeToJson()
        {
            return JsonConvert.SerializeObject(new
            {
                Type = "RollDice",
                PlayerId = _player.Id,
                Die1 = _die1,
                Die2 = _die2
            });
        }
    }
}
