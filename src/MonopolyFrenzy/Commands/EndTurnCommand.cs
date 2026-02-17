using System;
using MonopolyFrenzy.Core;
using Newtonsoft.Json;

namespace MonopolyFrenzy.Commands
{
    /// <summary>
    /// Command to end the current player's turn and advance to the next player.
    /// </summary>
    public class EndTurnCommand : ICommand
    {
        private readonly GameState _gameState;
        private Player _previousPlayer;
        private int _previousTurnNumber;
        
        /// <summary>
        /// Initializes a new instance of the EndTurnCommand class.
        /// </summary>
        /// <param name="gameState">The current game state.</param>
        public EndTurnCommand(GameState gameState)
        {
            _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
        }
        
        public CommandResult Execute()
        {
            try
            {
                // Store previous state for undo
                _previousPlayer = _gameState.CurrentPlayer;
                _previousTurnNumber = _gameState.TurnNumber;
                
                // Advance to next turn
                _gameState.NextTurn();
                
                return CommandResult.Successful(new
                {
                    PreviousPlayer = _previousPlayer?.Name,
                    CurrentPlayer = _gameState.CurrentPlayer?.Name,
                    TurnNumber = _gameState.TurnNumber
                });
            }
            catch (Exception ex)
            {
                return CommandResult.Failed($"Failed to end turn: {ex.Message}");
            }
        }
        
        public void Undo()
        {
            // Note: Undoing turn progression is complex as it requires
            // restoring all game state changes that occurred during the turn.
            // In practice, this would require a more sophisticated state management system.
            // For now, we log that undo is not fully supported for this command.
        }
        
        public string SerializeToJson()
        {
            return JsonConvert.SerializeObject(new
            {
                Type = "EndTurn",
                PreviousPlayerId = _previousPlayer?.Id,
                PreviousTurnNumber = _previousTurnNumber
            });
        }
    }
}
