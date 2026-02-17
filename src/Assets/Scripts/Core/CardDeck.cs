using System;
using System.Collections.Generic;
using System.Linq;

namespace MonopolyFrenzy.Core
{
    /// <summary>
    /// Represents a deck of cards (Chance or Community Chest).
    /// </summary>
    public class CardDeck
    {
        private readonly List<Card> _cards;
        private readonly Queue<Card> _drawPile;
        private readonly Random _random;
        
        /// <summary>
        /// Gets the type of deck (Chance or Community Chest).
        /// </summary>
        public CardDeckType DeckType { get; private set; }
        
        /// <summary>
        /// Gets the number of cards remaining in the draw pile.
        /// </summary>
        public int CardsRemaining => _drawPile.Count;
        
        /// <summary>
        /// Initializes a new instance of the CardDeck class.
        /// </summary>
        /// <param name="deckType">The type of deck.</param>
        /// <param name="random">Random number generator for shuffling.</param>
        public CardDeck(CardDeckType deckType, Random random = null)
        {
            DeckType = deckType;
            _random = random ?? new Random();
            _cards = new List<Card>();
            _drawPile = new Queue<Card>();
            
            InitializeDeck();
            Shuffle();
        }
        
        /// <summary>
        /// Initializes the deck with standard Monopoly cards.
        /// </summary>
        private void InitializeDeck()
        {
            if (DeckType == CardDeckType.Chance)
            {
                InitializeChanceCards();
            }
            else
            {
                InitializeCommunityChestCards();
            }
        }
        
        /// <summary>
        /// Initializes the 16 standard Chance cards.
        /// </summary>
        private void InitializeChanceCards()
        {
            _cards.Add(Card.CreateChanceCard("CH01", "Advance to Go (Collect $200)", CardActionType.Move, 0));
            _cards.Add(Card.CreateChanceCard("CH02", "Advance to Illinois Ave - If you pass Go, collect $200", CardActionType.Move, 24));
            _cards.Add(Card.CreateChanceCard("CH03", "Advance to St. Charles Place - If you pass Go, collect $200", CardActionType.Move, 11));
            _cards.Add(Card.CreateChanceCard("CH04", "Advance token to nearest Utility. If unowned, you may buy it from the Bank. If owned, pay owner 10x dice roll", CardActionType.MoveToNearest, 1)); // 1 = Utility
            _cards.Add(Card.CreateChanceCard("CH05", "Advance token to nearest Railroad. If unowned, you may buy it from the Bank. If owned, pay owner twice the rental", CardActionType.MoveToNearest, 2)); // 2 = Railroad
            _cards.Add(Card.CreateChanceCard("CH06", "Advance token to nearest Railroad. If unowned, you may buy it from the Bank. If owned, pay owner twice the rental", CardActionType.MoveToNearest, 2)); // 2 = Railroad (duplicate)
            _cards.Add(Card.CreateChanceCard("CH07", "Bank pays you dividend of $50", CardActionType.CollectMoney, 50));
            _cards.Add(Card.CreateChanceCard("CH08", "Get Out of Jail Free - This card may be kept until needed or sold", CardActionType.GetOutOfJailFree, 0, 0, true));
            _cards.Add(Card.CreateChanceCard("CH09", "Go Back 3 Spaces", CardActionType.Advance, -3));
            _cards.Add(Card.CreateChanceCard("CH10", "Go to Jail - Go directly to Jail - Do not pass Go, do not collect $200", CardActionType.GoToJail, 0));
            _cards.Add(Card.CreateChanceCard("CH11", "Make general repairs on all your property - For each house pay $25 - For each hotel pay $100", CardActionType.PayPerHouse, 25, 100));
            _cards.Add(Card.CreateChanceCard("CH12", "Pay poor tax of $15", CardActionType.PayMoney, 15));
            _cards.Add(Card.CreateChanceCard("CH13", "Take a trip to Reading Railroad - If you pass Go, collect $200", CardActionType.Move, 5));
            _cards.Add(Card.CreateChanceCard("CH14", "Take a walk on the Boardwalk - Advance token to Boardwalk", CardActionType.Move, 39));
            _cards.Add(Card.CreateChanceCard("CH15", "You have been elected Chairman of the Board - Pay each player $50", CardActionType.PayEachPlayer, 50));
            _cards.Add(Card.CreateChanceCard("CH16", "Your building and loan matures - Collect $150", CardActionType.CollectMoney, 150));
        }
        
        /// <summary>
        /// Initializes the 16 standard Community Chest cards.
        /// </summary>
        private void InitializeCommunityChestCards()
        {
            _cards.Add(Card.CreateCommunityChestCard("CC01", "Advance to Go (Collect $200)", CardActionType.Move, 0));
            _cards.Add(Card.CreateCommunityChestCard("CC02", "Bank error in your favor - Collect $200", CardActionType.CollectMoney, 200));
            _cards.Add(Card.CreateCommunityChestCard("CC03", "Doctor's fees - Pay $50", CardActionType.PayMoney, 50));
            _cards.Add(Card.CreateCommunityChestCard("CC04", "From sale of stock you get $50", CardActionType.CollectMoney, 50));
            _cards.Add(Card.CreateCommunityChestCard("CC05", "Get Out of Jail Free - This card may be kept until needed or sold", CardActionType.GetOutOfJailFree, 0, 0, true));
            _cards.Add(Card.CreateCommunityChestCard("CC06", "Go to Jail - Go directly to jail - Do not pass Go, do not collect $200", CardActionType.GoToJail, 0));
            _cards.Add(Card.CreateCommunityChestCard("CC07", "Grand Opera Night - Collect $50 from every player for opening night seats", CardActionType.CollectFromEachPlayer, 50));
            _cards.Add(Card.CreateCommunityChestCard("CC08", "Holiday Fund matures - Receive $100", CardActionType.CollectMoney, 100));
            _cards.Add(Card.CreateCommunityChestCard("CC09", "Income tax refund - Collect $20", CardActionType.CollectMoney, 20));
            _cards.Add(Card.CreateCommunityChestCard("CC10", "It is your birthday - Collect $10 from each player", CardActionType.CollectFromEachPlayer, 10));
            _cards.Add(Card.CreateCommunityChestCard("CC11", "Life insurance matures - Collect $100", CardActionType.CollectMoney, 100));
            _cards.Add(Card.CreateCommunityChestCard("CC12", "Pay hospital fees of $100", CardActionType.PayMoney, 100));
            _cards.Add(Card.CreateCommunityChestCard("CC13", "Pay school fees of $150", CardActionType.PayMoney, 150));
            _cards.Add(Card.CreateCommunityChestCard("CC14", "Receive $25 consultancy fee", CardActionType.CollectMoney, 25));
            _cards.Add(Card.CreateCommunityChestCard("CC15", "You are assessed for street repairs - $40 per house - $115 per hotel", CardActionType.PayPerHouse, 40, 115));
            _cards.Add(Card.CreateCommunityChestCard("CC16", "You have won second prize in a beauty contest - Collect $10", CardActionType.CollectMoney, 10));
        }
        
        /// <summary>
        /// Shuffles the deck and resets the draw pile.
        /// </summary>
        public void Shuffle()
        {
            _drawPile.Clear();
            
            // Fisher-Yates shuffle
            var shuffled = _cards.OrderBy(x => _random.Next()).ToList();
            
            foreach (var card in shuffled)
            {
                _drawPile.Enqueue(card);
            }
        }
        
        /// <summary>
        /// Draws the top card from the deck.
        /// </summary>
        /// <returns>The drawn card.</returns>
        public Card DrawCard()
        {
            if (_drawPile.Count == 0)
            {
                Shuffle();
            }
            
            return _drawPile.Dequeue();
        }
        
        /// <summary>
        /// Returns a card to the bottom of the deck.
        /// </summary>
        /// <param name="card">The card to return.</param>
        public void ReturnCard(Card card)
        {
            if (card == null)
                throw new ArgumentNullException(nameof(card));
            
            _drawPile.Enqueue(card);
        }
        
        /// <summary>
        /// Gets all cards in the deck (for inspection/testing).
        /// </summary>
        /// <returns>List of all cards in the deck.</returns>
        public List<Card> GetAllCards()
        {
            return new List<Card>(_cards);
        }
    }
}
