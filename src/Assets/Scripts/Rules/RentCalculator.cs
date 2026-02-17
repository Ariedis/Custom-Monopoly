using System;
using System.Collections.Generic;
using System.Linq;
using MonopolyFrenzy.Core;

namespace MonopolyFrenzy.Rules
{
    /// <summary>
    /// Calculates rent for properties, railroads, and utilities.
    /// </summary>
    public class RentCalculator
    {
        private readonly GameState _gameState;
        
        /// <summary>
        /// Initializes a new instance of the RentCalculator class.
        /// </summary>
        /// <param name="gameState">The current game state.</param>
        public RentCalculator(GameState gameState)
        {
            _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
        }
        
        /// <summary>
        /// Calculates rent for a property space.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="diceRoll">The dice roll (for utilities).</param>
        /// <returns>The rent amount.</returns>
        public int CalculateRent(Property property, int diceRoll = 0)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));
            
            // No rent if mortgaged or unowned
            if (property.IsMortgaged || property.Owner == null)
                return 0;
            
            // Check if owner has monopoly
            bool hasMonopoly = OwnsMonopoly(property.Owner, property.Color);
            
            // Hotel rent
            if (property.HasHotel)
                return property.RentWithHotel;
            
            // Rent with houses
            if (property.Houses > 0)
            {
                return property.Houses switch
                {
                    1 => property.RentWith1House,
                    2 => property.RentWith2Houses,
                    3 => property.RentWith3Houses,
                    4 => property.RentWith4Houses,
                    _ => property.BaseRent
                };
            }
            
            // Base rent - doubled if monopoly and no houses
            int rent = property.BaseRent;
            if (hasMonopoly)
                rent *= 2;
            
            return rent;
        }
        
        /// <summary>
        /// Calculates rent for a railroad.
        /// </summary>
        /// <param name="railroad">The railroad.</param>
        /// <returns>The rent amount.</returns>
        public int CalculateRailroadRent(Railroad railroad)
        {
            if (railroad == null)
                throw new ArgumentNullException(nameof(railroad));
            
            if (railroad.IsMortgaged || railroad.Owner == null)
                return 0;
            
            int railroadsOwned = CountRailroadsOwned(railroad.Owner);
            return railroad.CalculateRent(railroadsOwned);
        }
        
        /// <summary>
        /// Calculates rent for a utility.
        /// </summary>
        /// <param name="utility">The utility.</param>
        /// <param name="diceRoll">The dice roll.</param>
        /// <returns>The rent amount.</returns>
        public int CalculateUtilityRent(Utility utility, int diceRoll)
        {
            if (utility == null)
                throw new ArgumentNullException(nameof(utility));
            
            if (utility.IsMortgaged || utility.Owner == null)
                return 0;
            
            int utilitiesOwned = CountUtilitiesOwned(utility.Owner);
            return utility.CalculateRent(diceRoll, utilitiesOwned);
        }
        
        /// <summary>
        /// Checks if a player owns all properties in a color group.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="color">The property color.</param>
        /// <returns>True if the player owns a monopoly, false otherwise.</returns>
        public bool OwnsMonopoly(Player player, PropertyColor color)
        {
            if (player == null || _gameState.Board == null)
                return false;
            
            var propertiesInGroup = _gameState.Board.GetPropertiesByColor(color);
            
            if (propertiesInGroup.Count == 0)
                return false;
            
            return propertiesInGroup.All(p => p.Owner == player);
        }
        
        /// <summary>
        /// Counts the number of railroads owned by a player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>The number of railroads owned.</returns>
        private int CountRailroadsOwned(Player player)
        {
            if (player == null || _gameState.Board == null)
                return 0;
            
            var railroads = _gameState.Board.GetRailroads();
            return railroads.Count(r => r.Owner == player);
        }
        
        /// <summary>
        /// Counts the number of utilities owned by a player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>The number of utilities owned.</returns>
        private int CountUtilitiesOwned(Player player)
        {
            if (player == null || _gameState.Board == null)
                return 0;
            
            var utilities = _gameState.Board.GetUtilities();
            return utilities.Count(u => u.Owner == player);
        }
    }
}
