using System;

namespace MonopolyFrenzy.Core
{
    /// <summary>
    /// Type of card deck.
    /// </summary>
    public enum CardDeckType
    {
        Chance,
        CommunityChest
    }
    
    /// <summary>
    /// Type of card action.
    /// </summary>
    public enum CardActionType
    {
        CollectMoney,
        PayMoney,
        PayEachPlayer,
        CollectFromEachPlayer,
        Move,
        MoveToNearest,
        GoToJail,
        GetOutOfJailFree,
        PayPerHouse,
        Advance
    }
    
    /// <summary>
    /// Represents a Chance or Community Chest card.
    /// </summary>
    public class Card
    {
        /// <summary>
        /// Gets the unique identifier for this card.
        /// </summary>
        public string Id { get; private set; }
        
        /// <summary>
        /// Gets the card deck type (Chance or Community Chest).
        /// </summary>
        public CardDeckType DeckType { get; private set; }
        
        /// <summary>
        /// Gets the text displayed on the card.
        /// </summary>
        public string Text { get; private set; }
        
        /// <summary>
        /// Gets the type of action this card performs.
        /// </summary>
        public CardActionType ActionType { get; private set; }
        
        /// <summary>
        /// Gets the value associated with this card's action (e.g., money amount, position).
        /// </summary>
        public int Value { get; private set; }
        
        /// <summary>
        /// Gets the secondary value for complex actions (e.g., per house payment).
        /// </summary>
        public int SecondaryValue { get; private set; }
        
        /// <summary>
        /// Gets whether this card should be kept by the player (Get Out of Jail Free).
        /// </summary>
        public bool IsKeptByPlayer { get; private set; }
        
        /// <summary>
        /// Initializes a new instance of the Card class.
        /// </summary>
        /// <param name="id">Unique identifier.</param>
        /// <param name="deckType">Type of deck this card belongs to.</param>
        /// <param name="text">The text displayed on the card.</param>
        /// <param name="actionType">The type of action this card performs.</param>
        /// <param name="value">The value associated with the action.</param>
        /// <param name="secondaryValue">Secondary value for complex actions.</param>
        /// <param name="isKeptByPlayer">Whether the player keeps this card.</param>
        public Card(
            string id,
            CardDeckType deckType,
            string text,
            CardActionType actionType,
            int value = 0,
            int secondaryValue = 0,
            bool isKeptByPlayer = false)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException(nameof(text));
            
            Id = id;
            DeckType = deckType;
            Text = text;
            ActionType = actionType;
            Value = value;
            SecondaryValue = secondaryValue;
            IsKeptByPlayer = isKeptByPlayer;
        }
        
        /// <summary>
        /// Creates a standard Chance card.
        /// </summary>
        public static Card CreateChanceCard(string id, string text, CardActionType actionType, int value = 0, int secondaryValue = 0, bool isKeptByPlayer = false)
        {
            return new Card(id, CardDeckType.Chance, text, actionType, value, secondaryValue, isKeptByPlayer);
        }
        
        /// <summary>
        /// Creates a standard Community Chest card.
        /// </summary>
        public static Card CreateCommunityChestCard(string id, string text, CardActionType actionType, int value = 0, int secondaryValue = 0, bool isKeptByPlayer = false)
        {
            return new Card(id, CardDeckType.CommunityChest, text, actionType, value, secondaryValue, isKeptByPlayer);
        }
        
        public override string ToString()
        {
            return $"{DeckType}: {Text}";
        }
    }
}
