using System;
using System.Collections.Generic;

namespace MonopolyFrenzy.Core
{
    /// <summary>
    /// Represents a player in the Monopoly game.
    /// Contains player data including money, position, and ownership.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Gets the unique identifier for this player.
        /// </summary>
        public string Id { get; private set; }
        
        /// <summary>
        /// Gets or sets the player's display name.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the player's current money balance.
        /// </summary>
        public int Money { get; set; }
        
        /// <summary>
        /// Gets or sets the player's current position on the board (0-39).
        /// </summary>
        public int Position { get; set; }
        
        /// <summary>
        /// Gets or sets whether this player is bankrupt and eliminated from the game.
        /// </summary>
        public bool IsBankrupt { get; set; }
        
        /// <summary>
        /// Gets or sets whether this player is currently in jail.
        /// </summary>
        public bool IsInJail { get; set; }
        
        /// <summary>
        /// Gets or sets the number of turns the player has been in jail.
        /// </summary>
        public int JailTurns { get; set; }
        
        /// <summary>
        /// Gets or sets the number of "Get Out of Jail Free" cards the player owns.
        /// </summary>
        public int GetOutOfJailFreeCards { get; set; }
        
        /// <summary>
        /// Gets the list of properties owned by this player.
        /// </summary>
        public List<Property> OwnedProperties { get; private set; }
        
        /// <summary>
        /// Initializes a new instance of the Player class.
        /// </summary>
        /// <param name="id">Unique identifier for the player.</param>
        /// <param name="name">Display name for the player.</param>
        /// <param name="startingMoney">Starting money balance (default 1500).</param>
        public Player(string id, string name, int startingMoney = 1500)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (startingMoney < 0)
                throw new ArgumentException("Starting money cannot be negative", nameof(startingMoney));
            
            Id = id;
            Name = name;
            Money = startingMoney;
            Position = 0;
            IsBankrupt = false;
            IsInJail = false;
            JailTurns = 0;
            GetOutOfJailFreeCards = 0;
            OwnedProperties = new List<Property>();
        }
        
        /// <summary>
        /// Adds money to the player's balance.
        /// </summary>
        /// <param name="amount">Amount to add.</param>
        public void AddMoney(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));
            
            Money += amount;
        }
        
        /// <summary>
        /// Removes money from the player's balance.
        /// </summary>
        /// <param name="amount">Amount to remove.</param>
        /// <returns>True if the player has enough money, false otherwise.</returns>
        public bool RemoveMoney(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));
            
            if (Money >= amount)
            {
                Money -= amount;
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Creates a deep copy of this player.
        /// </summary>
        /// <returns>A new Player instance with the same values.</returns>
        public Player Clone()
        {
            var clone = new Player(Id, Name, Money)
            {
                Position = Position,
                IsBankrupt = IsBankrupt,
                IsInJail = IsInJail,
                JailTurns = JailTurns,
                GetOutOfJailFreeCards = GetOutOfJailFreeCards
            };
            
            // Note: OwnedProperties will need to be cloned separately in context
            // to avoid circular references with Property.Owner
            
            return clone;
        }
    }
}
