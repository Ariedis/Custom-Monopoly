using System;
using System.Collections.Generic;
using System.Linq;
using MonopolyFrenzy.Core;
using MonopolyFrenzy.Events;
using Newtonsoft.Json;

namespace MonopolyFrenzy.Commands
{
    /// <summary>
    /// Represents a trade offer between two players.
    /// </summary>
    public class TradeOffer
    {
        /// <summary>
        /// Gets or sets the list of properties being offered.
        /// </summary>
        public List<Property> Properties { get; set; }
        
        /// <summary>
        /// Gets or sets the amount of money being offered.
        /// </summary>
        public int Money { get; set; }
        
        /// <summary>
        /// Gets or sets the number of Get Out of Jail Free cards being offered.
        /// </summary>
        public int GetOutOfJailFreeCards { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the TradeOffer class.
        /// </summary>
        public TradeOffer()
        {
            Properties = new List<Property>();
            Money = 0;
            GetOutOfJailFreeCards = 0;
        }
    }
    
    /// <summary>
    /// Command to execute a trade between two players.
    /// </summary>
    public class TradeCommand : ICommand
    {
        private readonly GameState _gameState;
        private readonly Player _player1;
        private readonly Player _player2;
        private readonly TradeOffer _player1Offer;
        private readonly TradeOffer _player2Offer;
        private readonly EventBus _eventBus;
        private bool _wasExecuted;
        
        /// <summary>
        /// Gets the first player in the trade.
        /// </summary>
        public Player Player1 => _player1;
        
        /// <summary>
        /// Gets the second player in the trade.
        /// </summary>
        public Player Player2 => _player2;
        
        /// <summary>
        /// Gets the offer from player 1.
        /// </summary>
        public TradeOffer Player1Offer => _player1Offer;
        
        /// <summary>
        /// Gets the offer from player 2.
        /// </summary>
        public TradeOffer Player2Offer => _player2Offer;
        
        /// <summary>
        /// Initializes a new instance of the TradeCommand class.
        /// </summary>
        /// <param name="gameState">The current game state.</param>
        /// <param name="player1">The first player in the trade.</param>
        /// <param name="player2">The second player in the trade.</param>
        /// <param name="player1Offer">What player 1 is offering.</param>
        /// <param name="player2Offer">What player 2 is offering.</param>
        /// <param name="eventBus">The event bus for publishing events.</param>
        public TradeCommand(
            GameState gameState,
            Player player1,
            Player player2,
            TradeOffer player1Offer,
            TradeOffer player2Offer,
            EventBus eventBus = null)
        {
            _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
            _player1 = player1 ?? throw new ArgumentNullException(nameof(player1));
            _player2 = player2 ?? throw new ArgumentNullException(nameof(player2));
            _player1Offer = player1Offer ?? throw new ArgumentNullException(nameof(player1Offer));
            _player2Offer = player2Offer ?? throw new ArgumentNullException(nameof(player2Offer));
            _eventBus = eventBus;
            _wasExecuted = false;
        }
        
        public CommandResult Execute()
        {
            try
            {
                // Validate trade
                var validationResult = ValidateTrade();
                if (!validationResult.Success)
                    return validationResult;
                
                // Execute the trade
                ExecuteTradeTransfer();
                
                _wasExecuted = true;
                
                // Publish trade event
                _eventBus?.Publish(new TradeExecutedEvent
                {
                    Player1Id = _player1.Id,
                    Player2Id = _player2.Id,
                    Player1Properties = _player1Offer.Properties.Select(p => p.Name).ToArray(),
                    Player2Properties = _player2Offer.Properties.Select(p => p.Name).ToArray(),
                    Player1Money = _player1Offer.Money,
                    Player2Money = _player2Offer.Money
                });
                
                return CommandResult.Successful(new
                {
                    Player1Traded = new
                    {
                        Properties = _player1Offer.Properties.Select(p => p.Name).ToArray(),
                        Money = _player1Offer.Money,
                        GetOutOfJailFreeCards = _player1Offer.GetOutOfJailFreeCards
                    },
                    Player2Traded = new
                    {
                        Properties = _player2Offer.Properties.Select(p => p.Name).ToArray(),
                        Money = _player2Offer.Money,
                        GetOutOfJailFreeCards = _player2Offer.GetOutOfJailFreeCards
                    }
                });
            }
            catch (Exception ex)
            {
                return CommandResult.Failed($"Failed to execute trade: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Validates that the trade can be executed.
        /// </summary>
        /// <returns>Validation result.</returns>
        private CommandResult ValidateTrade()
        {
            // Cannot trade with yourself
            if (_player1.Id == _player2.Id)
                return CommandResult.Failed("Cannot trade with yourself");
            
            // Both players must be active
            if (_player1.IsBankrupt)
                return CommandResult.Failed($"{_player1.Name} is bankrupt");
            
            if (_player2.IsBankrupt)
                return CommandResult.Failed($"{_player2.Name} is bankrupt");
            
            // Validate player 1's offer
            var player1Validation = ValidatePlayerOffer(_player1, _player1Offer);
            if (!player1Validation.Success)
                return player1Validation;
            
            // Validate player 2's offer
            var player2Validation = ValidatePlayerOffer(_player2, _player2Offer);
            if (!player2Validation.Success)
                return player2Validation;
            
            // At least one side must offer something
            if (IsEmptyOffer(_player1Offer) && IsEmptyOffer(_player2Offer))
                return CommandResult.Failed("Trade must include at least one item");
            
            return CommandResult.Successful();
        }
        
        /// <summary>
        /// Validates that a player can offer what they're offering.
        /// </summary>
        /// <param name="player">The player making the offer.</param>
        /// <param name="offer">The offer to validate.</param>
        /// <returns>Validation result.</returns>
        private CommandResult ValidatePlayerOffer(Player player, TradeOffer offer)
        {
            // Validate money
            if (offer.Money < 0)
                return CommandResult.Failed($"{player.Name} cannot offer negative money");
            
            if (offer.Money > player.Money)
                return CommandResult.Failed($"{player.Name} doesn't have ${offer.Money}");
            
            // Validate Get Out of Jail Free cards
            if (offer.GetOutOfJailFreeCards < 0)
                return CommandResult.Failed($"{player.Name} cannot offer negative cards");
            
            if (offer.GetOutOfJailFreeCards > player.GetOutOfJailFreeCards)
                return CommandResult.Failed($"{player.Name} doesn't have {offer.GetOutOfJailFreeCards} Get Out of Jail Free cards");
            
            // Validate properties
            foreach (var property in offer.Properties)
            {
                if (property == null)
                    return CommandResult.Failed("Cannot trade null property");
                
                if (property.Owner != player)
                    return CommandResult.Failed($"{player.Name} doesn't own {property.Name}");
                
                if (property.IsMortgaged)
                    return CommandResult.Failed($"Cannot trade mortgaged property: {property.Name}");
                
                if (property.Houses > 0 || property.HasHotel)
                    return CommandResult.Failed($"Cannot trade {property.Name} - must sell buildings first");
            }
            
            return CommandResult.Successful();
        }
        
        /// <summary>
        /// Checks if an offer is empty.
        /// </summary>
        /// <param name="offer">The offer to check.</param>
        /// <returns>True if the offer contains nothing.</returns>
        private bool IsEmptyOffer(TradeOffer offer)
        {
            return offer.Properties.Count == 0 &&
                   offer.Money == 0 &&
                   offer.GetOutOfJailFreeCards == 0;
        }
        
        /// <summary>
        /// Executes the actual transfer of assets between players.
        /// </summary>
        private void ExecuteTradeTransfer()
        {
            // Transfer properties from player 1 to player 2
            foreach (var property in _player1Offer.Properties)
            {
                _player1.OwnedProperties.Remove(property);
                property.Owner = _player2;
                _player2.OwnedProperties.Add(property);
            }
            
            // Transfer properties from player 2 to player 1
            foreach (var property in _player2Offer.Properties)
            {
                _player2.OwnedProperties.Remove(property);
                property.Owner = _player1;
                _player1.OwnedProperties.Add(property);
            }
            
            // Transfer money (player 1 gives money to player 2)
            if (_player1Offer.Money > 0)
            {
                _player1.RemoveMoney(_player1Offer.Money);
                _player2.AddMoney(_player1Offer.Money);
            }
            
            // Transfer money (player 2 gives money to player 1)
            if (_player2Offer.Money > 0)
            {
                _player2.RemoveMoney(_player2Offer.Money);
                _player1.AddMoney(_player2Offer.Money);
            }
            
            // Transfer Get Out of Jail Free cards
            if (_player1Offer.GetOutOfJailFreeCards > 0)
            {
                _player1.GetOutOfJailFreeCards -= _player1Offer.GetOutOfJailFreeCards;
                _player2.GetOutOfJailFreeCards += _player1Offer.GetOutOfJailFreeCards;
            }
            
            if (_player2Offer.GetOutOfJailFreeCards > 0)
            {
                _player2.GetOutOfJailFreeCards -= _player2Offer.GetOutOfJailFreeCards;
                _player1.GetOutOfJailFreeCards += _player2Offer.GetOutOfJailFreeCards;
            }
        }
        
        public void Undo()
        {
            if (!_wasExecuted)
                return;
            
            // Reverse the trade by swapping the offers and executing again
            // Transfer properties back from player 2 to player 1
            foreach (var property in _player1Offer.Properties)
            {
                _player2.OwnedProperties.Remove(property);
                property.Owner = _player1;
                _player1.OwnedProperties.Add(property);
            }
            
            // Transfer properties back from player 1 to player 2
            foreach (var property in _player2Offer.Properties)
            {
                _player1.OwnedProperties.Remove(property);
                property.Owner = _player2;
                _player2.OwnedProperties.Add(property);
            }
            
            // Reverse money transfers
            if (_player1Offer.Money > 0)
            {
                _player2.RemoveMoney(_player1Offer.Money);
                _player1.AddMoney(_player1Offer.Money);
            }
            
            if (_player2Offer.Money > 0)
            {
                _player1.RemoveMoney(_player2Offer.Money);
                _player2.AddMoney(_player2Offer.Money);
            }
            
            // Reverse Get Out of Jail Free card transfers
            if (_player1Offer.GetOutOfJailFreeCards > 0)
            {
                _player2.GetOutOfJailFreeCards -= _player1Offer.GetOutOfJailFreeCards;
                _player1.GetOutOfJailFreeCards += _player1Offer.GetOutOfJailFreeCards;
            }
            
            if (_player2Offer.GetOutOfJailFreeCards > 0)
            {
                _player1.GetOutOfJailFreeCards -= _player2Offer.GetOutOfJailFreeCards;
                _player2.GetOutOfJailFreeCards += _player2Offer.GetOutOfJailFreeCards;
            }
            
            _wasExecuted = false;
        }
        
        public string SerializeToJson()
        {
            return JsonConvert.SerializeObject(new
            {
                Command = "Trade",
                Player1Id = _player1.Id,
                Player2Id = _player2.Id,
                Player1Offer = new
                {
                    PropertyNames = _player1Offer.Properties.Select(p => p.Name).ToArray(),
                    Money = _player1Offer.Money,
                    GetOutOfJailFreeCards = _player1Offer.GetOutOfJailFreeCards
                },
                Player2Offer = new
                {
                    PropertyNames = _player2Offer.Properties.Select(p => p.Name).ToArray(),
                    Money = _player2Offer.Money,
                    GetOutOfJailFreeCards = _player2Offer.GetOutOfJailFreeCards
                }
            });
        }
    }
}
