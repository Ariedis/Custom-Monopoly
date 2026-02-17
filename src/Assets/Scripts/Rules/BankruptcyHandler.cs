using System;
using System.Linq;
using MonopolyFrenzy.Core;
using MonopolyFrenzy.Events;

namespace MonopolyFrenzy.Rules
{
    /// <summary>
    /// Handles player bankruptcy, asset distribution, and game over conditions.
    /// </summary>
    public class BankruptcyHandler
    {
        private readonly GameState _gameState;
        private readonly EventBus _eventBus;
        
        /// <summary>
        /// Initializes a new instance of the BankruptcyHandler class.
        /// </summary>
        /// <param name="gameState">The current game state.</param>
        /// <param name="eventBus">The event bus for publishing bankruptcy events.</param>
        public BankruptcyHandler(GameState gameState, EventBus eventBus = null)
        {
            _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
            _eventBus = eventBus;
        }
        
        /// <summary>
        /// Declares a player bankrupt and handles asset distribution.
        /// </summary>
        /// <param name="bankruptPlayer">The player going bankrupt.</param>
        /// <param name="creditor">The creditor (null if bankrupt to bank).</param>
        /// <returns>True if bankruptcy was processed successfully.</returns>
        public bool DeclareBankruptcy(Player bankruptPlayer, Player creditor = null)
        {
            if (bankruptPlayer == null)
                throw new ArgumentNullException(nameof(bankruptPlayer));
            
            if (bankruptPlayer.IsBankrupt)
                return false; // Already bankrupt
            
            // Mark player as bankrupt
            bankruptPlayer.IsBankrupt = true;
            bankruptPlayer.Money = 0;
            
            int assetsTransferred = 0;
            
            // Transfer all properties
            if (bankruptPlayer.OwnedProperties.Count > 0)
            {
                var properties = bankruptPlayer.OwnedProperties.ToList();
                
                foreach (var property in properties)
                {
                    // Remove houses/hotels
                    if (property.Houses > 0)
                    {
                        assetsTransferred += property.Houses * (property.HouseCost / 2);
                        property.Houses = 0;
                    }
                    
                    if (property.HasHotel)
                    {
                        assetsTransferred += property.HouseCost / 2;
                        property.HasHotel = false;
                    }
                    
                    // Transfer property to creditor or return to bank
                    if (creditor != null)
                    {
                        // Transfer to creditor
                        property.Owner = creditor;
                        creditor.OwnedProperties.Add(property);
                        assetsTransferred += property.IsMortgaged ? 0 : property.MortgageValue;
                    }
                    else
                    {
                        // Return to bank (unowned and unmortgaged)
                        property.Owner = null;
                        property.IsMortgaged = false;
                        assetsTransferred += property.MortgageValue;
                    }
                }
                
                bankruptPlayer.OwnedProperties.Clear();
            }
            
            // Transfer Get Out of Jail Free cards
            if (bankruptPlayer.GetOutOfJailFreeCards > 0 && creditor != null)
            {
                creditor.GetOutOfJailFreeCards += bankruptPlayer.GetOutOfJailFreeCards;
                bankruptPlayer.GetOutOfJailFreeCards = 0;
            }
            
            // Publish bankruptcy event
            _eventBus?.Publish(new PlayerBankruptEvent
            {
                PlayerId = bankruptPlayer.Id,
                CreditorId = creditor?.Id,
                AssetsTransferred = assetsTransferred
            });
            
            // Check for game over
            CheckGameOver();
            
            return true;
        }
        
        /// <summary>
        /// Checks if a player can pay a debt, considering all assets.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="amount">The amount owed.</param>
        /// <returns>True if the player can potentially pay through liquidation.</returns>
        public bool CanPayDebt(Player player, int amount)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));
            
            if (player.Money >= amount)
                return true;
            
            // Calculate total liquidation value
            int totalValue = player.Money;
            
            foreach (var property in player.OwnedProperties)
            {
                // Value of houses/hotels (sell at half price)
                if (property.Houses > 0)
                    totalValue += property.Houses * (property.HouseCost / 2);
                
                if (property.HasHotel)
                    totalValue += property.HouseCost / 2;
                
                // Mortgage value
                if (!property.IsMortgaged)
                    totalValue += property.MortgageValue;
            }
            
            return totalValue >= amount;
        }
        
        /// <summary>
        /// Calculates the total asset value of a player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>Total value including cash, properties, and buildings.</returns>
        public int CalculateTotalAssets(Player player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));
            
            int totalValue = player.Money;
            
            foreach (var property in player.OwnedProperties)
            {
                // Property value (unmortgaged properties)
                if (!property.IsMortgaged)
                    totalValue += property.PurchasePrice;
                
                // Building value
                if (property.Houses > 0)
                    totalValue += property.Houses * property.HouseCost;
                
                if (property.HasHotel)
                    totalValue += property.HouseCost;
            }
            
            return totalValue;
        }
        
        /// <summary>
        /// Checks if the game should end and publishes game over event if so.
        /// </summary>
        private void CheckGameOver()
        {
            var activePlayers = _gameState.Players.Where(p => !p.IsBankrupt).ToList();
            
            if (activePlayers.Count == 1)
            {
                var winner = activePlayers[0];
                
                _eventBus?.Publish(new GameOverEvent
                {
                    WinnerId = winner.Id,
                    WinnerName = winner.Name,
                    TotalTurns = _gameState.TurnNumber
                });
            }
            else if (activePlayers.Count == 0)
            {
                // Edge case: All players bankrupt simultaneously
                _eventBus?.Publish(new GameOverEvent
                {
                    WinnerId = null,
                    WinnerName = "No Winner",
                    TotalTurns = _gameState.TurnNumber
                });
            }
        }
        
        /// <summary>
        /// Gets the count of active (non-bankrupt) players.
        /// </summary>
        /// <returns>Number of active players.</returns>
        public int GetActivePlayerCount()
        {
            return _gameState.Players.Count(p => !p.IsBankrupt);
        }
        
        /// <summary>
        /// Determines if the game is over (only one or zero active players).
        /// </summary>
        /// <returns>True if the game is over.</returns>
        public bool IsGameOver()
        {
            return GetActivePlayerCount() <= 1;
        }
    }
}
