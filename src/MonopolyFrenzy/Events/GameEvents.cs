using System;

namespace MonopolyFrenzy.Events
{
    /// <summary>
    /// Event fired when a player moves on the board.
    /// </summary>
    public class PlayerMovedEvent
    {
        public string PlayerId { get; set; }
        public int FromPosition { get; set; }
        public int ToPosition { get; set; }
        public bool PassedGo { get; set; }
    }
    
    /// <summary>
    /// Event fired when a property is purchased.
    /// </summary>
    public class PropertyPurchasedEvent
    {
        public string PlayerId { get; set; }
        public string PropertyName { get; set; }
        public int Price { get; set; }
        public int PlayerMoneyRemaining { get; set; }
    }
    
    /// <summary>
    /// Event fired when money is transferred between players or bank.
    /// </summary>
    public class MoneyTransferredEvent
    {
        public string FromPlayerId { get; set; }
        public string ToPlayerId { get; set; }
        public int Amount { get; set; }
        public string Reason { get; set; }
    }
    
    /// <summary>
    /// Event fired when a player goes bankrupt.
    /// </summary>
    public class PlayerBankruptEvent
    {
        public string PlayerId { get; set; }
        public string CreditorId { get; set; }
        public int AssetsTransferred { get; set; }
    }
    
    /// <summary>
    /// Event fired when the game starts.
    /// </summary>
    public class GameStartedEvent
    {
        public int PlayerCount { get; set; }
        public string[] PlayerIds { get; set; }
    }
    
    /// <summary>
    /// Event fired when the game ends.
    /// </summary>
    public class GameOverEvent
    {
        public string WinnerId { get; set; }
        public string WinnerName { get; set; }
        public int TotalTurns { get; set; }
    }
    
    /// <summary>
    /// Event fired when a turn starts.
    /// </summary>
    public class TurnStartedEvent
    {
        public string PlayerId { get; set; }
        public int TurnNumber { get; set; }
    }
    
    /// <summary>
    /// Event fired when a turn ends.
    /// </summary>
    public class TurnEndedEvent
    {
        public string PlayerId { get; set; }
        public int TurnNumber { get; set; }
    }
    
    /// <summary>
    /// Event fired when dice are rolled.
    /// </summary>
    public class DiceRolledEvent
    {
        public string PlayerId { get; set; }
        public int Die1 { get; set; }
        public int Die2 { get; set; }
        public int Total { get; set; }
        public bool IsDoubles { get; set; }
    }
    
    /// <summary>
    /// Event fired when a house is purchased.
    /// </summary>
    public class HousePurchasedEvent
    {
        public string PlayerId { get; set; }
        public string PropertyName { get; set; }
        public int HouseCount { get; set; }
        public bool IsHotel { get; set; }
        public int Cost { get; set; }
    }
    
    /// <summary>
    /// Event fired when a property is mortgaged.
    /// </summary>
    public class PropertyMortgagedEvent
    {
        public string PlayerId { get; set; }
        public string PropertyName { get; set; }
        public int MortgageValue { get; set; }
    }
    
    /// <summary>
    /// Event fired when a property is unmortgaged.
    /// </summary>
    public class PropertyUnmortgagedEvent
    {
        public string PlayerId { get; set; }
        public string PropertyName { get; set; }
        public int Cost { get; set; }
    }
    
    /// <summary>
    /// Event fired when a trade is executed.
    /// </summary>
    public class TradeExecutedEvent
    {
        public string Player1Id { get; set; }
        public string Player2Id { get; set; }
        public string[] Player1Properties { get; set; }
        public string[] Player2Properties { get; set; }
        public int Player1Money { get; set; }
        public int Player2Money { get; set; }
    }
    
    /// <summary>
    /// Event fired when a card is drawn.
    /// </summary>
    public class CardDrawnEvent
    {
        public string PlayerId { get; set; }
        public string CardType { get; set; }
        public string CardText { get; set; }
    }
    
    /// <summary>
    /// Event fired when a player is sent to jail.
    /// </summary>
    public class PlayerJailedEvent
    {
        public string PlayerId { get; set; }
        public string Reason { get; set; }
    }
    
    /// <summary>
    /// Event fired when a player is released from jail.
    /// </summary>
    public class PlayerReleasedFromJailEvent
    {
        public string PlayerId { get; set; }
        public string Method { get; set; }
    }
}
