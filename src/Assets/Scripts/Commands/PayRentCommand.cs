using System;
using MonopolyFrenzy.Core;
using MonopolyFrenzy.Events;
using MonopolyFrenzy.Rules;
using Newtonsoft.Json;

namespace MonopolyFrenzy.Commands
{
    /// <summary>
    /// Command to pay rent when landing on an owned property.
    /// </summary>
    public class PayRentCommand : ICommand
    {
        private readonly GameState _gameState;
        private readonly Player _player;
        private readonly Space _space;
        private readonly int _diceRoll;
        private readonly EventBus _eventBus;
        private int _rentAmount;
        private Player _owner;
        
        /// <summary>
        /// Gets the rent amount that was paid.
        /// </summary>
        public int RentAmount => _rentAmount;
        
        /// <summary>
        /// Initializes a new instance of the PayRentCommand class.
        /// </summary>
        /// <param name="gameState">The current game state.</param>
        /// <param name="player">The player paying rent.</param>
        /// <param name="space">The space landed on.</param>
        /// <param name="diceRoll">The dice roll (for utilities).</param>
        /// <param name="eventBus">The event bus for publishing events.</param>
        public PayRentCommand(GameState gameState, Player player, Space space, int diceRoll = 0, EventBus eventBus = null)
        {
            _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _space = space ?? throw new ArgumentNullException(nameof(space));
            _diceRoll = diceRoll;
            _eventBus = eventBus;
        }
        
        public CommandResult Execute()
        {
            try
            {
                var rentCalculator = new RentCalculator(_gameState);
                
                // Calculate rent based on space type
                if (_space is Property property)
                {
                    if (property.Owner == null || property.Owner == _player)
                        return CommandResult.Successful(new { RentAmount = 0, Message = "No rent owed" });
                    
                    _owner = property.Owner;
                    _rentAmount = rentCalculator.CalculateRent(property, _diceRoll);
                }
                else if (_space is Railroad railroad)
                {
                    if (railroad.Owner == null || railroad.Owner == _player)
                        return CommandResult.Successful(new { RentAmount = 0, Message = "No rent owed" });
                    
                    _owner = railroad.Owner;
                    _rentAmount = rentCalculator.CalculateRailroadRent(railroad);
                }
                else if (_space is Utility utility)
                {
                    if (utility.Owner == null || utility.Owner == _player)
                        return CommandResult.Successful(new { RentAmount = 0, Message = "No rent owed" });
                    
                    _owner = utility.Owner;
                    _rentAmount = rentCalculator.CalculateUtilityRent(utility, _diceRoll);
                }
                else
                {
                    return CommandResult.Failed("Space does not require rent payment");
                }
                
                // Check if player can pay
                if (_player.Money < _rentAmount)
                {
                    return CommandResult.Failed($"Insufficient funds to pay rent of ${_rentAmount}. Player has ${_player.Money}. Bankruptcy required.");
                }
                
                // Transfer money
                _player.RemoveMoney(_rentAmount);
                _owner.AddMoney(_rentAmount);
                
                // Publish money transferred event
                _eventBus?.Publish(new MoneyTransferredEvent
                {
                    FromPlayerId = _player.Id,
                    ToPlayerId = _owner.Id,
                    Amount = _rentAmount,
                    Reason = $"Rent for {_space.Name}"
                });
                
                return CommandResult.Successful(new
                {
                    RentAmount = _rentAmount,
                    PayerMoney = _player.Money,
                    ReceiverMoney = _owner.Money,
                    SpaceName = _space.Name
                });
            }
            catch (Exception ex)
            {
                return CommandResult.Failed($"Failed to pay rent: {ex.Message}");
            }
        }
        
        public void Undo()
        {
            if (_owner == null || _rentAmount == 0)
                return;
            
            // Reverse the transaction
            _owner.RemoveMoney(_rentAmount);
            _player.AddMoney(_rentAmount);
        }
        
        public string SerializeToJson()
        {
            return JsonConvert.SerializeObject(new
            {
                Type = "PayRent",
                PlayerId = _player.Id,
                SpaceIndex = _space.Index,
                SpaceName = _space.Name,
                RentAmount = _rentAmount,
                OwnerId = _owner?.Id
            });
        }
    }
}
