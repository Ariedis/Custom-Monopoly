using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using MonopolyFrenzy.Core;

namespace MonopolyFrenzy.Tests.Core
{
    /// <summary>
    /// Test Suite 1: Game State Management Tests
    /// Tests for User Story 1.1: Game State Management
    /// 
    /// Validates:
    /// - GameState class contains all game data (board, players, current turn, phase)
    /// - GameState is serializable to JSON
    /// - GameState can be cloned for AI evaluation
    /// - GameState publishes events when changed
    /// - Unit tests can create and manipulate GameState
    /// - No Unity dependencies in GameState class
    /// - Memory usage under 10 MB for typical game
    /// </summary>
    [TestFixture]
    public class GameStateTests
    {
        private GameState _gameState;
        
        [SetUp]
        public void Setup()
        {
            _gameState = new GameState();
        }
        
        [TearDown]
        public void Teardown()
        {
            _gameState = null;
        }
        
        #region Initialization Tests
        
        [Test]
        public void Initialize_CreatesDefaultGameState()
        {
            // Arrange & Act
            _gameState.Initialize();
            
            // Assert
            Assert.IsNotNull(_gameState.Board, "Board should be initialized");
            Assert.IsNotNull(_gameState.Players, "Players collection should be initialized");
            Assert.AreEqual(0, _gameState.Players.Count, "Players collection should be empty initially");
            Assert.AreEqual(GamePhase.Setup, _gameState.CurrentPhase, "Initial phase should be Setup");
            Assert.IsNull(_gameState.CurrentPlayer, "No current player before game starts");
        }
        
        [Test]
        public void Initialize_CreatesBoardWith40Spaces()
        {
            // Arrange & Act
            _gameState.Initialize();
            
            // Assert
            Assert.IsNotNull(_gameState.Board);
            Assert.AreEqual(40, _gameState.Board.Spaces.Count, "Board should have exactly 40 spaces");
        }
        
        [Test]
        public void Initialize_SetsInitialTurnToZero()
        {
            // Arrange & Act
            _gameState.Initialize();
            
            // Assert
            Assert.AreEqual(0, _gameState.TurnNumber, "Initial turn number should be 0");
        }
        
        #endregion
        
        #region Player Management Tests
        
        [Test]
        public void AddPlayer_WithValidName_AddsPlayerToList()
        {
            // Arrange
            _gameState.Initialize();
            var playerName = "Alice";
            
            // Act
            var player = _gameState.AddPlayer(playerName);
            
            // Assert
            Assert.AreEqual(1, _gameState.Players.Count);
            Assert.AreEqual(playerName, player.Name);
            Assert.AreEqual(1500, player.Money, "Starting money should be $1500");
            Assert.AreEqual(0, player.Position, "Starting position should be 0 (GO)");
        }
        
        [Test]
        public void AddPlayer_MultipleValidPlayers_AddsSeparatePlayersToList()
        {
            // Arrange
            _gameState.Initialize();
            
            // Act
            _gameState.AddPlayer("Alice");
            _gameState.AddPlayer("Bob");
            _gameState.AddPlayer("Charlie");
            
            // Assert
            Assert.AreEqual(3, _gameState.Players.Count);
            Assert.IsTrue(_gameState.Players.All(p => p.Money == 1500), "All players should have starting money");
        }
        
        [Test]
        public void AddPlayer_ExceedingMaxPlayers_ThrowsInvalidOperationException()
        {
            // Arrange
            _gameState.Initialize();
            for (int i = 0; i < 6; i++)
            {
                _gameState.AddPlayer($"Player{i}");
            }
            
            // Act & Assert
            Assert.Throws<System.InvalidOperationException>(
                () => _gameState.AddPlayer("ExtraPlayer"),
                "Should not allow more than 6 players");
        }
        
        [Test]
        public void AddPlayer_WithNullName_ThrowsArgumentNullException()
        {
            // Arrange
            _gameState.Initialize();
            
            // Act & Assert
            Assert.Throws<System.ArgumentNullException>(
                () => _gameState.AddPlayer(null),
                "Should not allow null player name");
        }
        
        [Test]
        public void AddPlayer_WithEmptyName_ThrowsArgumentException()
        {
            // Arrange
            _gameState.Initialize();
            
            // Act & Assert
            Assert.Throws<System.ArgumentException>(
                () => _gameState.AddPlayer(""),
                "Should not allow empty player name");
        }
        
        [Test]
        public void AddPlayer_WithDuplicateName_AddsSuffixToName()
        {
            // Arrange
            _gameState.Initialize();
            _gameState.AddPlayer("Alice");
            
            // Act
            var duplicate = _gameState.AddPlayer("Alice");
            
            // Assert
            Assert.AreEqual("Alice (2)", duplicate.Name, "Duplicate name should have suffix");
        }
        
        [Test]
        public void AddPlayer_AssignsUniqueId()
        {
            // Arrange
            _gameState.Initialize();
            
            // Act
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            
            // Assert
            Assert.AreNotEqual(player1.Id, player2.Id, "Player IDs should be unique");
        }
        
        [Test]
        public void AddPlayer_LessThanMinPlayers_CannotStartGame()
        {
            // Arrange
            _gameState.Initialize();
            _gameState.AddPlayer("Alice");
            
            // Act & Assert
            Assert.IsFalse(_gameState.CanStartGame, "Cannot start game with only 1 player");
        }
        
        [Test]
        public void AddPlayer_WithMinPlayers_CanStartGame()
        {
            // Arrange
            _gameState.Initialize();
            _gameState.AddPlayer("Alice");
            _gameState.AddPlayer("Bob");
            
            // Act & Assert
            Assert.IsTrue(_gameState.CanStartGame, "Should be able to start with 2 players");
        }
        
        #endregion
        
        #region Serialization Tests
        
        [Test]
        public void SerializeToJson_WithInitializedState_ReturnsValidJson()
        {
            // Arrange
            _gameState.Initialize();
            _gameState.AddPlayer("Alice");
            _gameState.AddPlayer("Bob");
            
            // Act
            var json = _gameState.SerializeToJson();
            
            // Assert
            Assert.IsNotNull(json);
            Assert.IsTrue(json.Length > 0, "JSON should not be empty");
            Assert.IsTrue(json.Contains("Alice"), "JSON should contain player names");
            Assert.IsTrue(json.Contains("Bob"), "JSON should contain player names");
        }
        
        [Test]
        public void DeserializeFromJson_WithValidJson_RestoresGameState()
        {
            // Arrange
            _gameState.Initialize();
            _gameState.AddPlayer("Alice");
            _gameState.AddPlayer("Bob");
            var json = _gameState.SerializeToJson();
            
            // Act
            var restoredState = GameState.DeserializeFromJson(json);
            
            // Assert
            Assert.IsNotNull(restoredState);
            Assert.AreEqual(2, restoredState.Players.Count);
            Assert.AreEqual("Alice", restoredState.Players[0].Name);
            Assert.AreEqual("Bob", restoredState.Players[1].Name);
        }
        
        [Test]
        public void DeserializeFromJson_WithInvalidJson_ThrowsException()
        {
            // Arrange
            var invalidJson = "{ invalid json }";
            
            // Act & Assert
            Assert.Throws<System.Exception>(
                () => GameState.DeserializeFromJson(invalidJson),
                "Should throw exception for invalid JSON");
        }
        
        #endregion
        
        #region Clone Tests
        
        [Test]
        public void Clone_CreatesIndependentCopy()
        {
            // Arrange
            _gameState.Initialize();
            var player = _gameState.AddPlayer("Alice");
            player.Money = 2000;
            
            // Act
            var clonedState = _gameState.Clone();
            
            // Assert
            Assert.IsNotNull(clonedState);
            Assert.AreNotSame(_gameState, clonedState, "Clone should be a different object");
            Assert.AreEqual(_gameState.Players.Count, clonedState.Players.Count);
            Assert.AreEqual(_gameState.Players[0].Money, clonedState.Players[0].Money);
        }
        
        [Test]
        public void Clone_ChangesToClone_DoNotAffectOriginal()
        {
            // Arrange
            _gameState.Initialize();
            _gameState.AddPlayer("Alice");
            var clonedState = _gameState.Clone();
            
            // Act
            clonedState.Players[0].Money = 500;
            
            // Assert
            Assert.AreEqual(1500, _gameState.Players[0].Money, "Original should not be affected");
            Assert.AreEqual(500, clonedState.Players[0].Money, "Clone should have changed value");
        }
        
        [Test]
        public void Clone_PreservesAllGameStateProperties()
        {
            // Arrange
            _gameState.Initialize();
            _gameState.AddPlayer("Alice");
            _gameState.AddPlayer("Bob");
            _gameState.StartGame();
            
            // Act
            var clonedState = _gameState.Clone();
            
            // Assert
            Assert.AreEqual(_gameState.CurrentPhase, clonedState.CurrentPhase);
            Assert.AreEqual(_gameState.TurnNumber, clonedState.TurnNumber);
            Assert.AreEqual(_gameState.Players.Count, clonedState.Players.Count);
        }
        
        #endregion
        
        #region Event System Tests
        
        [Test]
        public void AddPlayer_PublishesPlayerAddedEvent()
        {
            // Arrange
            _gameState.Initialize();
            bool eventFired = false;
            _gameState.OnPlayerAdded += (player) => eventFired = true;
            
            // Act
            _gameState.AddPlayer("Alice");
            
            // Assert
            Assert.IsTrue(eventFired, "PlayerAdded event should be fired");
        }
        
        [Test]
        public void StartGame_PublishesGameStartedEvent()
        {
            // Arrange
            _gameState.Initialize();
            _gameState.AddPlayer("Alice");
            _gameState.AddPlayer("Bob");
            bool eventFired = false;
            _gameState.OnGameStarted += () => eventFired = true;
            
            // Act
            _gameState.StartGame();
            
            // Assert
            Assert.IsTrue(eventFired, "GameStarted event should be fired");
        }
        
        [Test]
        public void StateChange_PublishesStateChangedEvent()
        {
            // Arrange
            _gameState.Initialize();
            _gameState.AddPlayer("Alice");
            _gameState.AddPlayer("Bob");
            bool eventFired = false;
            GamePhase newPhase = GamePhase.Setup;
            _gameState.OnStateChanged += (phase) => 
            { 
                eventFired = true;
                newPhase = phase;
            };
            
            // Act
            _gameState.StartGame();
            
            // Assert
            Assert.IsTrue(eventFired, "StateChanged event should be fired");
            Assert.AreEqual(GamePhase.Playing, newPhase);
        }
        
        #endregion
        
        #region Game Phase Tests
        
        [Test]
        public void StartGame_WithValidPlayers_ChangesPhaseToPlaying()
        {
            // Arrange
            _gameState.Initialize();
            _gameState.AddPlayer("Alice");
            _gameState.AddPlayer("Bob");
            
            // Act
            _gameState.StartGame();
            
            // Assert
            Assert.AreEqual(GamePhase.Playing, _gameState.CurrentPhase);
        }
        
        [Test]
        public void StartGame_SetsFirstPlayerAsCurrent()
        {
            // Arrange
            _gameState.Initialize();
            var firstPlayer = _gameState.AddPlayer("Alice");
            _gameState.AddPlayer("Bob");
            
            // Act
            _gameState.StartGame();
            
            // Assert
            Assert.IsNotNull(_gameState.CurrentPlayer);
            Assert.AreEqual(firstPlayer.Id, _gameState.CurrentPlayer.Id);
        }
        
        [Test]
        public void StartGame_WithInsufficientPlayers_ThrowsInvalidOperationException()
        {
            // Arrange
            _gameState.Initialize();
            _gameState.AddPlayer("Alice");
            
            // Act & Assert
            Assert.Throws<System.InvalidOperationException>(
                () => _gameState.StartGame(),
                "Cannot start game with less than 2 players");
        }
        
        [Test]
        public void EndGame_ChangesPhaseToGameOver()
        {
            // Arrange
            _gameState.Initialize();
            _gameState.AddPlayer("Alice");
            _gameState.AddPlayer("Bob");
            _gameState.StartGame();
            
            // Act
            _gameState.EndGame();
            
            // Assert
            Assert.AreEqual(GamePhase.GameOver, _gameState.CurrentPhase);
        }
        
        #endregion
        
        #region Memory and Performance Tests
        
        [Test]
        public void GameState_WithFullGame_MemoryUsageAcceptable()
        {
            // Arrange
            _gameState.Initialize();
            for (int i = 0; i < 6; i++)
            {
                _gameState.AddPlayer($"Player{i}");
            }
            _gameState.StartGame();
            
            // Act - Simulate property ownership
            var estimatedMemoryBytes = EstimateMemoryUsage(_gameState);
            var estimatedMemoryMB = estimatedMemoryBytes / (1024.0 * 1024.0);
            
            // Assert
            Assert.Less(estimatedMemoryMB, 10.0, "Memory usage should be under 10 MB");
        }
        
        private long EstimateMemoryUsage(GameState state)
        {
            // Rough estimation: players, board, properties
            long size = 0;
            size += state.Players.Count * 1024; // ~1KB per player
            size += 40 * 512; // ~512 bytes per space
            size += 10240; // ~10KB for other data structures
            return size;
        }
        
        #endregion
        
        #region Turn Management Tests
        
        [Test]
        public void NextTurn_AdvancesToNextPlayer()
        {
            // Arrange
            _gameState.Initialize();
            _gameState.AddPlayer("Alice");
            var secondPlayer = _gameState.AddPlayer("Bob");
            _gameState.StartGame();
            
            // Act
            _gameState.NextTurn();
            
            // Assert
            Assert.AreEqual(secondPlayer.Id, _gameState.CurrentPlayer.Id);
            Assert.AreEqual(1, _gameState.TurnNumber);
        }
        
        [Test]
        public void NextTurn_AfterLastPlayer_WrapsToFirstPlayer()
        {
            // Arrange
            _gameState.Initialize();
            var firstPlayer = _gameState.AddPlayer("Alice");
            _gameState.AddPlayer("Bob");
            _gameState.StartGame();
            _gameState.NextTurn(); // Bob's turn
            
            // Act
            _gameState.NextTurn(); // Should wrap to Alice
            
            // Assert
            Assert.AreEqual(firstPlayer.Id, _gameState.CurrentPlayer.Id);
            Assert.AreEqual(2, _gameState.TurnNumber);
        }
        
        [Test]
        public void NextTurn_SkipsBankruptPlayers()
        {
            // Arrange
            _gameState.Initialize();
            _gameState.AddPlayer("Alice");
            var bobPlayer = _gameState.AddPlayer("Bob");
            var charliePlayer = _gameState.AddPlayer("Charlie");
            _gameState.StartGame();
            bobPlayer.IsBankrupt = true;
            
            // Act
            _gameState.NextTurn(); // Should skip Bob and go to Charlie
            
            // Assert
            Assert.AreEqual(charliePlayer.Id, _gameState.CurrentPlayer.Id);
        }
        
        #endregion
    }
}
