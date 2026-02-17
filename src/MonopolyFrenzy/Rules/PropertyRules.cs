using System;
using System.Collections.Generic;
using System.Linq;
using MonopolyFrenzy.Core;

namespace MonopolyFrenzy.Rules
{
    /// <summary>
    /// Validates and enforces property-related rules.
    /// </summary>
    public class PropertyRules
    {
        private readonly GameState _gameState;
        private readonly RentCalculator _rentCalculator;
        
        /// <summary>
        /// Initializes a new instance of the PropertyRules class.
        /// </summary>
        /// <param name="gameState">The current game state.</param>
        public PropertyRules(GameState gameState)
        {
            _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
            _rentCalculator = new RentCalculator(gameState);
        }
        
        /// <summary>
        /// Validates if a player can purchase a property.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="property">The property.</param>
        /// <returns>Tuple of (canPurchase, errorMessage).</returns>
        public (bool canPurchase, string errorMessage) CanPurchaseProperty(Player player, Property property)
        {
            if (player == null)
                return (false, "Player is null");
            
            if (property == null)
                return (false, "Property is null");
            
            if (property.Owner != null)
                return (false, "Property is already owned");
            
            if (property.IsMortgaged)
                return (false, "Property is mortgaged");
            
            if (player.Money < property.PurchasePrice)
                return (false, $"Insufficient funds. Need ${property.PurchasePrice}, have ${player.Money}");
            
            return (true, null);
        }
        
        /// <summary>
        /// Validates if a player can build a house on a property.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="property">The property.</param>
        /// <returns>Tuple of (canBuild, errorMessage).</returns>
        public (bool canBuild, string errorMessage) CanBuildHouse(Player player, Property property)
        {
            if (player == null)
                return (false, "Player is null");
            
            if (property == null)
                return (false, "Property is null");
            
            if (property.Owner != player)
                return (false, "Player does not own this property");
            
            if (property.IsMortgaged)
                return (false, "Cannot build on mortgaged property");
            
            if (property.HasHotel)
                return (false, "Property already has a hotel");
            
            if (property.Houses >= 4)
                return (false, "Property already has 4 houses. Build a hotel instead.");
            
            // Must own monopoly
            if (!_rentCalculator.OwnsMonopoly(player, property.Color))
                return (false, "Must own all properties in color group to build");
            
            // Check if player has enough money
            if (player.Money < property.HouseCost)
                return (false, $"Insufficient funds. House costs ${property.HouseCost}, have ${player.Money}");
            
            // Even building rule: can't build if it would make this property have 2+ more houses than others in group
            var propertiesInGroup = _gameState.Board.GetPropertiesByColor(property.Color);
            int minHouses = propertiesInGroup.Where(p => p != property).Min(p => p.Houses);
            
            if (property.Houses > minHouses)
                return (false, "Must build evenly across color group");
            
            return (true, null);
        }
        
        /// <summary>
        /// Validates if a player can build a hotel on a property.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="property">The property.</param>
        /// <returns>Tuple of (canBuild, errorMessage).</returns>
        public (bool canBuild, string errorMessage) CanBuildHotel(Player player, Property property)
        {
            if (player == null)
                return (false, "Player is null");
            
            if (property == null)
                return (false, "Property is null");
            
            if (property.Owner != player)
                return (false, "Player does not own this property");
            
            if (property.IsMortgaged)
                return (false, "Cannot build on mortgaged property");
            
            if (property.HasHotel)
                return (false, "Property already has a hotel");
            
            if (property.Houses < 4)
                return (false, "Must have 4 houses before building hotel");
            
            // Must own monopoly
            if (!_rentCalculator.OwnsMonopoly(player, property.Color))
                return (false, "Must own all properties in color group to build");
            
            // Check if player has enough money
            if (player.Money < property.HouseCost)
                return (false, $"Insufficient funds. Hotel costs ${property.HouseCost}, have ${player.Money}");
            
            return (true, null);
        }
        
        /// <summary>
        /// Validates if a property can be mortgaged.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="property">The property.</param>
        /// <returns>Tuple of (canMortgage, errorMessage).</returns>
        public (bool canMortgage, string errorMessage) CanMortgageProperty(Player player, Property property)
        {
            if (player == null)
                return (false, "Player is null");
            
            if (property == null)
                return (false, "Property is null");
            
            if (property.Owner != player)
                return (false, "Player does not own this property");
            
            if (property.IsMortgaged)
                return (false, "Property is already mortgaged");
            
            if (property.Houses > 0 || property.HasHotel)
                return (false, "Must sell all houses/hotels before mortgaging");
            
            return (true, null);
        }
        
        /// <summary>
        /// Validates if a property can be unmortgaged.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="property">The property.</param>
        /// <returns>Tuple of (canUnmortgage, errorMessage).</returns>
        public (bool canUnmortgage, string errorMessage) CanUnmortgageProperty(Player player, Property property)
        {
            if (player == null)
                return (false, "Player is null");
            
            if (property == null)
                return (false, "Property is null");
            
            if (property.Owner != player)
                return (false, "Player does not own this property");
            
            if (!property.IsMortgaged)
                return (false, "Property is not mortgaged");
            
            // Unmortgage cost is mortgage value + 10%
            int unmortgageCost = (int)(property.MortgageValue * 1.1);
            
            if (player.Money < unmortgageCost)
                return (false, $"Insufficient funds. Need ${unmortgageCost}, have ${player.Money}");
            
            return (true, null);
        }
    }
}
