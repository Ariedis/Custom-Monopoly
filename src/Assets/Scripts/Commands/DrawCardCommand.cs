using System;
using System.Linq;
using MonopolyFrenzy.Core;
using MonopolyFrenzy.Events;
using MonopolyFrenzy.Rules;
using Newtonsoft.Json;

namespace MonopolyFrenzy.Commands
{
    /// <summary>
    /// Command to draw and execute a card from a deck.
    /// </summary>
    public class DrawCardCommand : ICommand
    {
        private readonly GameState _gameState;
        private readonly Player _player;
        private readonly CardDeck _deck;
        private readonly EventBus _eventBus;
        private readonly JailRules _jailRules;
        private Card _drawnCard;
        private int _previousMoney;
        private int _previousPosition;
        
        /// <summary>
        /// Gets the card that was drawn (null until Execute is called).
        /// </summary>
        public Card DrawnCard => _drawnCard;
        
        /// <summary>
        /// Initializes a new instance of the DrawCardCommand class.
        /// </summary>
        /// <param name="gameState">The current game state.</param>
        /// <param name="player">The player drawing the card.</param>
        /// <param name="deck">The deck to draw from.</param>
        /// <param name="eventBus">The event bus for publishing events.</param>
        /// <param name="jailRules">The jail rules handler.</param>
        public DrawCardCommand(GameState gameState, Player player, CardDeck deck, EventBus eventBus = null, JailRules jailRules = null)
        {
            _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _deck = deck ?? throw new ArgumentNullException(nameof(deck));
            _eventBus = eventBus;
            _jailRules = jailRules;
        }
        
        public CommandResult Execute()
        {
            try
            {
                // Store previous state for undo
                _previousMoney = _player.Money;
                _previousPosition = _player.Position;
                
                // Draw card
                _drawnCard = _deck.DrawCard();
                
                // Publish card drawn event
                _eventBus?.Publish(new CardDrawnEvent
                {
                    PlayerId = _player.Id,
                    CardType = _drawnCard.DeckType.ToString(),
                    CardText = _drawnCard.Text
                });
                
                // Execute card action
                var result = ExecuteCardAction(_drawnCard);
                
                // Return card to deck unless player keeps it
                if (!_drawnCard.IsKeptByPlayer)
                {
                    _deck.ReturnCard(_drawnCard);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                return CommandResult.Failed($"Failed to draw card: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Executes the action specified by the card.
        /// </summary>
        /// <param name="card">The card to execute.</param>
        /// <returns>Result of the card action.</returns>
        private CommandResult ExecuteCardAction(Card card)
        {
            switch (card.ActionType)
            {
                case CardActionType.CollectMoney:
                    _player.AddMoney(card.Value);
                    return CommandResult.Successful(new { Action = "Collected", Amount = card.Value });
                
                case CardActionType.PayMoney:
                    if (_player.Money >= card.Value)
                    {
                        _player.RemoveMoney(card.Value);
                        return CommandResult.Successful(new { Action = "Paid", Amount = card.Value });
                    }
                    else
                    {
                        return CommandResult.Failed($"Insufficient funds to pay ${card.Value}");
                    }
                
                case CardActionType.PayEachPlayer:
                    int totalPaid = 0;
                    foreach (var otherPlayer in _gameState.Players.Where(p => p.Id != _player.Id && !p.IsBankrupt))
                    {
                        if (_player.RemoveMoney(card.Value))
                        {
                            otherPlayer.AddMoney(card.Value);
                            totalPaid += card.Value;
                        }
                        else
                        {
                            return CommandResult.Failed($"Insufficient funds to pay all players");
                        }
                    }
                    return CommandResult.Successful(new { Action = "PaidEachPlayer", TotalPaid = totalPaid });
                
                case CardActionType.CollectFromEachPlayer:
                    int totalCollected = 0;
                    foreach (var otherPlayer in _gameState.Players.Where(p => p.Id != _player.Id && !p.IsBankrupt))
                    {
                        if (otherPlayer.RemoveMoney(card.Value))
                        {
                            _player.AddMoney(card.Value);
                            totalCollected += card.Value;
                        }
                    }
                    return CommandResult.Successful(new { Action = "CollectedFromEachPlayer", TotalCollected = totalCollected });
                
                case CardActionType.Move:
                    int oldPosition = _player.Position;
                    _player.Position = card.Value;
                    
                    // Check if passed GO
                    bool passedGo = card.Value < oldPosition || card.Value == 0;
                    if (passedGo)
                    {
                        _player.AddMoney(200);
                    }
                    
                    return CommandResult.Successful(new { Action = "Moved", NewPosition = card.Value, PassedGo = passedGo });
                
                case CardActionType.Advance:
                    int newPosition = (_player.Position + card.Value + 40) % 40;
                    bool passedGoAdvance = newPosition < _player.Position;
                    
                    _player.Position = newPosition;
                    
                    if (passedGoAdvance)
                    {
                        _player.AddMoney(200);
                    }
                    
                    return CommandResult.Successful(new { Action = "Advanced", NewPosition = newPosition, PassedGo = passedGoAdvance });
                
                case CardActionType.MoveToNearest:
                    return MoveToNearest(card.Value);
                
                case CardActionType.GoToJail:
                    if (_jailRules != null)
                    {
                        _jailRules.SendToJail(_player, "Card - Go to Jail");
                    }
                    else
                    {
                        _player.IsInJail = true;
                        _player.Position = JailRules.JailPosition;
                    }
                    return CommandResult.Successful(new { Action = "SentToJail" });
                
                case CardActionType.GetOutOfJailFree:
                    _player.GetOutOfJailFreeCards++;
                    return CommandResult.Successful(new { Action = "ReceivedGetOutOfJailFreeCard" });
                
                case CardActionType.PayPerHouse:
                    int houseCost = card.Value;
                    int hotelCost = card.SecondaryValue;
                    int totalCost = 0;
                    
                    foreach (var property in _player.OwnedProperties)
                    {
                        totalCost += property.Houses * houseCost;
                        if (property.HasHotel)
                            totalCost += hotelCost;
                    }
                    
                    if (_player.Money >= totalCost)
                    {
                        _player.RemoveMoney(totalCost);
                        return CommandResult.Successful(new { Action = "PaidRepairs", TotalCost = totalCost });
                    }
                    else
                    {
                        return CommandResult.Failed($"Insufficient funds to pay repairs (${totalCost})");
                    }
                
                default:
                    return CommandResult.Failed($"Unknown card action type: {card.ActionType}");
            }
        }
        
        /// <summary>
        /// Moves the player to the nearest railroad or utility.
        /// </summary>
        /// <param name="propertyType">1 for Utility, 2 for Railroad.</param>
        /// <returns>Command result.</returns>
        private CommandResult MoveToNearest(int propertyType)
        {
            int currentPos = _player.Position;
            int nearestPos = -1;
            
            if (propertyType == 1) // Utility
            {
                // Utilities are at positions 12 and 28
                int[] utilityPositions = { 12, 28 };
                nearestPos = FindNearestPosition(currentPos, utilityPositions);
            }
            else if (propertyType == 2) // Railroad
            {
                // Railroads are at positions 5, 15, 25, 35
                int[] railroadPositions = { 5, 15, 25, 35 };
                nearestPos = FindNearestPosition(currentPos, railroadPositions);
            }
            
            if (nearestPos == -1)
            {
                return CommandResult.Failed("Could not find nearest property");
            }
            
            bool passedGo = nearestPos < currentPos;
            _player.Position = nearestPos;
            
            if (passedGo)
            {
                _player.AddMoney(200);
            }
            
            return CommandResult.Successful(new { Action = "MovedToNearest", NewPosition = nearestPos, PassedGo = passedGo });
        }
        
        /// <summary>
        /// Finds the nearest position ahead of the current position.
        /// </summary>
        /// <param name="currentPos">Current position.</param>
        /// <param name="targets">Array of target positions.</param>
        /// <returns>The nearest position.</returns>
        private int FindNearestPosition(int currentPos, int[] targets)
        {
            // Find the first target position ahead of current position
            foreach (var target in targets.OrderBy(t => t))
            {
                if (target > currentPos)
                    return target;
            }
            
            // If no target ahead, wrap around to first target
            return targets.Min();
        }
        
        public void Undo()
        {
            if (_drawnCard == null || _player == null)
                return;
            
            // Restore previous state
            _player.Money = _previousMoney;
            _player.Position = _previousPosition;
            
            // Note: More complex undo logic would be needed for cards that affect other players
            // or modify game state in multiple ways. This is a simplified implementation.
        }
        
        public string SerializeToJson()
        {
            return JsonConvert.SerializeObject(new
            {
                Command = "DrawCard",
                PlayerId = _player.Id,
                DeckType = _deck.DeckType.ToString()
            });
        }
    }
}
