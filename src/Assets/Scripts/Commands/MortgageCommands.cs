using System;
using MonopolyFrenzy.Core;
using MonopolyFrenzy.Events;
using MonopolyFrenzy.Rules;
using Newtonsoft.Json;

namespace MonopolyFrenzy.Commands
{
    /// <summary>
    /// Command to mortgage a property.
    /// </summary>
    public class MortgageCommand : ICommand
    {
        private readonly GameState _gameState;
        private readonly Player _player;
        private readonly Property _property;
        private readonly EventBus _eventBus;
        private bool _wasMortgaged;
        
        /// <summary>
        /// Initializes a new instance of the MortgageCommand class.
        /// </summary>
        /// <param name="gameState">The current game state.</param>
        /// <param name="player">The player mortgaging the property.</param>
        /// <param name="property">The property to mortgage.</param>
        /// <param name="eventBus">The event bus for publishing events.</param>
        public MortgageCommand(GameState gameState, Player player, Property property, EventBus eventBus = null)
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
                var rules = new PropertyRules(_gameState);
                var (canMortgage, error) = rules.CanMortgageProperty(_player, _property);
                
                if (!canMortgage)
                    return CommandResult.Failed(error);
                
                // Store previous state
                _wasMortgaged = _property.IsMortgaged;
                
                // Mortgage property
                _property.IsMortgaged = true;
                _player.AddMoney(_property.MortgageValue);
                
                // Publish property mortgaged event
                _eventBus?.Publish(new PropertyMortgagedEvent
                {
                    PlayerId = _player.Id,
                    PropertyName = _property.Name,
                    MortgageValue = _property.MortgageValue
                });
                
                return CommandResult.Successful(new
                {
                    PropertyName = _property.Name,
                    MortgageValue = _property.MortgageValue,
                    PlayerMoney = _player.Money
                });
            }
            catch (Exception ex)
            {
                return CommandResult.Failed($"Failed to mortgage property: {ex.Message}");
            }
        }
        
        public void Undo()
        {
            if (_property == null)
                return;
            
            // Restore previous state
            _property.IsMortgaged = _wasMortgaged;
            _player.RemoveMoney(_property.MortgageValue);
        }
        
        public string SerializeToJson()
        {
            return JsonConvert.SerializeObject(new
            {
                Type = "Mortgage",
                PlayerId = _player.Id,
                PropertyIndex = _property.Index,
                PropertyName = _property.Name,
                MortgageValue = _property.MortgageValue
            });
        }
    }
    
    /// <summary>
    /// Command to unmortgage a property.
    /// </summary>
    public class UnmortgageCommand : ICommand
    {
        private readonly GameState _gameState;
        private readonly Player _player;
        private readonly Property _property;
        private readonly EventBus _eventBus;
        private bool _wasMortgaged;
        private int _costPaid;
        
        /// <summary>
        /// Initializes a new instance of the UnmortgageCommand class.
        /// </summary>
        /// <param name="gameState">The current game state.</param>
        /// <param name="player">The player unmortgaging the property.</param>
        /// <param name="property">The property to unmortgage.</param>
        /// <param name="eventBus">The event bus for publishing events.</param>
        public UnmortgageCommand(GameState gameState, Player player, Property property, EventBus eventBus = null)
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
                var rules = new PropertyRules(_gameState);
                var (canUnmortgage, error) = rules.CanUnmortgageProperty(_player, _property);
                
                if (!canUnmortgage)
                    return CommandResult.Failed(error);
                
                // Store previous state
                _wasMortgaged = _property.IsMortgaged;
                
                // Calculate cost (mortgage value + 10%)
                _costPaid = (int)(_property.MortgageValue * 1.1);
                
                // Unmortgage property
                _property.IsMortgaged = false;
                _player.RemoveMoney(_costPaid);
                
                // Publish property unmortgaged event
                _eventBus?.Publish(new PropertyUnmortgagedEvent
                {
                    PlayerId = _player.Id,
                    PropertyName = _property.Name,
                    Cost = _costPaid
                });
                
                return CommandResult.Successful(new
                {
                    PropertyName = _property.Name,
                    Cost = _costPaid,
                    PlayerMoney = _player.Money
                });
            }
            catch (Exception ex)
            {
                return CommandResult.Failed($"Failed to unmortgage property: {ex.Message}");
            }
        }
        
        public void Undo()
        {
            if (_property == null)
                return;
            
            // Restore previous state
            _property.IsMortgaged = _wasMortgaged;
            _player.AddMoney(_costPaid);
        }
        
        public string SerializeToJson()
        {
            return JsonConvert.SerializeObject(new
            {
                Type = "Unmortgage",
                PlayerId = _player.Id,
                PropertyIndex = _property.Index,
                PropertyName = _property.Name,
                Cost = _costPaid
            });
        }
    }
}
