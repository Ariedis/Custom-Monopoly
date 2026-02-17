using System;
using MonopolyFrenzy.Core;
using Newtonsoft.Json;

namespace MonopolyFrenzy.Commands
{
    /// <summary>
    /// Command to move a player to a new position on the board.
    /// </summary>
    public class MoveCommand : ICommand
    {
        private readonly GameState _gameState;
        private readonly Player _player;
        private readonly int _spaces;
        private int _previousPosition;
        private bool _passedGo;
        
        /// <summary>
        /// Gets the number of spaces to move.
        /// </summary>
        public int Spaces => _spaces;
        
        /// <summary>
        /// Gets the new position after moving.
        /// </summary>
        public int NewPosition { get; private set; }
        
        /// <summary>
        /// Gets whether the player passed GO during this move.
        /// </summary>
        public bool PassedGo => _passedGo;
        
        /// <summary>
        /// Initializes a new instance of the MoveCommand class.
        /// </summary>
        /// <param name="gameState">The current game state.</param>
        /// <param name="player">The player to move.</param>
        /// <param name="spaces">The number of spaces to move (positive forward, negative backward).</param>
        public MoveCommand(GameState gameState, Player player, int spaces)
        {
            _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _spaces = spaces;
        }
        
        public CommandResult Execute()
        {
            try
            {
                _previousPosition = _player.Position;
                
                // Calculate new position
                int newPosition = _previousPosition + _spaces;
                
                // Check if player passed GO (only when moving forward)
                _passedGo = false;
                if (_spaces > 0 && newPosition >= 40)
                {
                    _passedGo = true;
                    _player.AddMoney(200); // Collect $200 for passing GO
                }
                
                // Wrap position to 0-39 range
                NewPosition = ((newPosition % 40) + 40) % 40;
                _player.Position = NewPosition;
                
                return CommandResult.Successful(new
                {
                    PreviousPosition = _previousPosition,
                    NewPosition = NewPosition,
                    PassedGo = _passedGo,
                    CollectedMoney = _passedGo ? 200 : 0
                });
            }
            catch (Exception ex)
            {
                return CommandResult.Failed($"Failed to move player: {ex.Message}");
            }
        }
        
        public void Undo()
        {
            if (_player == null)
                return;
            
            // Restore previous position
            _player.Position = _previousPosition;
            
            // Remove GO money if it was collected
            if (_passedGo)
            {
                _player.RemoveMoney(200);
            }
        }
        
        public string SerializeToJson()
        {
            return JsonConvert.SerializeObject(new
            {
                Type = "Move",
                PlayerId = _player.Id,
                Spaces = _spaces,
                PreviousPosition = _previousPosition,
                NewPosition = NewPosition,
                PassedGo = _passedGo
            });
        }
    }
}
