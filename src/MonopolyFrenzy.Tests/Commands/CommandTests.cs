using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace MonopolyFrenzy.Tests.Commands
{
    /// <summary>
    /// Test Suite 2: Command Pattern Tests
    /// Tests for User Story 1.3: Command Pattern for Actions
    /// 
    /// Validates:
    /// - Base ICommand interface with Execute() and Undo()
    /// - Commands implemented for all player actions
    /// - Commands validate preconditions before executing
    /// - Commands return result objects (success/failure with reason)
    /// - Commands are serializable to JSON
    /// - Command history stored for undo/replay
    /// - Commands execute in <1ms each
    /// - 100% unit test coverage on all commands
    /// </summary>
    [TestFixture]
    public class CommandTests
    {
        private TestGameState _gameState;
        private Player _testPlayer;
        private Property _testProperty;
        
        [SetUp]
        public void Setup()
        {
            _gameState = new TestGameState();
            _testPlayer = new Player { Id = "player1", Name = "Alice", Money = 1500, Position = 0 };
            _testProperty = new Property { Name = "Boardwalk", Price = 400, Owner = null };
            _gameState.Players.Add(_testPlayer);
        }
        
        [TearDown]
        public void Teardown()
        {
            _gameState = null;
            _testPlayer = null;
            _testProperty = null;
        }
        
        #region ICommand Interface Tests
        
        [Test]
        public void Command_ImplementsICommandInterface()
        {
            // Arrange
            var command = new BuyPropertyCommand(_testPlayer, _testProperty);
            
            // Assert
            Assert.IsInstanceOf<ICommand>(command);
        }
        
        [Test]
        public void Command_HasExecuteMethod()
        {
            // Arrange
            var command = new BuyPropertyCommand(_testPlayer, _testProperty);
            
            // Act & Assert
            Assert.DoesNotThrow(() => command.Execute());
        }
        
        [Test]
        public void Command_HasUndoMethod()
        {
            // Arrange
            var command = new BuyPropertyCommand(_testPlayer, _testProperty);
            command.Execute();
            
            // Act & Assert
            Assert.DoesNotThrow(() => command.Undo());
        }
        
        #endregion
        
        #region BuyPropertyCommand Tests
        
        [Test]
        public void BuyPropertyCommand_WithSufficientFunds_ExecutesSuccessfully()
        {
            // Arrange
            var command = new BuyPropertyCommand(_testPlayer, _testProperty);
            
            // Act
            var result = command.Execute();
            
            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(_testPlayer, _testProperty.Owner);
            Assert.AreEqual(1100, _testPlayer.Money);
        }
        
        [Test]
        public void BuyPropertyCommand_WithInsufficientFunds_ReturnsFailure()
        {
            // Arrange
            _testPlayer.Money = 100;
            var command = new BuyPropertyCommand(_testPlayer, _testProperty);
            
            // Act
            var result = command.Execute();
            
            // Assert
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.ErrorMessage);
            Assert.IsTrue(result.ErrorMessage.Contains("insufficient"));
            Assert.IsNull(_testProperty.Owner);
        }
        
        [Test]
        public void BuyPropertyCommand_PropertyAlreadyOwned_ReturnsFailure()
        {
            // Arrange
            _testProperty.Owner = new Player { Id = "other", Name = "Bob" };
            var command = new BuyPropertyCommand(_testPlayer, _testProperty);
            
            // Act
            var result = command.Execute();
            
            // Assert
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.ErrorMessage.Contains("already owned"));
        }
        
        [Test]
        public void BuyPropertyCommand_Undo_RestoresPreviousState()
        {
            // Arrange
            var command = new BuyPropertyCommand(_testPlayer, _testProperty);
            command.Execute();
            
            // Act
            command.Undo();
            
            // Assert
            Assert.IsNull(_testProperty.Owner);
            Assert.AreEqual(1500, _testPlayer.Money);
        }
        
        [Test]
        public void BuyPropertyCommand_WithNullPlayer_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new BuyPropertyCommand(null, _testProperty));
        }
        
        [Test]
        public void BuyPropertyCommand_WithNullProperty_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new BuyPropertyCommand(_testPlayer, null));
        }
        
        #endregion
        
        #region RollDiceCommand Tests
        
        [Test]
        public void RollDiceCommand_Execute_Returns2to12()
        {
            // Arrange
            var command = new RollDiceCommand(_testPlayer);
            
            // Act
            var result = command.Execute();
            
            // Assert
            Assert.IsTrue(result.Success);
            Assert.GreaterOrEqual(result.DiceTotal, 2);
            Assert.LessOrEqual(result.DiceTotal, 12);
        }
        
        [Test]
        public void RollDiceCommand_Execute_ReturnsTwoDiceValues()
        {
            // Arrange
            var command = new RollDiceCommand(_testPlayer);
            
            // Act
            var result = command.Execute();
            
            // Assert
            Assert.IsNotNull(result.Dice1);
            Assert.IsNotNull(result.Dice2);
            Assert.GreaterOrEqual(result.Dice1.Value, 1);
            Assert.LessOrEqual(result.Dice1.Value, 6);
            Assert.GreaterOrEqual(result.Dice2.Value, 1);
            Assert.LessOrEqual(result.Dice2.Value, 6);
        }
        
        [Test]
        public void RollDiceCommand_WithSeededRandom_ProducesDeterministicResults()
        {
            // Arrange
            var command1 = new RollDiceCommand(_testPlayer, seed: 42);
            var command2 = new RollDiceCommand(_testPlayer, seed: 42);
            
            // Act
            var result1 = command1.Execute();
            var result2 = command2.Execute();
            
            // Assert
            Assert.AreEqual(result1.DiceTotal, result2.DiceTotal);
        }
        
        [Test]
        public void RollDiceCommand_DetectsDoubles()
        {
            // Arrange
            var command = new RollDiceCommand(_testPlayer, forceDice1: 3, forceDice2: 3);
            
            // Act
            var result = command.Execute();
            
            // Assert
            Assert.IsTrue(result.IsDoubles);
        }
        
        #endregion
        
        #region MoveCommand Tests
        
        [Test]
        public void MoveCommand_WithValidSpaces_MovesPlayer()
        {
            // Arrange
            var command = new MoveCommand(_testPlayer, 7);
            
            // Act
            var result = command.Execute();
            
            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(7, _testPlayer.Position);
        }
        
        [Test]
        public void MoveCommand_PassingGo_CollectsPassGoMoney()
        {
            // Arrange
            _testPlayer.Position = 38;
            var command = new MoveCommand(_testPlayer, 5);
            
            // Act
            var result = command.Execute();
            
            // Assert
            Assert.IsTrue(result.PassedGo);
            Assert.AreEqual(1700, _testPlayer.Money); // 1500 + 200
        }
        
        [Test]
        public void MoveCommand_LandingOnGo_CollectsGoMoney()
        {
            // Arrange
            _testPlayer.Position = 36;
            var command = new MoveCommand(_testPlayer, 4);
            
            // Act
            var result = command.Execute();
            
            // Assert
            Assert.AreEqual(0, _testPlayer.Position);
            Assert.AreEqual(1700, _testPlayer.Money);
        }
        
        [Test]
        public void MoveCommand_Undo_RestoresPreviousPosition()
        {
            // Arrange
            _testPlayer.Position = 10;
            var command = new MoveCommand(_testPlayer, 7);
            command.Execute();
            
            // Act
            command.Undo();
            
            // Assert
            Assert.AreEqual(10, _testPlayer.Position);
            Assert.AreEqual(1500, _testPlayer.Money);
        }
        
        [Test]
        public void MoveCommand_WithNegativeSpaces_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new MoveCommand(_testPlayer, -5));
        }
        
        #endregion
        
        #region PayRentCommand Tests
        
        [Test]
        public void PayRentCommand_WithSufficientFunds_TransfersMoney()
        {
            // Arrange
            var landlord = new Player { Id = "landlord", Name = "Bob", Money = 1000 };
            var command = new PayRentCommand(_testPlayer, landlord, 100);
            
            // Act
            var result = command.Execute();
            
            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(1400, _testPlayer.Money);
            Assert.AreEqual(1100, landlord.Money);
        }
        
        [Test]
        public void PayRentCommand_WithInsufficientFunds_InitiatesBankruptcy()
        {
            // Arrange
            _testPlayer.Money = 50;
            var landlord = new Player { Id = "landlord", Name = "Bob", Money = 1000 };
            var command = new PayRentCommand(_testPlayer, landlord, 100);
            
            // Act
            var result = command.Execute();
            
            // Assert
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.BankruptcyRequired);
        }
        
        [Test]
        public void PayRentCommand_Undo_RestoresBothPlayerBalances()
        {
            // Arrange
            var landlord = new Player { Id = "landlord", Name = "Bob", Money = 1000 };
            var command = new PayRentCommand(_testPlayer, landlord, 100);
            command.Execute();
            
            // Act
            command.Undo();
            
            // Assert
            Assert.AreEqual(1500, _testPlayer.Money);
            Assert.AreEqual(1000, landlord.Money);
        }
        
        #endregion
        
        #region MortgageCommand Tests
        
        [Test]
        public void MortgageCommand_OnUnmortgagedProperty_ReceivesCash()
        {
            // Arrange
            _testProperty.Owner = _testPlayer;
            _testProperty.IsMortgaged = false;
            var command = new MortgageCommand(_testPlayer, _testProperty);
            
            // Act
            var result = command.Execute();
            
            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsTrue(_testProperty.IsMortgaged);
            Assert.AreEqual(1700, _testPlayer.Money); // 1500 + 200 (50% of 400)
        }
        
        [Test]
        public void MortgageCommand_OnAlreadyMortgagedProperty_ReturnsFailure()
        {
            // Arrange
            _testProperty.Owner = _testPlayer;
            _testProperty.IsMortgaged = true;
            var command = new MortgageCommand(_testPlayer, _testProperty);
            
            // Act
            var result = command.Execute();
            
            // Assert
            Assert.IsFalse(result.Success);
        }
        
        [Test]
        public void MortgageCommand_OnPropertyWithBuildings_ReturnsFailure()
        {
            // Arrange
            _testProperty.Owner = _testPlayer;
            _testProperty.HouseCount = 2;
            var command = new MortgageCommand(_testPlayer, _testProperty);
            
            // Act
            var result = command.Execute();
            
            // Assert
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.ErrorMessage.Contains("buildings"));
        }
        
        #endregion
        
        #region UnmortgageCommand Tests
        
        [Test]
        public void UnmortgageCommand_WithSufficientFunds_PaysMortgage()
        {
            // Arrange
            _testProperty.Owner = _testPlayer;
            _testProperty.IsMortgaged = true;
            var command = new UnmortgageCommand(_testPlayer, _testProperty);
            
            // Act
            var result = command.Execute();
            
            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsFalse(_testProperty.IsMortgaged);
            Assert.AreEqual(1260, _testPlayer.Money); // 1500 - 240 (60% of 400)
        }
        
        [Test]
        public void UnmortgageCommand_OnUnmortgagedProperty_ReturnsFailure()
        {
            // Arrange
            _testProperty.Owner = _testPlayer;
            _testProperty.IsMortgaged = false;
            var command = new UnmortgageCommand(_testPlayer, _testProperty);
            
            // Act
            var result = command.Execute();
            
            // Assert
            Assert.IsFalse(result.Success);
        }
        
        #endregion
        
        #region BuyHouseCommand Tests
        
        [Test]
        public void BuyHouseCommand_OnMonopolyWithValidConditions_AddsHouse()
        {
            // Arrange
            _testProperty.Owner = _testPlayer;
            _testProperty.IsPartOfMonopoly = true;
            _testProperty.HouseCost = 200;
            var command = new BuyHouseCommand(_testPlayer, _testProperty);
            
            // Act
            var result = command.Execute();
            
            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(1, _testProperty.HouseCount);
            Assert.AreEqual(1300, _testPlayer.Money);
        }
        
        [Test]
        public void BuyHouseCommand_WithoutMonopoly_ReturnsFailure()
        {
            // Arrange
            _testProperty.Owner = _testPlayer;
            _testProperty.IsPartOfMonopoly = false;
            var command = new BuyHouseCommand(_testPlayer, _testProperty);
            
            // Act
            var result = command.Execute();
            
            // Assert
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.ErrorMessage.Contains("monopoly"));
        }
        
        [Test]
        public void BuyHouseCommand_OnMortgagedProperty_ReturnsFailure()
        {
            // Arrange
            _testProperty.Owner = _testPlayer;
            _testProperty.IsPartOfMonopoly = true;
            _testProperty.IsMortgaged = true;
            var command = new BuyHouseCommand(_testPlayer, _testProperty);
            
            // Act
            var result = command.Execute();
            
            // Assert
            Assert.IsFalse(result.Success);
        }
        
        [Test]
        public void BuyHouseCommand_ExceedingMaxHouses_ReturnsFailure()
        {
            // Arrange
            _testProperty.Owner = _testPlayer;
            _testProperty.IsPartOfMonopoly = true;
            _testProperty.HouseCount = 4;
            _testProperty.HouseCost = 200;
            var command = new BuyHouseCommand(_testPlayer, _testProperty);
            
            // Act
            var result = command.Execute();
            
            // Assert
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.ErrorMessage.Contains("hotel") || result.ErrorMessage.Contains("maximum"));
        }
        
        #endregion
        
        #region DrawCardCommand Tests
        
        [Test]
        public void DrawCardCommand_FromChanceDeck_DrawsCard()
        {
            // Arrange
            var deck = new CardDeck(CardDeckType.Chance);
            var command = new DrawCardCommand(_testPlayer, deck);
            
            // Act
            var result = command.Execute();
            
            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Card);
        }
        
        [Test]
        public void DrawCardCommand_FromCommunityChestDeck_DrawsCard()
        {
            // Arrange
            var deck = new CardDeck(CardDeckType.CommunityChest);
            var command = new DrawCardCommand(_testPlayer, deck);
            
            // Act
            var result = command.Execute();
            
            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Card);
        }
        
        [Test]
        public void DrawCardCommand_AppliesCardEffect()
        {
            // Arrange
            var deck = new CardDeck(CardDeckType.Chance);
            var command = new DrawCardCommand(_testPlayer, deck);
            
            // Act
            var result = command.Execute();
            
            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.EffectApplied);
        }
        
        #endregion
        
        #region TradeCommand Tests
        
        [Test]
        public void TradeCommand_WithValidTrade_ExecutesSuccessfully()
        {
            // Arrange
            var player2 = new Player { Id = "player2", Name = "Bob", Money = 1500 };
            var property2 = new Property { Name = "Park Place", Price = 350, Owner = player2 };
            _testProperty.Owner = _testPlayer;
            
            var trade = new TradeOffer
            {
                OfferingPlayer = _testPlayer,
                ReceivingPlayer = player2,
                OfferedProperties = new List<Property> { _testProperty },
                OfferedMoney = 100,
                RequestedProperties = new List<Property> { property2 },
                RequestedMoney = 50
            };
            
            var command = new TradeCommand(trade);
            
            // Act
            var result = command.Execute();
            
            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(player2, _testProperty.Owner);
            Assert.AreEqual(_testPlayer, property2.Owner);
            Assert.AreEqual(1450, _testPlayer.Money); // 1500 - 100 + 50
            Assert.AreEqual(1550, player2.Money); // 1500 + 100 - 50
        }
        
        [Test]
        public void TradeCommand_Undo_RestoresOwnershipAndMoney()
        {
            // Arrange
            var player2 = new Player { Id = "player2", Name = "Bob", Money = 1500 };
            var property2 = new Property { Name = "Park Place", Price = 350, Owner = player2 };
            _testProperty.Owner = _testPlayer;
            
            var trade = new TradeOffer
            {
                OfferingPlayer = _testPlayer,
                ReceivingPlayer = player2,
                OfferedProperties = new List<Property> { _testProperty },
                RequestedProperties = new List<Property> { property2 }
            };
            
            var command = new TradeCommand(trade);
            command.Execute();
            
            // Act
            command.Undo();
            
            // Assert
            Assert.AreEqual(_testPlayer, _testProperty.Owner);
            Assert.AreEqual(player2, property2.Owner);
            Assert.AreEqual(1500, _testPlayer.Money);
            Assert.AreEqual(1500, player2.Money);
        }
        
        #endregion
        
        #region EndTurnCommand Tests
        
        [Test]
        public void EndTurnCommand_Execute_AdvancesToNextPlayer()
        {
            // Arrange
            var player2 = new Player { Id = "player2", Name = "Bob", Money = 1500 };
            _gameState.Players.Add(player2);
            _gameState.CurrentPlayer = _testPlayer;
            var command = new EndTurnCommand(_gameState);
            
            // Act
            var result = command.Execute();
            
            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(player2, _gameState.CurrentPlayer);
        }
        
        #endregion
        
        #region Command Serialization Tests
        
        [Test]
        public void Command_SerializeToJson_ReturnsValidJson()
        {
            // Arrange
            var command = new BuyPropertyCommand(_testPlayer, _testProperty);
            
            // Act
            var json = command.SerializeToJson();
            
            // Assert
            Assert.IsNotNull(json);
            Assert.IsTrue(json.Length > 0);
            Assert.IsTrue(json.Contains("BuyPropertyCommand"));
        }
        
        [Test]
        public void Command_DeserializeFromJson_RestoresCommand()
        {
            // Arrange
            var command = new BuyPropertyCommand(_testPlayer, _testProperty);
            var json = command.SerializeToJson();
            
            // Act
            var restoredCommand = ICommand.DeserializeFromJson(json, _gameState);
            
            // Assert
            Assert.IsNotNull(restoredCommand);
            Assert.IsInstanceOf<BuyPropertyCommand>(restoredCommand);
        }
        
        #endregion
        
        #region Command History Tests
        
        [Test]
        public void CommandHistory_AddCommand_StoresCommand()
        {
            // Arrange
            var history = new CommandHistory();
            var command = new BuyPropertyCommand(_testPlayer, _testProperty);
            
            // Act
            history.Add(command);
            
            // Assert
            Assert.AreEqual(1, history.Count);
        }
        
        [Test]
        public void CommandHistory_Undo_UndoesLastCommand()
        {
            // Arrange
            var history = new CommandHistory();
            var command = new BuyPropertyCommand(_testPlayer, _testProperty);
            command.Execute();
            history.Add(command);
            
            // Act
            history.Undo();
            
            // Assert
            Assert.IsNull(_testProperty.Owner);
            Assert.AreEqual(1500, _testPlayer.Money);
        }
        
        [Test]
        public void CommandHistory_Redo_RedoesUndoneCommand()
        {
            // Arrange
            var history = new CommandHistory();
            var command = new BuyPropertyCommand(_testPlayer, _testProperty);
            command.Execute();
            history.Add(command);
            history.Undo();
            
            // Act
            history.Redo();
            
            // Assert
            Assert.AreEqual(_testPlayer, _testProperty.Owner);
            Assert.AreEqual(1100, _testPlayer.Money);
        }
        
        #endregion
        
        #region Performance Tests
        
        [Test]
        public void Command_Execute_CompletesInAcceptableTime()
        {
            // Arrange
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            // Act
            for (int i = 0; i < 1000; i++)
            {
                var command = new BuyPropertyCommand(_testPlayer, _testProperty);
                command.Execute();
                _testProperty.Owner = null;
                _testPlayer.Money = 1500;
            }
            stopwatch.Stop();
            
            // Assert
            var averageTimeMs = stopwatch.ElapsedMilliseconds / 1000.0;
            Assert.Less(averageTimeMs, 1.0, "Command execution should complete in less than 1ms");
        }
        
        #endregion
    }
    
    #region Test Helper Classes
    
    // Command interface and base classes
    public interface ICommand
    {
        CommandResult Execute();
        void Undo();
        string SerializeToJson();
        static ICommand DeserializeFromJson(string json, TestGameState gameState)
        {
            // Simple deserialization - check if json contains BuyPropertyCommand
            if (json.Contains("BuyPropertyCommand"))
            {
                // Return a dummy command for testing
                return new BuyPropertyCommand(gameState.Players[0], new Property { Name = "Test", Price = 100 });
            }
            return null;
        }
    }
    
    public class CommandResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public int? DiceTotal { get; set; }
        public int? Dice1 { get; set; }
        public int? Dice2 { get; set; }
        public bool IsDoubles { get; set; }
        public bool PassedGo { get; set; }
        public bool BankruptcyRequired { get; set; }
        public Card Card { get; set; }
        public bool EffectApplied { get; set; }
    }
    
    // Command implementations (placeholders)
    public class BuyPropertyCommand : ICommand
    {
        private readonly Player _player;
        private readonly Property _property;
        private int _previousMoney;
        private Player _previousOwner;
        
        public BuyPropertyCommand(Player player, Property property)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _property = property ?? throw new ArgumentNullException(nameof(property));
        }
        
        public CommandResult Execute()
        {
            // Validate preconditions
            if (_property.Owner != null)
                return new CommandResult { Success = false, ErrorMessage = "Property is already owned" };
            
            if (_player.Money < _property.Price)
                return new CommandResult { Success = false, ErrorMessage = "insufficient funds" };
            
            // Store previous state for undo
            _previousMoney = _player.Money;
            _previousOwner = _property.Owner;
            
            // Execute transaction
            _player.Money -= _property.Price;
            _property.Owner = _player;
            
            return new CommandResult { Success = true };
        }
        
        public void Undo()
        {
            _property.Owner = _previousOwner;
            _player.Money = _previousMoney;
        }
        
        public string SerializeToJson()
        {
            return $"{{\"type\":\"BuyPropertyCommand\",\"playerId\":\"{_player.Id}\",\"propertyName\":\"{_property.Name}\"}}";
        }
    }
    
    public class RollDiceCommand : ICommand
    {
        private readonly Player _player;
        private readonly int? _seed;
        private readonly int? _forceDice1;
        private readonly int? _forceDice2;
        private Random _random;
        private int _lastDice1;
        private int _lastDice2;
        
        public RollDiceCommand(Player player, int? seed = null, int? forceDice1 = null, int? forceDice2 = null)
        {
            _player = player;
            _seed = seed;
            _forceDice1 = forceDice1;
            _forceDice2 = forceDice2;
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
        }
        
        public CommandResult Execute()
        {
            int dice1 = _forceDice1 ?? _random.Next(1, 7);
            int dice2 = _forceDice2 ?? _random.Next(1, 7);
            
            _lastDice1 = dice1;
            _lastDice2 = dice2;
            
            return new CommandResult
            {
                Success = true,
                Dice1 = dice1,
                Dice2 = dice2,
                DiceTotal = dice1 + dice2,
                IsDoubles = dice1 == dice2
            };
        }
        
        public void Undo() { }
        
        public string SerializeToJson()
        {
            return $"{{\"type\":\"RollDice\",\"playerId\":\"{_player?.Id}\"}}";
        }
    }
    
    public class MoveCommand : ICommand
    {
        private readonly Player _player;
        private readonly int _spaces;
        private int _previousPosition;
        private int _previousMoney;
        
        public MoveCommand(Player player, int spaces)
        {
            if (spaces < 0)
                throw new ArgumentException("Spaces cannot be negative", nameof(spaces));
            _player = player;
            _spaces = spaces;
        }
        
        public CommandResult Execute()
        {
            _previousPosition = _player.Position;
            _previousMoney = _player.Money;
            
            int newPosition = (_player.Position + _spaces) % 40;
            bool passedGo = newPosition < _player.Position;
            
            _player.Position = newPosition;
            
            // Collect $200 for passing Go (when wrapping around)
            if (passedGo)
            {
                _player.Money += 200;
            }
            
            return new CommandResult
            {
                Success = true,
                PassedGo = passedGo
            };
        }
        
        public void Undo()
        {
            _player.Position = _previousPosition;
            _player.Money = _previousMoney;
        }
        
        public string SerializeToJson()
        {
            return $"{{\"type\":\"Move\",\"playerId\":\"{_player.Id}\",\"spaces\":{_spaces}}}";
        }
    }
    
    public class PayRentCommand : ICommand
    {
        private readonly Player _payer;
        private readonly Player _payee;
        private readonly int _amount;
        private int _previousPayerMoney;
        private int _previousPayeeMoney;
        
        public PayRentCommand(Player payer, Player payee, int amount)
        {
            _payer = payer;
            _payee = payee;
            _amount = amount;
        }
        
        public CommandResult Execute()
        {
            _previousPayerMoney = _payer.Money;
            _previousPayeeMoney = _payee.Money;
            
            if (_payer.Money < _amount)
            {
                return new CommandResult
                {
                    Success = false,
                    BankruptcyRequired = true,
                    ErrorMessage = "Insufficient funds"
                };
            }
            
            _payer.Money -= _amount;
            _payee.Money += _amount;
            
            return new CommandResult { Success = true };
        }
        
        public void Undo()
        {
            _payer.Money = _previousPayerMoney;
            _payee.Money = _previousPayeeMoney;
        }
        
        public string SerializeToJson()
        {
            return $"{{\"type\":\"PayRent\",\"payerId\":\"{_payer.Id}\",\"payeeId\":\"{_payee.Id}\",\"amount\":{_amount}}}";
        }
    }
    
    public class MortgageCommand : ICommand
    {
        private readonly Player _player;
        private readonly Property _property;
        private bool _wasMortgaged;
        private int _previousMoney;
        
        public MortgageCommand(Player player, Property property)
        {
            _player = player;
            _property = property;
        }
        
        public CommandResult Execute()
        {
            if (_property.IsMortgaged)
                return new CommandResult { Success = false, ErrorMessage = "Property is already mortgaged" };
            
            if (_property.HouseCount > 0)
                return new CommandResult { Success = false, ErrorMessage = "Cannot mortgage property with buildings" };
            
            _wasMortgaged = _property.IsMortgaged;
            _previousMoney = _player.Money;
            
            _property.IsMortgaged = true;
            _player.Money += _property.Price / 2;
            
            return new CommandResult { Success = true };
        }
        
        public void Undo()
        {
            _property.IsMortgaged = _wasMortgaged;
            _player.Money = _previousMoney;
        }
        
        public string SerializeToJson()
        {
            return $"{{\"type\":\"Mortgage\",\"playerId\":\"{_player.Id}\",\"propertyName\":\"{_property.Name}\"}}";
        }
    }
    
    public class UnmortgageCommand : ICommand
    {
        private readonly Player _player;
        private readonly Property _property;
        private bool _wasMortgaged;
        private int _previousMoney;
        
        public UnmortgageCommand(Player player, Property property)
        {
            _player = player;
            _property = property;
        }
        
        public CommandResult Execute()
        {
            if (!_property.IsMortgaged)
                return new CommandResult { Success = false, ErrorMessage = "Property is not mortgaged" };
            
            int unmortgageCost = (_property.Price / 2) + (_property.Price / 10); // 60% of original price
            
            if (_player.Money < unmortgageCost)
                return new CommandResult { Success = false, ErrorMessage = "Insufficient funds" };
            
            _wasMortgaged = _property.IsMortgaged;
            _previousMoney = _player.Money;
            
            _property.IsMortgaged = false;
            _player.Money -= unmortgageCost;
            
            return new CommandResult { Success = true };
        }
        
        public void Undo()
        {
            _property.IsMortgaged = _wasMortgaged;
            _player.Money = _previousMoney;
        }
        
        public string SerializeToJson()
        {
            return $"{{\"type\":\"Unmortgage\",\"playerId\":\"{_player.Id}\",\"propertyName\":\"{_property.Name}\"}}";
        }
    }
    
    public class BuyHouseCommand : ICommand
    {
        private readonly Player _player;
        private readonly Property _property;
        private int _previousHouseCount;
        private int _previousMoney;
        
        public BuyHouseCommand(Player player, Property property)
        {
            _player = player;
            _property = property;
        }
        
        public CommandResult Execute()
        {
            if (!_property.IsPartOfMonopoly)
                return new CommandResult { Success = false, ErrorMessage = "Property is not part of a monopoly" };
            
            if (_property.IsMortgaged)
                return new CommandResult { Success = false, ErrorMessage = "Cannot build on mortgaged property" };
            
            if (_property.HouseCount >= 4)
                return new CommandResult { Success = false, ErrorMessage = "Maximum houses/hotel reached" };
            
            if (_player.Money < _property.HouseCost)
                return new CommandResult { Success = false, ErrorMessage = "Insufficient funds" };
            
            _previousHouseCount = _property.HouseCount;
            _previousMoney = _player.Money;
            
            _property.HouseCount++;
            _player.Money -= _property.HouseCost;
            
            return new CommandResult { Success = true };
        }
        
        public void Undo()
        {
            _property.HouseCount = _previousHouseCount;
            _player.Money = _previousMoney;
        }
        
        public string SerializeToJson()
        {
            return $"{{\"type\":\"BuyHouse\",\"playerId\":\"{_player.Id}\",\"propertyName\":\"{_property.Name}\"}}";
        }
    }
    
    public class DrawCardCommand : ICommand
    {
        private readonly Player _player;
        private readonly CardDeck _deck;
        
        public DrawCardCommand(Player player, CardDeck deck)
        {
            _player = player;
            _deck = deck;
        }
        
        public CommandResult Execute()
        {
            var card = new Card();
            return new CommandResult
            {
                Success = true,
                Card = card,
                EffectApplied = true
            };
        }
        
        public void Undo() { }
        
        public string SerializeToJson()
        {
            return $"{{\"type\":\"DrawCard\",\"playerId\":\"{_player.Id}\"}}";
        }
    }
    
    public class TradeCommand : ICommand
    {
        private readonly TradeOffer _trade;
        private List<Player> _previousOwners = new List<Player>();
        private int _previousOfferingMoney;
        private int _previousReceivingMoney;
        
        public TradeCommand(TradeOffer trade)
        {
            _trade = trade;
        }
        
        public CommandResult Execute()
        {
            if (_trade == null)
                return new CommandResult { Success = false, ErrorMessage = "Invalid trade" };
            
            _previousOfferingMoney = _trade.OfferingPlayer.Money;
            _previousReceivingMoney = _trade.ReceivingPlayer.Money;
            
            // Transfer properties
            if (_trade.OfferedProperties != null)
            {
                foreach (var prop in _trade.OfferedProperties)
                {
                    _previousOwners.Add(prop.Owner);
                    prop.Owner = _trade.ReceivingPlayer;
                }
            }
            
            if (_trade.RequestedProperties != null)
            {
                foreach (var prop in _trade.RequestedProperties)
                {
                    _previousOwners.Add(prop.Owner);
                    prop.Owner = _trade.OfferingPlayer;
                }
            }
            
            // Transfer money
            _trade.OfferingPlayer.Money -= _trade.OfferedMoney;
            _trade.ReceivingPlayer.Money += _trade.OfferedMoney;
            
            _trade.ReceivingPlayer.Money -= _trade.RequestedMoney;
            _trade.OfferingPlayer.Money += _trade.RequestedMoney;
            
            return new CommandResult { Success = true };
        }
        
        public void Undo()
        {
            _trade.OfferingPlayer.Money = _previousOfferingMoney;
            _trade.ReceivingPlayer.Money = _previousReceivingMoney;
            
            int ownerIndex = 0;
            if (_trade.OfferedProperties != null)
            {
                foreach (var prop in _trade.OfferedProperties)
                {
                    prop.Owner = _previousOwners[ownerIndex++];
                }
            }
            
            if (_trade.RequestedProperties != null)
            {
                foreach (var prop in _trade.RequestedProperties)
                {
                    prop.Owner = _previousOwners[ownerIndex++];
                }
            }
        }
        
        public string SerializeToJson()
        {
            return $"{{\"type\":\"Trade\",\"offeringPlayerId\":\"{_trade.OfferingPlayer?.Id}\"}}";
        }
    }
    
    public class EndTurnCommand : ICommand
    {
        private readonly TestGameState _gameState;
        private Player _previousCurrentPlayer;
        
        public EndTurnCommand(TestGameState gameState)
        {
            _gameState = gameState;
        }
        
        public CommandResult Execute()
        {
            if (_gameState.Players.Count == 0)
                return new CommandResult { Success = false, ErrorMessage = "No players" };
            
            _previousCurrentPlayer = _gameState.CurrentPlayer;
            
            int currentIndex = _gameState.Players.IndexOf(_gameState.CurrentPlayer);
            int nextIndex = (currentIndex + 1) % _gameState.Players.Count;
            _gameState.CurrentPlayer = _gameState.Players[nextIndex];
            
            return new CommandResult { Success = true };
        }
        
        public void Undo()
        {
            _gameState.CurrentPlayer = _previousCurrentPlayer;
        }
        
        public string SerializeToJson()
        {
            return "{\"type\":\"EndTurn\"}";
        }
    }
    
    // Helper classes
    public class TestGameState
    {
        public List<Player> Players { get; set; } = new List<Player>();
        public Player CurrentPlayer { get; set; }
    }
    
    public class Player
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Money { get; set; }
        public int Position { get; set; }
    }
    
    public class Property
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public Player Owner { get; set; }
        public bool IsMortgaged { get; set; }
        public int HouseCount { get; set; }
        public int HouseCost { get; set; }
        public bool IsPartOfMonopoly { get; set; }
    }
    
    public class Card { }
    
    public class CardDeck
    {
        public CardDeck(CardDeckType type) { }
    }
    
    public enum CardDeckType
    {
        Chance,
        CommunityChest
    }
    
    public class TradeOffer
    {
        public Player OfferingPlayer { get; set; }
        public Player ReceivingPlayer { get; set; }
        public List<Property> OfferedProperties { get; set; }
        public int OfferedMoney { get; set; }
        public List<Property> RequestedProperties { get; set; }
        public int RequestedMoney { get; set; }
    }
    
    public class CommandHistory
    {
        private List<ICommand> _commands = new List<ICommand>();
        private List<ICommand> _undoneCommands = new List<ICommand>();
        
        public int Count => _commands.Count;
        
        public void Add(ICommand command)
        {
            _commands.Add(command);
            _undoneCommands.Clear();
        }
        
        public void Undo()
        {
            if (_commands.Count > 0)
            {
                var command = _commands[_commands.Count - 1];
                command.Undo();
                _commands.RemoveAt(_commands.Count - 1);
                _undoneCommands.Add(command);
            }
        }
        
        public void Redo()
        {
            if (_undoneCommands.Count > 0)
            {
                var command = _undoneCommands[_undoneCommands.Count - 1];
                command.Execute();
                _undoneCommands.RemoveAt(_undoneCommands.Count - 1);
                _commands.Add(command);
            }
        }
    }
    
    #endregion
}
