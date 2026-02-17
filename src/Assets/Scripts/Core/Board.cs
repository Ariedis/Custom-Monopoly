using System;
using System.Collections.Generic;

namespace MonopolyFrenzy.Core
{
    /// <summary>
    /// Represents the Monopoly game board with 40 spaces.
    /// </summary>
    public class Board
    {
        /// <summary>
        /// Gets the list of all spaces on the board (0-39).
        /// </summary>
        public List<Space> Spaces { get; private set; }
        
        /// <summary>
        /// Initializes a new instance of the Board class with standard Monopoly spaces.
        /// </summary>
        public Board()
        {
            Spaces = new List<Space>(40);
            InitializeStandardBoard();
        }
        
        /// <summary>
        /// Creates the standard Monopoly board with all 40 spaces.
        /// </summary>
        private void InitializeStandardBoard()
        {
            // Position 0: GO
            Spaces.Add(new GoSpace());
            
            // Position 1: Mediterranean Avenue (Brown)
            Spaces.Add(new Property(1, "Mediterranean Avenue", PropertyColor.Brown, 60, 2, 10, 30, 90, 160, 250, 50));
            
            // Position 2: Community Chest
            Spaces.Add(new CommunityChestSpace(2));
            
            // Position 3: Baltic Avenue (Brown)
            Spaces.Add(new Property(3, "Baltic Avenue", PropertyColor.Brown, 60, 4, 20, 60, 180, 320, 450, 50));
            
            // Position 4: Income Tax
            Spaces.Add(new TaxSpace(4, "Income Tax", 200));
            
            // Position 5: Reading Railroad
            Spaces.Add(new Railroad(5, "Reading Railroad"));
            
            // Position 6: Oriental Avenue (Light Blue)
            Spaces.Add(new Property(6, "Oriental Avenue", PropertyColor.LightBlue, 100, 6, 30, 90, 270, 400, 550, 50));
            
            // Position 7: Chance
            Spaces.Add(new ChanceSpace(7));
            
            // Position 8: Vermont Avenue (Light Blue)
            Spaces.Add(new Property(8, "Vermont Avenue", PropertyColor.LightBlue, 100, 6, 30, 90, 270, 400, 550, 50));
            
            // Position 9: Connecticut Avenue (Light Blue)
            Spaces.Add(new Property(9, "Connecticut Avenue", PropertyColor.LightBlue, 120, 8, 40, 100, 300, 450, 600, 50));
            
            // Position 10: Jail/Just Visiting
            Spaces.Add(new JailSpace());
            
            // Position 11: St. Charles Place (Pink)
            Spaces.Add(new Property(11, "St. Charles Place", PropertyColor.Pink, 140, 10, 50, 150, 450, 625, 750, 100));
            
            // Position 12: Electric Company
            Spaces.Add(new Utility(12, "Electric Company"));
            
            // Position 13: States Avenue (Pink)
            Spaces.Add(new Property(13, "States Avenue", PropertyColor.Pink, 140, 10, 50, 150, 450, 625, 750, 100));
            
            // Position 14: Virginia Avenue (Pink)
            Spaces.Add(new Property(14, "Virginia Avenue", PropertyColor.Pink, 160, 12, 60, 180, 500, 700, 900, 100));
            
            // Position 15: Pennsylvania Railroad
            Spaces.Add(new Railroad(15, "Pennsylvania Railroad"));
            
            // Position 16: St. James Place (Orange)
            Spaces.Add(new Property(16, "St. James Place", PropertyColor.Orange, 180, 14, 70, 200, 550, 750, 950, 100));
            
            // Position 17: Community Chest
            Spaces.Add(new CommunityChestSpace(17));
            
            // Position 18: Tennessee Avenue (Orange)
            Spaces.Add(new Property(18, "Tennessee Avenue", PropertyColor.Orange, 180, 14, 70, 200, 550, 750, 950, 100));
            
            // Position 19: New York Avenue (Orange)
            Spaces.Add(new Property(19, "New York Avenue", PropertyColor.Orange, 200, 16, 80, 220, 600, 800, 1000, 100));
            
            // Position 20: Free Parking
            Spaces.Add(new FreeParkingSpace());
            
            // Position 21: Kentucky Avenue (Red)
            Spaces.Add(new Property(21, "Kentucky Avenue", PropertyColor.Red, 220, 18, 90, 250, 700, 875, 1050, 150));
            
            // Position 22: Chance
            Spaces.Add(new ChanceSpace(22));
            
            // Position 23: Indiana Avenue (Red)
            Spaces.Add(new Property(23, "Indiana Avenue", PropertyColor.Red, 220, 18, 90, 250, 700, 875, 1050, 150));
            
            // Position 24: Illinois Avenue (Red)
            Spaces.Add(new Property(24, "Illinois Avenue", PropertyColor.Red, 240, 20, 100, 300, 750, 925, 1100, 150));
            
            // Position 25: B&O Railroad
            Spaces.Add(new Railroad(25, "B&O Railroad"));
            
            // Position 26: Atlantic Avenue (Yellow)
            Spaces.Add(new Property(26, "Atlantic Avenue", PropertyColor.Yellow, 260, 22, 110, 330, 800, 975, 1150, 150));
            
            // Position 27: Ventnor Avenue (Yellow)
            Spaces.Add(new Property(27, "Ventnor Avenue", PropertyColor.Yellow, 260, 22, 110, 330, 800, 975, 1150, 150));
            
            // Position 28: Water Works
            Spaces.Add(new Utility(28, "Water Works"));
            
            // Position 29: Marvin Gardens (Yellow)
            Spaces.Add(new Property(29, "Marvin Gardens", PropertyColor.Yellow, 280, 24, 120, 360, 850, 1025, 1200, 150));
            
            // Position 30: Go To Jail
            Spaces.Add(new GoToJailSpace());
            
            // Position 31: Pacific Avenue (Green)
            Spaces.Add(new Property(31, "Pacific Avenue", PropertyColor.Green, 300, 26, 130, 390, 900, 1100, 1275, 200));
            
            // Position 32: North Carolina Avenue (Green)
            Spaces.Add(new Property(32, "North Carolina Avenue", PropertyColor.Green, 300, 26, 130, 390, 900, 1100, 1275, 200));
            
            // Position 33: Community Chest
            Spaces.Add(new CommunityChestSpace(33));
            
            // Position 34: Pennsylvania Avenue (Green)
            Spaces.Add(new Property(34, "Pennsylvania Avenue", PropertyColor.Green, 320, 28, 150, 450, 1000, 1200, 1400, 200));
            
            // Position 35: Short Line Railroad
            Spaces.Add(new Railroad(35, "Short Line Railroad"));
            
            // Position 36: Chance
            Spaces.Add(new ChanceSpace(36));
            
            // Position 37: Park Place (Dark Blue)
            Spaces.Add(new Property(37, "Park Place", PropertyColor.DarkBlue, 350, 35, 175, 500, 1100, 1300, 1500, 200));
            
            // Position 38: Luxury Tax
            Spaces.Add(new TaxSpace(38, "Luxury Tax", 100));
            
            // Position 39: Boardwalk (Dark Blue)
            Spaces.Add(new Property(39, "Boardwalk", PropertyColor.DarkBlue, 400, 50, 200, 600, 1400, 1700, 2000, 200));
        }
        
        /// <summary>
        /// Gets a space at the specified index.
        /// </summary>
        /// <param name="index">The index of the space (0-39).</param>
        /// <returns>The space at the specified index, or null if invalid.</returns>
        public Space GetSpace(int index)
        {
            if (index < 0 || index >= 40)
                return null;
            
            return Spaces[index];
        }
        
        /// <summary>
        /// Gets all properties of a specific color group.
        /// </summary>
        /// <param name="color">The property color.</param>
        /// <returns>List of properties in the color group.</returns>
        public List<Property> GetPropertiesByColor(PropertyColor color)
        {
            var properties = new List<Property>();
            
            foreach (var space in Spaces)
            {
                if (space is Property property && property.Color == color)
                {
                    properties.Add(property);
                }
            }
            
            return properties;
        }
        
        /// <summary>
        /// Gets all railroads on the board.
        /// </summary>
        /// <returns>List of railroad spaces.</returns>
        public List<Railroad> GetRailroads()
        {
            var railroads = new List<Railroad>();
            
            foreach (var space in Spaces)
            {
                if (space is Railroad railroad)
                {
                    railroads.Add(railroad);
                }
            }
            
            return railroads;
        }
        
        /// <summary>
        /// Gets all utilities on the board.
        /// </summary>
        /// <returns>List of utility spaces.</returns>
        public List<Utility> GetUtilities()
        {
            var utilities = new List<Utility>();
            
            foreach (var space in Spaces)
            {
                if (space is Utility utility)
                {
                    utilities.Add(utility);
                }
            }
            
            return utilities;
        }
    }
}
