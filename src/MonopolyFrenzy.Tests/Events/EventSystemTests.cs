using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace MonopolyFrenzy.Tests.Events
{
    /// <summary>
    /// Test Suite 5: Event System Tests
    /// Tests for User Story 1.5: Event System
    /// 
    /// Validates:
    /// - Event bus implementation with publish/subscribe
    /// - Events defined for all game state changes
    /// - Events carry sufficient data for observers
    /// - Events are fired after state changes (not before)
    /// - Event subscriptions can be added/removed dynamically
    /// - Event system has negligible performance overhead (<0.1ms)
    /// - Events are thread-safe
    /// - Unit tests can observe and verify events
    /// </summary>
    [TestFixture]
    public class EventSystemTests
    {
        private IEventBus _eventBus;
        
        [SetUp]
        public void Setup()
        {
            _eventBus = new EventBus();
        }
        
        [TearDown]
        public void Teardown()
        {
            _eventBus = null;
        }
        
        #region Event Bus Basic Tests
        
        [Test]
        public void Subscribe_WithValidHandler_AddsSubscription()
        {
            // Arrange
            bool eventFired = false;
            Action<PlayerMovedEvent> handler = (e) => eventFired = true;
            
            // Act
            _eventBus.Subscribe(handler);
            _eventBus.Publish(new PlayerMovedEvent { PlayerId = "player1" });
            
            // Assert
            Assert.IsTrue(eventFired, "Event handler should be called");
        }
        
        [Test]
        public void Subscribe_MultipleHandlers_AllHandlersCalled()
        {
            // Arrange
            int callCount = 0;
            Action<PlayerMovedEvent> handler1 = (e) => callCount++;
            Action<PlayerMovedEvent> handler2 = (e) => callCount++;
            Action<PlayerMovedEvent> handler3 = (e) => callCount++;
            
            _eventBus.Subscribe(handler1);
            _eventBus.Subscribe(handler2);
            _eventBus.Subscribe(handler3);
            
            // Act
            _eventBus.Publish(new PlayerMovedEvent { PlayerId = "player1" });
            
            // Assert
            Assert.AreEqual(3, callCount, "All handlers should be called");
        }
        
        [Test]
        public void Unsubscribe_RemovesHandler()
        {
            // Arrange
            bool eventFired = false;
            Action<PlayerMovedEvent> handler = (e) => eventFired = true;
            _eventBus.Subscribe(handler);
            
            // Act
            _eventBus.Unsubscribe(handler);
            _eventBus.Publish(new PlayerMovedEvent { PlayerId = "player1" });
            
            // Assert
            Assert.IsFalse(eventFired, "Event handler should not be called after unsubscribe");
        }
        
        [Test]
        public void Publish_WithNoSubscribers_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _eventBus.Publish(new PlayerMovedEvent { PlayerId = "player1" }));
        }
        
        [Test]
        public void Subscribe_WithNullHandler_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _eventBus.Subscribe<PlayerMovedEvent>(null));
        }
        
        #endregion
        
        #region Game State Change Events Tests
        
        [Test]
        public void PlayerMovedEvent_ContainsPlayerIdAndPosition()
        {
            // Arrange
            PlayerMovedEvent receivedEvent = null;
            _eventBus.Subscribe<PlayerMovedEvent>(e => receivedEvent = e);
            
            // Act
            _eventBus.Publish(new PlayerMovedEvent 
            { 
                PlayerId = "player1", 
                FromPosition = 5, 
                ToPosition = 12,
                PassedGo = false
            });
            
            // Assert
            Assert.IsNotNull(receivedEvent);
            Assert.AreEqual("player1", receivedEvent.PlayerId);
            Assert.AreEqual(5, receivedEvent.FromPosition);
            Assert.AreEqual(12, receivedEvent.ToPosition);
            Assert.IsFalse(receivedEvent.PassedGo);
        }
        
        [Test]
        public void PropertyPurchasedEvent_ContainsAllNecessaryData()
        {
            // Arrange
            PropertyPurchasedEvent receivedEvent = null;
            _eventBus.Subscribe<PropertyPurchasedEvent>(e => receivedEvent = e);
            
            // Act
            _eventBus.Publish(new PropertyPurchasedEvent
            {
                PlayerId = "player1",
                PropertyName = "Boardwalk",
                Price = 400,
                PlayerMoneyRemaining = 1100
            });
            
            // Assert
            Assert.IsNotNull(receivedEvent);
            Assert.AreEqual("player1", receivedEvent.PlayerId);
            Assert.AreEqual("Boardwalk", receivedEvent.PropertyName);
            Assert.AreEqual(400, receivedEvent.Price);
            Assert.AreEqual(1100, receivedEvent.PlayerMoneyRemaining);
        }
        
        [Test]
        public void MoneyTransferredEvent_ContainsSenderAndReceiver()
        {
            // Arrange
            MoneyTransferredEvent receivedEvent = null;
            _eventBus.Subscribe<MoneyTransferredEvent>(e => receivedEvent = e);
            
            // Act
            _eventBus.Publish(new MoneyTransferredEvent
            {
                FromPlayerId = "player1",
                ToPlayerId = "player2",
                Amount = 50,
                Reason = "Rent payment"
            });
            
            // Assert
            Assert.IsNotNull(receivedEvent);
            Assert.AreEqual("player1", receivedEvent.FromPlayerId);
            Assert.AreEqual("player2", receivedEvent.ToPlayerId);
            Assert.AreEqual(50, receivedEvent.Amount);
            Assert.AreEqual("Rent payment", receivedEvent.Reason);
        }
        
        [Test]
        public void PlayerBankruptEvent_ContainsPlayerAndCreditor()
        {
            // Arrange
            PlayerBankruptEvent receivedEvent = null;
            _eventBus.Subscribe<PlayerBankruptEvent>(e => receivedEvent = e);
            
            // Act
            _eventBus.Publish(new PlayerBankruptEvent
            {
                PlayerId = "player1",
                CreditorId = "player2",
                TotalDebt = 500
            });
            
            // Assert
            Assert.IsNotNull(receivedEvent);
            Assert.AreEqual("player1", receivedEvent.PlayerId);
            Assert.AreEqual("player2", receivedEvent.CreditorId);
            Assert.AreEqual(500, receivedEvent.TotalDebt);
        }
        
        [Test]
        public void GameStartedEvent_ContainsPlayerList()
        {
            // Arrange
            GameStartedEvent receivedEvent = null;
            _eventBus.Subscribe<GameStartedEvent>(e => receivedEvent = e);
            
            // Act
            _eventBus.Publish(new GameStartedEvent
            {
                PlayerIds = new List<string> { "player1", "player2", "player3" },
                StartingPlayer = "player1"
            });
            
            // Assert
            Assert.IsNotNull(receivedEvent);
            Assert.AreEqual(3, receivedEvent.PlayerIds.Count);
            Assert.AreEqual("player1", receivedEvent.StartingPlayer);
        }
        
        [Test]
        public void GameOverEvent_ContainsWinner()
        {
            // Arrange
            GameOverEvent receivedEvent = null;
            _eventBus.Subscribe<GameOverEvent>(e => receivedEvent = e);
            
            // Act
            _eventBus.Publish(new GameOverEvent
            {
                WinnerId = "player1",
                TotalTurns = 150,
                GameDurationMinutes = 45
            });
            
            // Assert
            Assert.IsNotNull(receivedEvent);
            Assert.AreEqual("player1", receivedEvent.WinnerId);
            Assert.AreEqual(150, receivedEvent.TotalTurns);
            Assert.AreEqual(45, receivedEvent.GameDurationMinutes);
        }
        
        [Test]
        public void TurnStartedEvent_ContainsCurrentPlayer()
        {
            // Arrange
            TurnStartedEvent receivedEvent = null;
            _eventBus.Subscribe<TurnStartedEvent>(e => receivedEvent = e);
            
            // Act
            _eventBus.Publish(new TurnStartedEvent
            {
                PlayerId = "player1",
                TurnNumber = 5
            });
            
            // Assert
            Assert.IsNotNull(receivedEvent);
            Assert.AreEqual("player1", receivedEvent.PlayerId);
            Assert.AreEqual(5, receivedEvent.TurnNumber);
        }
        
        [Test]
        public void TurnEndedEvent_ContainsCurrentPlayer()
        {
            // Arrange
            TurnEndedEvent receivedEvent = null;
            _eventBus.Subscribe<TurnEndedEvent>(e => receivedEvent = e);
            
            // Act
            _eventBus.Publish(new TurnEndedEvent
            {
                PlayerId = "player1",
                TurnNumber = 5
            });
            
            // Assert
            Assert.IsNotNull(receivedEvent);
            Assert.AreEqual("player1", receivedEvent.PlayerId);
            Assert.AreEqual(5, receivedEvent.TurnNumber);
        }
        
        [Test]
        public void DiceRolledEvent_ContainsDiceValues()
        {
            // Arrange
            DiceRolledEvent receivedEvent = null;
            _eventBus.Subscribe<DiceRolledEvent>(e => receivedEvent = e);
            
            // Act
            _eventBus.Publish(new DiceRolledEvent
            {
                PlayerId = "player1",
                Dice1 = 3,
                Dice2 = 4,
                Total = 7,
                IsDoubles = false
            });
            
            // Assert
            Assert.IsNotNull(receivedEvent);
            Assert.AreEqual("player1", receivedEvent.PlayerId);
            Assert.AreEqual(3, receivedEvent.Dice1);
            Assert.AreEqual(4, receivedEvent.Dice2);
            Assert.AreEqual(7, receivedEvent.Total);
            Assert.IsFalse(receivedEvent.IsDoubles);
        }
        
        [Test]
        public void HousePurchasedEvent_ContainsPropertyAndCost()
        {
            // Arrange
            HousePurchasedEvent receivedEvent = null;
            _eventBus.Subscribe<HousePurchasedEvent>(e => receivedEvent = e);
            
            // Act
            _eventBus.Publish(new HousePurchasedEvent
            {
                PlayerId = "player1",
                PropertyName = "Mediterranean Avenue",
                Cost = 50,
                TotalHouses = 2
            });
            
            // Assert
            Assert.IsNotNull(receivedEvent);
            Assert.AreEqual("player1", receivedEvent.PlayerId);
            Assert.AreEqual("Mediterranean Avenue", receivedEvent.PropertyName);
            Assert.AreEqual(50, receivedEvent.Cost);
            Assert.AreEqual(2, receivedEvent.TotalHouses);
        }
        
        [Test]
        public void PropertyMortgagedEvent_ContainsPropertyAndValue()
        {
            // Arrange
            PropertyMortgagedEvent receivedEvent = null;
            _eventBus.Subscribe<PropertyMortgagedEvent>(e => receivedEvent = e);
            
            // Act
            _eventBus.Publish(new PropertyMortgagedEvent
            {
                PlayerId = "player1",
                PropertyName = "Boardwalk",
                MortgageValue = 200
            });
            
            // Assert
            Assert.IsNotNull(receivedEvent);
            Assert.AreEqual("player1", receivedEvent.PlayerId);
            Assert.AreEqual("Boardwalk", receivedEvent.PropertyName);
            Assert.AreEqual(200, receivedEvent.MortgageValue);
        }
        
        [Test]
        public void TradeExecutedEvent_ContainsTradeDetails()
        {
            // Arrange
            TradeExecutedEvent receivedEvent = null;
            _eventBus.Subscribe<TradeExecutedEvent>(e => receivedEvent = e);
            
            // Act
            _eventBus.Publish(new TradeExecutedEvent
            {
                Player1Id = "player1",
                Player2Id = "player2",
                Player1Properties = new List<string> { "Boardwalk" },
                Player2Properties = new List<string> { "Park Place" },
                Player1Money = 100,
                Player2Money = 50
            });
            
            // Assert
            Assert.IsNotNull(receivedEvent);
            Assert.AreEqual("player1", receivedEvent.Player1Id);
            Assert.AreEqual("player2", receivedEvent.Player2Id);
            Assert.AreEqual(1, receivedEvent.Player1Properties.Count);
            Assert.AreEqual(1, receivedEvent.Player2Properties.Count);
        }
        
        [Test]
        public void CardDrawnEvent_ContainsCardDetails()
        {
            // Arrange
            CardDrawnEvent receivedEvent = null;
            _eventBus.Subscribe<CardDrawnEvent>(e => receivedEvent = e);
            
            // Act
            _eventBus.Publish(new CardDrawnEvent
            {
                PlayerId = "player1",
                CardType = "Chance",
                CardText = "Advance to GO"
            });
            
            // Assert
            Assert.IsNotNull(receivedEvent);
            Assert.AreEqual("player1", receivedEvent.PlayerId);
            Assert.AreEqual("Chance", receivedEvent.CardType);
            Assert.AreEqual("Advance to GO", receivedEvent.CardText);
        }
        
        [Test]
        public void PlayerJailedEvent_ContainsJailReason()
        {
            // Arrange
            PlayerJailedEvent receivedEvent = null;
            _eventBus.Subscribe<PlayerJailedEvent>(e => receivedEvent = e);
            
            // Act
            _eventBus.Publish(new PlayerJailedEvent
            {
                PlayerId = "player1",
                Reason = "Landed on Go To Jail"
            });
            
            // Assert
            Assert.IsNotNull(receivedEvent);
            Assert.AreEqual("player1", receivedEvent.PlayerId);
            Assert.AreEqual("Landed on Go To Jail", receivedEvent.Reason);
        }
        
        #endregion
        
        #region Event Ordering Tests
        
        [Test]
        public void Events_AreFiredInOrder()
        {
            // Arrange
            var eventOrder = new List<string>();
            _eventBus.Subscribe<PlayerMovedEvent>(e => eventOrder.Add("Moved"));
            _eventBus.Subscribe<PropertyPurchasedEvent>(e => eventOrder.Add("Purchased"));
            _eventBus.Subscribe<TurnEndedEvent>(e => eventOrder.Add("TurnEnded"));
            
            // Act
            _eventBus.Publish(new PlayerMovedEvent { PlayerId = "player1" });
            _eventBus.Publish(new PropertyPurchasedEvent { PlayerId = "player1", PropertyName = "Test" });
            _eventBus.Publish(new TurnEndedEvent { PlayerId = "player1" });
            
            // Assert
            Assert.AreEqual(3, eventOrder.Count);
            Assert.AreEqual("Moved", eventOrder[0]);
            Assert.AreEqual("Purchased", eventOrder[1]);
            Assert.AreEqual("TurnEnded", eventOrder[2]);
        }
        
        [Test]
        public void Events_AreFiredAfterStateChanges()
        {
            // Arrange
            var gameState = new TestGameState();
            bool eventFiredBeforeChange = false;
            bool eventFiredAfterChange = false;
            
            _eventBus.Subscribe<PropertyPurchasedEvent>(e => 
            {
                if (gameState.PropertyOwner == null)
                    eventFiredBeforeChange = true;
                else
                    eventFiredAfterChange = true;
            });
            
            // Act
            gameState.PropertyOwner = "player1"; // State change first
            _eventBus.Publish(new PropertyPurchasedEvent { PlayerId = "player1", PropertyName = "Test" });
            
            // Assert
            Assert.IsFalse(eventFiredBeforeChange, "Event should not fire before state change");
            Assert.IsTrue(eventFiredAfterChange, "Event should fire after state change");
        }
        
        #endregion
        
        #region Dynamic Subscription Tests
        
        [Test]
        public void Subscribe_DuringEventHandling_WorksForNextEvent()
        {
            // Arrange
            int handler1Calls = 0;
            int handler2Calls = 0;
            
            Action<PlayerMovedEvent> handler2 = null;
            Action<PlayerMovedEvent> handler1 = e => 
            {
                handler1Calls++;
                if (handler2Calls == 0) // First time only
                {
                    handler2 = e2 => handler2Calls++;
                    _eventBus.Subscribe(handler2);
                }
            };
            
            _eventBus.Subscribe(handler1);
            
            // Act
            _eventBus.Publish(new PlayerMovedEvent { PlayerId = "player1" });
            _eventBus.Publish(new PlayerMovedEvent { PlayerId = "player1" });
            
            // Assert
            Assert.AreEqual(2, handler1Calls);
            Assert.AreEqual(1, handler2Calls, "Second handler should only be called for second event");
        }
        
        [Test]
        public void Unsubscribe_DuringEventHandling_DoesNotAffectCurrentEvent()
        {
            // Arrange
            int handler1Calls = 0;
            int handler2Calls = 0;
            
            Action<PlayerMovedEvent> handler1 = null;
            Action<PlayerMovedEvent> handler2 = e => 
            {
                handler2Calls++;
                if (handler1 != null)
                    _eventBus.Unsubscribe(handler1);
            };
            
            handler1 = e => handler1Calls++;
            
            _eventBus.Subscribe(handler2);
            _eventBus.Subscribe(handler1);
            
            // Act
            _eventBus.Publish(new PlayerMovedEvent { PlayerId = "player1" });
            _eventBus.Publish(new PlayerMovedEvent { PlayerId = "player1" });
            
            // Assert
            Assert.AreEqual(1, handler1Calls, "Handler1 should only be called once");
            Assert.AreEqual(2, handler2Calls, "Handler2 should be called both times");
        }
        
        #endregion
        
        #region Performance Tests
        
        [Test]
        public void PublishEvent_WithNoSubscribers_CompletesQuickly()
        {
            // Arrange
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            // Act
            for (int i = 0; i < 10000; i++)
            {
                _eventBus.Publish(new PlayerMovedEvent { PlayerId = "player1" });
            }
            stopwatch.Stop();
            
            // Assert
            var averageTimeMs = stopwatch.ElapsedMilliseconds / 10000.0;
            Assert.Less(averageTimeMs, 0.1, "Event publish with no subscribers should be <0.1ms");
        }
        
        [Test]
        public void PublishEvent_WithSubscribers_CompletesQuickly()
        {
            // Arrange
            int callCount = 0;
            for (int i = 0; i < 100; i++)
            {
                _eventBus.Subscribe<PlayerMovedEvent>(e => callCount++);
            }
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            // Act
            for (int i = 0; i < 1000; i++)
            {
                _eventBus.Publish(new PlayerMovedEvent { PlayerId = "player1" });
            }
            stopwatch.Stop();
            
            // Assert
            var averageTimeMs = stopwatch.ElapsedMilliseconds / 1000.0;
            Assert.Less(averageTimeMs, 0.1, "Event publish should be <0.1ms even with 100 subscribers");
        }
        
        #endregion
        
        #region Thread Safety Tests
        
        [Test]
        public void EventBus_ConcurrentSubscribeAndPublish_IsThreadSafe()
        {
            // Arrange
            int eventCount = 0;
            var lockObj = new object();
            
            // Act
            System.Threading.Tasks.Parallel.For(0, 100, i =>
            {
                _eventBus.Subscribe<PlayerMovedEvent>(e => 
                {
                    lock (lockObj) { eventCount++; }
                });
                
                _eventBus.Publish(new PlayerMovedEvent { PlayerId = $"player{i}" });
            });
            
            // Give time for all events to process
            System.Threading.Thread.Sleep(100);
            
            // Assert
            Assert.Greater(eventCount, 0, "Some events should have been processed");
        }
        
        #endregion
        
        #region Error Handling Tests
        
        [Test]
        public void PublishEvent_HandlerThrowsException_ContinuesWithOtherHandlers()
        {
            // Arrange
            bool handler1Called = false;
            bool handler2Called = false;
            bool handler3Called = false;
            
            _eventBus.Subscribe<PlayerMovedEvent>(e => { handler1Called = true; });
            _eventBus.Subscribe<PlayerMovedEvent>(e => { throw new Exception("Handler error"); });
            _eventBus.Subscribe<PlayerMovedEvent>(e => { handler3Called = true; });
            
            // Act
            _eventBus.Publish(new PlayerMovedEvent { PlayerId = "player1" });
            
            // Assert
            Assert.IsTrue(handler1Called, "Handler before exception should be called");
            Assert.IsTrue(handler3Called, "Handler after exception should still be called");
        }
        
        #endregion
    }
    
    #region Test Helper Classes
    
    // Event Bus interface and implementation placeholder
    public interface IEventBus
    {
        void Subscribe<T>(Action<T> handler) where T : class;
        void Unsubscribe<T>(Action<T> handler) where T : class;
        void Publish<T>(T eventData) where T : class;
    }
    
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _subscribers = new Dictionary<Type, List<Delegate>>();
        private readonly object _lock = new object();
        
        public void Subscribe<T>(Action<T> handler) where T : class
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            
            lock (_lock)
            {
                var eventType = typeof(T);
                if (!_subscribers.ContainsKey(eventType))
                {
                    _subscribers[eventType] = new List<Delegate>();
                }
                _subscribers[eventType].Add(handler);
            }
        }
        
        public void Unsubscribe<T>(Action<T> handler) where T : class
        {
            if (handler == null)
                return;
            
            lock (_lock)
            {
                var eventType = typeof(T);
                if (_subscribers.ContainsKey(eventType))
                {
                    _subscribers[eventType].Remove(handler);
                }
            }
        }
        
        public void Publish<T>(T eventData) where T : class
        {
            if (eventData == null)
                return;
            
            List<Delegate> handlersToCall;
            lock (_lock)
            {
                var eventType = typeof(T);
                if (!_subscribers.ContainsKey(eventType))
                    return;
                
                // Create a copy to avoid modification during iteration
                handlersToCall = new List<Delegate>(_subscribers[eventType]);
            }
            
            // Call handlers outside the lock
            foreach (var handler in handlersToCall)
            {
                try
                {
                    ((Action<T>)handler)(eventData);
                }
                catch
                {
                    // Continue with other handlers even if one throws
                }
            }
        }
    }
    
    // Event classes
    public class PlayerMovedEvent
    {
        public string PlayerId { get; set; }
        public int FromPosition { get; set; }
        public int ToPosition { get; set; }
        public bool PassedGo { get; set; }
    }
    
    public class PropertyPurchasedEvent
    {
        public string PlayerId { get; set; }
        public string PropertyName { get; set; }
        public int Price { get; set; }
        public int PlayerMoneyRemaining { get; set; }
    }
    
    public class MoneyTransferredEvent
    {
        public string FromPlayerId { get; set; }
        public string ToPlayerId { get; set; }
        public int Amount { get; set; }
        public string Reason { get; set; }
    }
    
    public class PlayerBankruptEvent
    {
        public string PlayerId { get; set; }
        public string CreditorId { get; set; }
        public int TotalDebt { get; set; }
    }
    
    public class GameStartedEvent
    {
        public List<string> PlayerIds { get; set; }
        public string StartingPlayer { get; set; }
    }
    
    public class GameOverEvent
    {
        public string WinnerId { get; set; }
        public int TotalTurns { get; set; }
        public int GameDurationMinutes { get; set; }
    }
    
    public class TurnStartedEvent
    {
        public string PlayerId { get; set; }
        public int TurnNumber { get; set; }
    }
    
    public class TurnEndedEvent
    {
        public string PlayerId { get; set; }
        public int TurnNumber { get; set; }
    }
    
    public class DiceRolledEvent
    {
        public string PlayerId { get; set; }
        public int Dice1 { get; set; }
        public int Dice2 { get; set; }
        public int Total { get; set; }
        public bool IsDoubles { get; set; }
    }
    
    public class HousePurchasedEvent
    {
        public string PlayerId { get; set; }
        public string PropertyName { get; set; }
        public int Cost { get; set; }
        public int TotalHouses { get; set; }
    }
    
    public class PropertyMortgagedEvent
    {
        public string PlayerId { get; set; }
        public string PropertyName { get; set; }
        public int MortgageValue { get; set; }
    }
    
    public class TradeExecutedEvent
    {
        public string Player1Id { get; set; }
        public string Player2Id { get; set; }
        public List<string> Player1Properties { get; set; }
        public List<string> Player2Properties { get; set; }
        public int Player1Money { get; set; }
        public int Player2Money { get; set; }
    }
    
    public class CardDrawnEvent
    {
        public string PlayerId { get; set; }
        public string CardType { get; set; }
        public string CardText { get; set; }
    }
    
    public class PlayerJailedEvent
    {
        public string PlayerId { get; set; }
        public string Reason { get; set; }
    }
    
    // Test helper class
    public class TestGameState
    {
        public string PropertyOwner { get; set; }
    }
    
    #endregion
}
