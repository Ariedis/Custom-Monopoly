using System;
using MonopolyFrenzy.Core;
using MonopolyFrenzy.Events;
using Newtonsoft.Json;

namespace MonopolyFrenzy.Commands
{
    /// <summary>
    /// Command to purchase a property.
    /// </summary>
    public class BuyPropertyCommand : ICommand
    {
        private readonly GameState _gameState;
        private readonly Player _player;
        private readonly Property _property;
        private readonly EventBus _eventBus;
        private Player _previousOwner;
        
        /// <summary>
        /// Gets the property being purchased.
        /// </summary>
        public Property Property => _property;
        
        /// <summary>
        /// Initializes a new instance of the BuyPropertyCommand class.
        /// </summary>
        /// <param name="gameState">The current game state.</param>
        /// <param name="player">The player purchasing the property.</param>
        /// <param name="property">The property to purchase.</param>
        /// <param name="eventBus">The event bus for publishing events.</param>
        public BuyPropertyCommand(GameState gameState, Player player, Property property, EventBus eventBus = null)
        {
            _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _property = property ?? throw new ArgumentNullException(nameof(property));
            _eventBus = eventBus;
        }
        
        public CommandResult Execute()
        {
            try
            {
                // Validate preconditions
                if (_property.Owner != null)
                    return CommandResult.Failed("Property is already owned");
                
                if (_player.Money < _property.PurchasePrice)
                    return CommandResult.Failed($"Insufficient funds. Need ${_property.PurchasePrice}, have ${_player.Money}");
                
                if (_property.IsMortgaged)
                    return CommandResult.Failed("Cannot purchase mortgaged property");
                
                // Store previous state for undo
                _previousOwner = _property.Owner;
                
                // Execute transaction
                _player.RemoveMoney(_property.PurchasePrice);
                _property.Owner = _player;
                _player.OwnedProperties.Add(_property);
                
                // Publish property purchased event
                _eventBus?.Publish(new PropertyPurchasedEvent
                {
                    PlayerId = _player.Id,
                    PropertyName = _property.Name,
                    Price = _property.PurchasePrice,
                    PlayerMoneyRemaining = _player.Money
                });
                
                return CommandResult.Successful(new
                {
                    PropertyName = _property.Name,
                    Price = _property.PurchasePrice,
                    PlayerMoney = _player.Money
                });
            }
            catch (Exception ex)
            {
                return CommandResult.Failed($"Failed to purchase property: {ex.Message}");
            }
        }
        
        public void Undo()
        {
            if (_property == null || _player == null)
                return;
            
            // Restore property ownership
            _property.Owner = _previousOwner;
            _player.OwnedProperties.Remove(_property);
            
            // Refund money
            _player.AddMoney(_property.PurchasePrice);
        }
        
        public string SerializeToJson()
        {
            return JsonConvert.SerializeObject(new
            {
                Type = "BuyProperty",
                PlayerId = _player.Id,
                PropertyIndex = _property.Index,
                PropertyName = _property.Name,
                Price = _property.PurchasePrice
            });
        }
    }
}
