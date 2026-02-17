using System;

namespace MonopolyFrenzy.Core
{
    /// <summary>
    /// Types of spaces on the Monopoly board.
    /// </summary>
    public enum SpaceType
    {
        Go,
        Property,
        Railroad,
        Utility,
        Tax,
        Chance,
        CommunityChest,
        Jail,
        FreeParking,
        GoToJail
    }
    
    /// <summary>
    /// Represents a space on the Monopoly board.
    /// Base class for all board spaces.
    /// </summary>
    public abstract class Space
    {
        /// <summary>
        /// Gets the position index of this space on the board (0-39).
        /// </summary>
        public int Index { get; protected set; }
        
        /// <summary>
        /// Gets the name of this space.
        /// </summary>
        public string Name { get; protected set; }
        
        /// <summary>
        /// Gets the type of this space.
        /// </summary>
        public SpaceType Type { get; protected set; }
        
        /// <summary>
        /// Initializes a new instance of the Space class.
        /// </summary>
        /// <param name="index">Position index on the board.</param>
        /// <param name="name">Name of the space.</param>
        /// <param name="type">Type of the space.</param>
        protected Space(int index, string name, SpaceType type)
        {
            if (index < 0 || index >= 40)
                throw new ArgumentOutOfRangeException(nameof(index), "Space index must be between 0 and 39");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            
            Index = index;
            Name = name;
            Type = type;
        }
        
        /// <summary>
        /// Called when a player lands on this space.
        /// </summary>
        /// <param name="player">The player who landed on this space.</param>
        /// <param name="gameState">The current game state.</param>
        public abstract void OnLanded(Player player, GameState gameState);
    }
    
    /// <summary>
    /// Represents the GO space.
    /// </summary>
    public class GoSpace : Space
    {
        public GoSpace() : base(0, "GO", SpaceType.Go) { }
        
        public override void OnLanded(Player player, GameState gameState)
        {
            // Collect $200 for passing GO (handled elsewhere)
        }
    }
    
    /// <summary>
    /// Represents a Jail/Just Visiting space.
    /// </summary>
    public class JailSpace : Space
    {
        public JailSpace() : base(10, "Jail/Just Visiting", SpaceType.Jail) { }
        
        public override void OnLanded(Player player, GameState gameState)
        {
            // Just visiting, no action needed unless sent to jail
        }
    }
    
    /// <summary>
    /// Represents the Free Parking space.
    /// </summary>
    public class FreeParkingSpace : Space
    {
        public FreeParkingSpace() : base(20, "Free Parking", SpaceType.FreeParking) { }
        
        public override void OnLanded(Player player, GameState gameState)
        {
            // No action in standard rules (configurable in house rules)
        }
    }
    
    /// <summary>
    /// Represents the Go To Jail space.
    /// </summary>
    public class GoToJailSpace : Space
    {
        public GoToJailSpace() : base(30, "Go To Jail", SpaceType.GoToJail) { }
        
        public override void OnLanded(Player player, GameState gameState)
        {
            player.IsInJail = true;
            player.Position = 10; // Move to jail
            player.JailTurns = 0;
        }
    }
    
    /// <summary>
    /// Represents a Tax space (Income Tax or Luxury Tax).
    /// </summary>
    public class TaxSpace : Space
    {
        /// <summary>
        /// Gets the amount of tax to be paid.
        /// </summary>
        public int TaxAmount { get; private set; }
        
        public TaxSpace(int index, string name, int taxAmount) : base(index, name, SpaceType.Tax)
        {
            if (taxAmount < 0)
                throw new ArgumentException("Tax amount cannot be negative", nameof(taxAmount));
            
            TaxAmount = taxAmount;
        }
        
        public override void OnLanded(Player player, GameState gameState)
        {
            // Tax will be paid via command
        }
    }
    
    /// <summary>
    /// Represents a Chance space.
    /// </summary>
    public class ChanceSpace : Space
    {
        public ChanceSpace(int index) : base(index, "Chance", SpaceType.Chance) { }
        
        public override void OnLanded(Player player, GameState gameState)
        {
            // Draw a Chance card (handled via command)
        }
    }
    
    /// <summary>
    /// Represents a Community Chest space.
    /// </summary>
    public class CommunityChestSpace : Space
    {
        public CommunityChestSpace(int index) : base(index, "Community Chest", SpaceType.CommunityChest) { }
        
        public override void OnLanded(Player player, GameState gameState)
        {
            // Draw a Community Chest card (handled via command)
        }
    }
}
