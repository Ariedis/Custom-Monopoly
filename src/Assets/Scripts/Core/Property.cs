using System;

namespace MonopolyFrenzy.Core
{
    /// <summary>
    /// Property color groups in Monopoly.
    /// </summary>
    public enum PropertyColor
    {
        Brown,
        LightBlue,
        Pink,
        Orange,
        Red,
        Yellow,
        Green,
        DarkBlue,
        Railroad,
        Utility
    }
    
    /// <summary>
    /// Represents a property that can be purchased and owned.
    /// </summary>
    public class Property : Space
    {
        /// <summary>
        /// Gets the purchase price of the property.
        /// </summary>
        public int PurchasePrice { get; private set; }
        
        /// <summary>
        /// Gets the base rent (no houses).
        /// </summary>
        public int BaseRent { get; private set; }
        
        /// <summary>
        /// Gets the rent with 1 house.
        /// </summary>
        public int RentWith1House { get; private set; }
        
        /// <summary>
        /// Gets the rent with 2 houses.
        /// </summary>
        public int RentWith2Houses { get; private set; }
        
        /// <summary>
        /// Gets the rent with 3 houses.
        /// </summary>
        public int RentWith3Houses { get; private set; }
        
        /// <summary>
        /// Gets the rent with 4 houses.
        /// </summary>
        public int RentWith4Houses { get; private set; }
        
        /// <summary>
        /// Gets the rent with a hotel.
        /// </summary>
        public int RentWithHotel { get; private set; }
        
        /// <summary>
        /// Gets the cost to build a house on this property.
        /// </summary>
        public int HouseCost { get; private set; }
        
        /// <summary>
        /// Gets the mortgage value of the property.
        /// </summary>
        public int MortgageValue { get; private set; }
        
        /// <summary>
        /// Gets the property color group.
        /// </summary>
        public PropertyColor Color { get; private set; }
        
        /// <summary>
        /// Gets or sets the owner of this property.
        /// </summary>
        public Player Owner { get; set; }
        
        /// <summary>
        /// Gets or sets the number of houses on this property (0-4).
        /// </summary>
        public int Houses { get; set; }
        
        /// <summary>
        /// Gets or sets whether this property has a hotel.
        /// </summary>
        public bool HasHotel { get; set; }
        
        /// <summary>
        /// Gets or sets whether this property is mortgaged.
        /// </summary>
        public bool IsMortgaged { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the Property class.
        /// </summary>
        public Property(
            int index, 
            string name, 
            PropertyColor color,
            int purchasePrice,
            int baseRent,
            int rentWith1House,
            int rentWith2Houses,
            int rentWith3Houses,
            int rentWith4Houses,
            int rentWithHotel,
            int houseCost) 
            : base(index, name, SpaceType.Property)
        {
            if (purchasePrice < 0)
                throw new ArgumentException("Purchase price cannot be negative", nameof(purchasePrice));
            
            Color = color;
            PurchasePrice = purchasePrice;
            BaseRent = baseRent;
            RentWith1House = rentWith1House;
            RentWith2Houses = rentWith2Houses;
            RentWith3Houses = rentWith3Houses;
            RentWith4Houses = rentWith4Houses;
            RentWithHotel = rentWithHotel;
            HouseCost = houseCost;
            MortgageValue = purchasePrice / 2;
            
            Owner = null;
            Houses = 0;
            HasHotel = false;
            IsMortgaged = false;
        }
        
        public override void OnLanded(Player player, GameState gameState)
        {
            // Rent payment or purchase offer (handled via commands)
        }
        
        /// <summary>
        /// Calculates the current rent for this property.
        /// </summary>
        /// <param name="diceRoll">The dice roll (used for utilities).</param>
        /// <returns>The rent amount.</returns>
        public int CalculateRent(int diceRoll = 0)
        {
            if (IsMortgaged || Owner == null)
                return 0;
            
            if (HasHotel)
                return RentWithHotel;
            
            if (Houses > 0)
            {
                return Houses switch
                {
                    1 => RentWith1House,
                    2 => RentWith2Houses,
                    3 => RentWith3Houses,
                    4 => RentWith4Houses,
                    _ => BaseRent
                };
            }
            
            return BaseRent;
        }
    }
    
    /// <summary>
    /// Represents a Railroad property.
    /// </summary>
    public class Railroad : Space
    {
        public int PurchasePrice { get; private set; }
        public int MortgageValue { get; private set; }
        public Player Owner { get; set; }
        public bool IsMortgaged { get; set; }
        
        public Railroad(int index, string name) : base(index, name, SpaceType.Railroad)
        {
            PurchasePrice = 200;
            MortgageValue = 100;
            Owner = null;
            IsMortgaged = false;
        }
        
        public override void OnLanded(Player player, GameState gameState)
        {
            // Rent payment or purchase offer (handled via commands)
        }
        
        /// <summary>
        /// Calculates rent based on number of railroads owned.
        /// </summary>
        /// <param name="railroadsOwned">Number of railroads owned by the owner.</param>
        /// <returns>The rent amount.</returns>
        public int CalculateRent(int railroadsOwned)
        {
            if (IsMortgaged || Owner == null)
                return 0;
            
            return railroadsOwned switch
            {
                1 => 25,
                2 => 50,
                3 => 100,
                4 => 200,
                _ => 0
            };
        }
    }
    
    /// <summary>
    /// Represents a Utility property (Electric Company or Water Works).
    /// </summary>
    public class Utility : Space
    {
        public int PurchasePrice { get; private set; }
        public int MortgageValue { get; private set; }
        public Player Owner { get; set; }
        public bool IsMortgaged { get; set; }
        
        public Utility(int index, string name) : base(index, name, SpaceType.Utility)
        {
            PurchasePrice = 150;
            MortgageValue = 75;
            Owner = null;
            IsMortgaged = false;
        }
        
        public override void OnLanded(Player player, GameState gameState)
        {
            // Rent payment or purchase offer (handled via commands)
        }
        
        /// <summary>
        /// Calculates rent based on dice roll and number of utilities owned.
        /// </summary>
        /// <param name="diceRoll">The dice roll.</param>
        /// <param name="utilitiesOwned">Number of utilities owned by the owner.</param>
        /// <returns>The rent amount.</returns>
        public int CalculateRent(int diceRoll, int utilitiesOwned)
        {
            if (IsMortgaged || Owner == null)
                return 0;
            
            int multiplier = utilitiesOwned == 1 ? 4 : 10;
            return diceRoll * multiplier;
        }
    }
}
