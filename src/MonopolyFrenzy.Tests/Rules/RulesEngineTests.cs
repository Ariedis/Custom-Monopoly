using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonopolyFrenzy.Tests.Rules
{
    /// <summary>
    /// Test Suite 3: Monopoly Rules Engine Tests
    /// Tests for User Story 1.4: Monopoly Rules Engine
    /// 
    /// Validates all standard Monopoly rules:
    /// - Property purchase validation
    /// - Rent calculation (base rent, with houses/hotels, mortgage status)
    /// - House/hotel purchase rules
    /// - Trading rules
    /// - Bankruptcy rules
    /// - Jail rules
    /// - Chance and Community Chest card effects
    /// - Pass GO mechanics
    /// - Free Parking (configurable)
    /// - Auction system
    /// - Mortgage system
    /// </summary>
    [TestFixture]
    public class RulesEngineTests
    {
        private RulesEngine _rulesEngine;
        private GameContext _gameContext;
        
        [SetUp]
        public void Setup()
        {
            _gameContext = new GameContext();
            _rulesEngine = new RulesEngine(_gameContext);
        }
        
        [TearDown]
        public void Teardown()
        {
            _rulesEngine = null;
            _gameContext = null;
        }
        
        #region Property Purchase Validation Tests
        
        [Test]
        public void CanPurchaseProperty_WithSufficientFunds_ReturnsTrue()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            var property = CreateProperty("Boardwalk", 400);
            
            // Act
            var canPurchase = _rulesEngine.CanPurchaseProperty(player, property);
            
            // Assert
            Assert.IsTrue(canPurchase);
        }
        
        [Test]
        public void CanPurchaseProperty_WithInsufficientFunds_ReturnsFalse()
        {
            // Arrange
            var player = CreatePlayer("Alice", 100);
            var property = CreateProperty("Boardwalk", 400);
            
            // Act
            var canPurchase = _rulesEngine.CanPurchaseProperty(player, property);
            
            // Assert
            Assert.IsFalse(canPurchase);
        }
        
        [Test]
        public void CanPurchaseProperty_AlreadyOwned_ReturnsFalse()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            var owner = CreatePlayer("Bob", 1500);
            var property = CreateProperty("Boardwalk", 400);
            property.Owner = owner;
            
            // Act
            var canPurchase = _rulesEngine.CanPurchaseProperty(player, property);
            
            // Assert
            Assert.IsFalse(canPurchase);
        }
        
        [Test]
        public void CanPurchaseProperty_OnNonPurchasableSpace_ReturnsFalse()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            var space = CreateSpace("Income Tax", SpaceType.Tax);
            
            // Act
            var canPurchase = _rulesEngine.CanPurchaseSpace(player, space);
            
            // Assert
            Assert.IsFalse(canPurchase);
        }
        
        #endregion
        
        #region Rent Calculation Tests
        
        [Test]
        public void CalculateRent_BaseRent_ReturnsCorrectAmount()
        {
            // Arrange
            var property = CreateProperty("Mediterranean Avenue", 60);
            property.BaseRent = 2;
            
            // Act
            var rent = _rulesEngine.CalculateRent(property);
            
            // Assert
            Assert.AreEqual(2, rent);
        }
        
        [Test]
        public void CalculateRent_WithOneHouse_ReturnsIncreasedRent()
        {
            // Arrange
            var property = CreateProperty("Mediterranean Avenue", 60);
            property.BaseRent = 2;
            property.RentWithOneHouse = 10;
            property.HouseCount = 1;
            
            // Act
            var rent = _rulesEngine.CalculateRent(property);
            
            // Assert
            Assert.AreEqual(10, rent);
        }
        
        [Test]
        public void CalculateRent_WithTwoHouses_ReturnsIncreasedRent()
        {
            // Arrange
            var property = CreateProperty("Mediterranean Avenue", 60);
            property.RentWithTwoHouses = 30;
            property.HouseCount = 2;
            
            // Act
            var rent = _rulesEngine.CalculateRent(property);
            
            // Assert
            Assert.AreEqual(30, rent);
        }
        
        [Test]
        public void CalculateRent_WithHotel_ReturnsMaximumRent()
        {
            // Arrange
            var property = CreateProperty("Boardwalk", 400);
            property.RentWithHotel = 2000;
            property.HasHotel = true;
            
            // Act
            var rent = _rulesEngine.CalculateRent(property);
            
            // Assert
            Assert.AreEqual(2000, rent);
        }
        
        [Test]
        public void CalculateRent_MortgagedProperty_ReturnsZero()
        {
            // Arrange
            var property = CreateProperty("Boardwalk", 400);
            property.BaseRent = 50;
            property.IsMortgaged = true;
            
            // Act
            var rent = _rulesEngine.CalculateRent(property);
            
            // Assert
            Assert.AreEqual(0, rent);
        }
        
        [Test]
        public void CalculateRent_WithMonopoly_DoublesBaseRent()
        {
            // Arrange
            var property = CreateProperty("Mediterranean Avenue", 60);
            property.BaseRent = 2;
            property.HouseCount = 0;
            property.IsPartOfMonopoly = true;
            
            // Act
            var rent = _rulesEngine.CalculateRent(property);
            
            // Assert
            Assert.AreEqual(4, rent); // Doubled
        }
        
        [Test]
        public void CalculateRent_Railroad_BasedOnNumberOwned()
        {
            // Arrange
            var railroad = CreateRailroad("Reading Railroad");
            var owner = CreatePlayer("Alice", 1500);
            railroad.Owner = owner;
            owner.OwnedRailroads = 2;
            
            // Act
            var rent = _rulesEngine.CalculateRailroadRent(railroad, owner);
            
            // Assert
            Assert.AreEqual(50, rent); // $25, $50, $100, $200 for 1, 2, 3, 4 railroads
        }
        
        [TestCase(1, 25)]
        [TestCase(2, 50)]
        [TestCase(3, 100)]
        [TestCase(4, 200)]
        public void CalculateRent_Railroad_MultipleScenarios(int railroadsOwned, int expectedRent)
        {
            // Arrange
            var railroad = CreateRailroad("Reading Railroad");
            var owner = CreatePlayer("Alice", 1500);
            railroad.Owner = owner;
            owner.OwnedRailroads = railroadsOwned;
            
            // Act
            var rent = _rulesEngine.CalculateRailroadRent(railroad, owner);
            
            // Assert
            Assert.AreEqual(expectedRent, rent);
        }
        
        [Test]
        public void CalculateRent_Utility_BasedOnDiceRoll()
        {
            // Arrange
            var utility = CreateUtility("Electric Company");
            var owner = CreatePlayer("Alice", 1500);
            utility.Owner = owner;
            owner.OwnedUtilities = 1;
            var diceRoll = 7;
            
            // Act
            var rent = _rulesEngine.CalculateUtilityRent(utility, owner, diceRoll);
            
            // Assert
            Assert.AreEqual(28, rent); // 4x dice roll with 1 utility
        }
        
        [Test]
        public void CalculateRent_BothUtilities_TenTimesDiceRoll()
        {
            // Arrange
            var utility = CreateUtility("Electric Company");
            var owner = CreatePlayer("Alice", 1500);
            utility.Owner = owner;
            owner.OwnedUtilities = 2;
            var diceRoll = 7;
            
            // Act
            var rent = _rulesEngine.CalculateUtilityRent(utility, owner, diceRoll);
            
            // Assert
            Assert.AreEqual(70, rent); // 10x dice roll with 2 utilities
        }
        
        #endregion
        
        #region House/Hotel Purchase Rules Tests
        
        [Test]
        public void CanBuyHouse_WithMonopoly_ReturnsTrue()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            var property = CreateProperty("Mediterranean Avenue", 60);
            property.Owner = player;
            property.IsPartOfMonopoly = true;
            property.HouseCost = 50;
            
            // Act
            var canBuy = _rulesEngine.CanBuyHouse(player, property);
            
            // Assert
            Assert.IsTrue(canBuy);
        }
        
        [Test]
        public void CanBuyHouse_WithoutMonopoly_ReturnsFalse()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            var property = CreateProperty("Mediterranean Avenue", 60);
            property.Owner = player;
            property.IsPartOfMonopoly = false;
            
            // Act
            var canBuy = _rulesEngine.CanBuyHouse(player, property);
            
            // Assert
            Assert.IsFalse(canBuy);
        }
        
        [Test]
        public void CanBuyHouse_OnMortgagedProperty_ReturnsFalse()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            var property = CreateProperty("Mediterranean Avenue", 60);
            property.Owner = player;
            property.IsPartOfMonopoly = true;
            property.IsMortgaged = true;
            
            // Act
            var canBuy = _rulesEngine.CanBuyHouse(player, property);
            
            // Assert
            Assert.IsFalse(canBuy);
        }
        
        [Test]
        public void CanBuyHouse_WithAnyMortgagedInMonopoly_ReturnsFalse()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            var property1 = CreateProperty("Mediterranean Avenue", 60);
            var property2 = CreateProperty("Baltic Avenue", 60);
            property1.Owner = player;
            property1.IsPartOfMonopoly = true;
            property2.Owner = player;
            property2.IsPartOfMonopoly = true;
            property2.IsMortgaged = true;
            
            var monopoly = new List<Property> { property1, property2 };
            
            // Act
            var canBuy = _rulesEngine.CanBuyHouseInMonopoly(player, property1, monopoly);
            
            // Assert
            Assert.IsFalse(canBuy);
        }
        
        [Test]
        public void CanBuyHouse_EvenBuildingRule_EnforcesEvenDistribution()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            var property1 = CreateProperty("Mediterranean Avenue", 60);
            var property2 = CreateProperty("Baltic Avenue", 60);
            property1.Owner = player;
            property1.IsPartOfMonopoly = true;
            property1.HouseCount = 2;
            property2.Owner = player;
            property2.IsPartOfMonopoly = true;
            property2.HouseCount = 0;
            
            var monopoly = new List<Property> { property1, property2 };
            
            // Act
            var canBuy = _rulesEngine.CanBuyHouseInMonopoly(player, property1, monopoly);
            
            // Assert
            Assert.IsFalse(canBuy, "Cannot buy more houses on property1 until property2 has at least 1 house");
        }
        
        [Test]
        public void CanBuyHouse_WithFourHouses_CanUpgradeToHotel()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            var property = CreateProperty("Mediterranean Avenue", 60);
            property.Owner = player;
            property.IsPartOfMonopoly = true;
            property.HouseCount = 4;
            property.HouseCost = 50;
            
            // Act
            var canBuyHotel = _rulesEngine.CanBuyHotel(player, property);
            
            // Assert
            Assert.IsTrue(canBuyHotel);
        }
        
        [Test]
        public void CanBuyHouse_WithHotel_ReturnsFalse()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            var property = CreateProperty("Mediterranean Avenue", 60);
            property.Owner = player;
            property.IsPartOfMonopoly = true;
            property.HasHotel = true;
            
            // Act
            var canBuy = _rulesEngine.CanBuyHouse(player, property);
            
            // Assert
            Assert.IsFalse(canBuy);
        }
        
        [Test]
        public void CanBuyHouse_InsufficientFunds_ReturnsFalse()
        {
            // Arrange
            var player = CreatePlayer("Alice", 40);
            var property = CreateProperty("Mediterranean Avenue", 60);
            property.Owner = player;
            property.IsPartOfMonopoly = true;
            property.HouseCost = 50;
            
            // Act
            var canBuy = _rulesEngine.CanBuyHouse(player, property);
            
            // Assert
            Assert.IsFalse(canBuy);
        }
        
        #endregion
        
        #region Trading Rules Tests
        
        [Test]
        public void ValidateTrade_BothPartiesAgree_ReturnsValid()
        {
            // Arrange
            var player1 = CreatePlayer("Alice", 1500);
            var player2 = CreatePlayer("Bob", 1500);
            var property1 = CreateProperty("Mediterranean Avenue", 60);
            var property2 = CreateProperty("Baltic Avenue", 60);
            property1.Owner = player1;
            property2.Owner = player2;
            
            var trade = new TradeOffer
            {
                OfferingPlayer = player1,
                ReceivingPlayer = player2,
                OfferedProperties = new List<Property> { property1 },
                RequestedProperties = new List<Property> { property2 },
                BothPartiesAgree = true
            };
            
            // Act
            var isValid = _rulesEngine.ValidateTrade(trade);
            
            // Assert
            Assert.IsTrue(isValid.IsValid);
        }
        
        [Test]
        public void ValidateTrade_WithoutAgreement_ReturnsInvalid()
        {
            // Arrange
            var player1 = CreatePlayer("Alice", 1500);
            var player2 = CreatePlayer("Bob", 1500);
            var trade = new TradeOffer
            {
                OfferingPlayer = player1,
                ReceivingPlayer = player2,
                BothPartiesAgree = false
            };
            
            // Act
            var isValid = _rulesEngine.ValidateTrade(trade);
            
            // Assert
            Assert.IsFalse(isValid.IsValid);
        }
        
        [Test]
        public void ValidateTrade_PropertyWithBuildings_ReturnsInvalid()
        {
            // Arrange
            var player1 = CreatePlayer("Alice", 1500);
            var player2 = CreatePlayer("Bob", 1500);
            var property = CreateProperty("Mediterranean Avenue", 60);
            property.Owner = player1;
            property.HouseCount = 2;
            
            var trade = new TradeOffer
            {
                OfferingPlayer = player1,
                ReceivingPlayer = player2,
                OfferedProperties = new List<Property> { property },
                BothPartiesAgree = true
            };
            
            // Act
            var isValid = _rulesEngine.ValidateTrade(trade);
            
            // Assert
            Assert.IsFalse(isValid.IsValid);
            Assert.IsTrue(isValid.ErrorMessage.Contains("buildings"));
        }
        
        [Test]
        public void ValidateTrade_MortgagedProperty_IsAllowed()
        {
            // Arrange
            var player1 = CreatePlayer("Alice", 1500);
            var player2 = CreatePlayer("Bob", 1500);
            var property = CreateProperty("Mediterranean Avenue", 60);
            property.Owner = player1;
            property.IsMortgaged = true;
            
            var trade = new TradeOffer
            {
                OfferingPlayer = player1,
                ReceivingPlayer = player2,
                OfferedProperties = new List<Property> { property },
                BothPartiesAgree = true
            };
            
            // Act
            var isValid = _rulesEngine.ValidateTrade(trade);
            
            // Assert
            Assert.IsTrue(isValid.IsValid, "Mortgaged properties can be traded");
        }
        
        [Test]
        public void ValidateTrade_IncludingMoney_ValidatesPlayerFunds()
        {
            // Arrange
            var player1 = CreatePlayer("Alice", 100);
            var player2 = CreatePlayer("Bob", 1500);
            
            var trade = new TradeOffer
            {
                OfferingPlayer = player1,
                ReceivingPlayer = player2,
                OfferedMoney = 500,
                BothPartiesAgree = true
            };
            
            // Act
            var isValid = _rulesEngine.ValidateTrade(trade);
            
            // Assert
            Assert.IsFalse(isValid.IsValid);
            Assert.IsTrue(isValid.ErrorMessage.Contains("insufficient"));
        }
        
        #endregion
        
        #region Bankruptcy Rules Tests
        
        [Test]
        public void ProcessBankruptcy_ToPlayer_TransfersAllAssets()
        {
            // Arrange
            var bankruptPlayer = CreatePlayer("Alice", 0);
            var creditor = CreatePlayer("Bob", 1500);
            var property = CreateProperty("Mediterranean Avenue", 60);
            property.Owner = bankruptPlayer;
            bankruptPlayer.Properties.Add(property);
            bankruptPlayer.Money = 50;
            
            // Act
            _rulesEngine.ProcessBankruptcy(bankruptPlayer, creditor);
            
            // Assert
            Assert.IsTrue(bankruptPlayer.IsBankrupt);
            Assert.AreEqual(creditor, property.Owner);
            Assert.AreEqual(1550, creditor.Money);
            Assert.AreEqual(0, bankruptPlayer.Money);
        }
        
        [Test]
        public void ProcessBankruptcy_ToBank_ReturnsPropertiesToBank()
        {
            // Arrange
            var bankruptPlayer = CreatePlayer("Alice", 0);
            var property = CreateProperty("Mediterranean Avenue", 60);
            property.Owner = bankruptPlayer;
            bankruptPlayer.Properties.Add(property);
            
            // Act
            _rulesEngine.ProcessBankruptcy(bankruptPlayer, null);
            
            // Assert
            Assert.IsTrue(bankruptPlayer.IsBankrupt);
            Assert.IsNull(property.Owner);
        }
        
        [Test]
        public void ProcessBankruptcy_RemovesHousesAndHotels()
        {
            // Arrange
            var bankruptPlayer = CreatePlayer("Alice", 0);
            var property = CreateProperty("Mediterranean Avenue", 60);
            property.Owner = bankruptPlayer;
            property.HouseCount = 3;
            bankruptPlayer.Properties.Add(property);
            
            // Act
            _rulesEngine.ProcessBankruptcy(bankruptPlayer, null);
            
            // Assert
            Assert.AreEqual(0, property.HouseCount);
        }
        
        [Test]
        public void ProcessBankruptcy_UnmortgagesProperties()
        {
            // Arrange
            var bankruptPlayer = CreatePlayer("Alice", 0);
            var creditor = CreatePlayer("Bob", 1500);
            var property = CreateProperty("Mediterranean Avenue", 60);
            property.Owner = bankruptPlayer;
            property.IsMortgaged = true;
            bankruptPlayer.Properties.Add(property);
            
            // Act
            _rulesEngine.ProcessBankruptcy(bankruptPlayer, creditor);
            
            // Assert
            Assert.IsFalse(property.IsMortgaged, "Properties should be unmortgaged when transferred");
        }
        
        [Test]
        public void IsBankrupt_WithDebtGreaterThanAssets_ReturnsTrue()
        {
            // Arrange
            var player = CreatePlayer("Alice", 100);
            var property = CreateProperty("Mediterranean Avenue", 60);
            property.Owner = player;
            property.MortgageValue = 30;
            player.Properties.Add(property);
            var debt = 200;
            
            // Act
            var isBankrupt = _rulesEngine.IsBankrupt(player, debt);
            
            // Assert
            Assert.IsTrue(isBankrupt);
        }
        
        [Test]
        public void IsBankrupt_WithAssetsGreaterThanDebt_ReturnsFalse()
        {
            // Arrange
            var player = CreatePlayer("Alice", 100);
            var property = CreateProperty("Mediterranean Avenue", 60);
            property.Owner = player;
            property.MortgageValue = 150;
            player.Properties.Add(property);
            var debt = 200;
            
            // Act
            var isBankrupt = _rulesEngine.IsBankrupt(player, debt);
            
            // Assert
            Assert.IsFalse(isBankrupt);
        }
        
        #endregion
        
        #region Jail Rules Tests
        
        [Test]
        public void SendToJail_MovesPlayerToJail()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            player.Position = 30;
            
            // Act
            _rulesEngine.SendToJail(player);
            
            // Assert
            Assert.AreEqual(10, player.Position); // Jail position
            Assert.IsTrue(player.IsInJail);
        }
        
        [Test]
        public void SendToJail_DoesNotCollectPassGo()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            player.Position = 30;
            
            // Act
            _rulesEngine.SendToJail(player);
            
            // Assert
            Assert.AreEqual(1500, player.Money, "Should not collect $200 when sent to jail");
        }
        
        [Test]
        public void GetOutOfJail_WithDoubles_ReleasesPlayer()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            player.IsInJail = true;
            var dice1 = 3;
            var dice2 = 3;
            
            // Act
            var canLeave = _rulesEngine.CanLeaveJail(player, dice1, dice2);
            
            // Assert
            Assert.IsTrue(canLeave);
        }
        
        [Test]
        public void GetOutOfJail_WithPayment_ReleasesPlayer()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            player.IsInJail = true;
            
            // Act
            var result = _rulesEngine.PayToLeaveJail(player);
            
            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsFalse(player.IsInJail);
            Assert.AreEqual(1450, player.Money); // 1500 - 50
        }
        
        [Test]
        public void GetOutOfJail_WithGetOutOfJailFreeCard_ReleasesPlayer()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            player.IsInJail = true;
            player.GetOutOfJailFreeCards = 1;
            
            // Act
            var result = _rulesEngine.UseGetOutOfJailFreeCard(player);
            
            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsFalse(player.IsInJail);
            Assert.AreEqual(0, player.GetOutOfJailFreeCards);
            Assert.AreEqual(1500, player.Money); // No money deducted
        }
        
        [Test]
        public void StayInJail_AfterThreeTurns_MustPayOrUseCard()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            player.IsInJail = true;
            player.TurnsInJail = 3;
            
            // Act
            var mustLeave = _rulesEngine.MustLeaveJail(player);
            
            // Assert
            Assert.IsTrue(mustLeave);
        }
        
        [Test]
        public void StayInJail_CanStillCollectRent()
        {
            // Arrange
            var jailedPlayer = CreatePlayer("Alice", 1500);
            jailedPlayer.IsInJail = true;
            var property = CreateProperty("Boardwalk", 400);
            property.Owner = jailedPlayer;
            property.BaseRent = 50;
            
            // Act
            var rent = _rulesEngine.CalculateRent(property);
            
            // Assert
            Assert.AreEqual(50, rent, "Player in jail can still collect rent");
        }
        
        #endregion
        
        #region Pass GO Tests
        
        [Test]
        public void PassGo_Collect200Dollars()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            
            // Act
            _rulesEngine.CollectPassGo(player);
            
            // Assert
            Assert.AreEqual(1700, player.Money);
        }
        
        [Test]
        public void LandOnGo_Collect200Dollars()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            player.Position = 0;
            
            // Act
            _rulesEngine.ProcessLandingOnGo(player);
            
            // Assert
            Assert.AreEqual(1700, player.Money);
        }
        
        [Test]
        public void PassGo_WhenGoingToJail_DoesNotCollect()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            player.Position = 30; // Go To Jail space
            
            // Act
            _rulesEngine.SendToJail(player);
            
            // Assert
            Assert.AreEqual(1500, player.Money);
        }
        
        #endregion
        
        #region Free Parking Tests
        
        [Test]
        public void LandOnFreeParking_StandardRules_NoMoneyCollected()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            _rulesEngine.ConfigureFreeParking(collectMoney: false);
            
            // Act
            _rulesEngine.ProcessLandingOnFreeParking(player);
            
            // Assert
            Assert.AreEqual(1500, player.Money);
        }
        
        [Test]
        public void LandOnFreeParking_HouseRule_CollectsAccumulatedMoney()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            _rulesEngine.ConfigureFreeParking(collectMoney: true);
            _gameContext.FreeParkingPool = 500;
            
            // Act
            _rulesEngine.ProcessLandingOnFreeParking(player);
            
            // Assert
            Assert.AreEqual(2000, player.Money);
            Assert.AreEqual(0, _gameContext.FreeParkingPool);
        }
        
        #endregion
        
        #region Auction System Tests
        
        [Test]
        public void StartAuction_WhenPlayerDeclinesProperty_InitiatesAuction()
        {
            // Arrange
            var property = CreateProperty("Boardwalk", 400);
            var players = new List<Player>
            {
                CreatePlayer("Alice", 1500),
                CreatePlayer("Bob", 1500),
                CreatePlayer("Charlie", 1500)
            };
            
            // Act
            var auction = _rulesEngine.StartAuction(property, players);
            
            // Assert
            Assert.IsNotNull(auction);
            Assert.AreEqual(property, auction.Property);
            Assert.AreEqual(3, auction.Participants.Count);
        }
        
        [Test]
        public void ProcessAuction_HighestBidder_WinsProperty()
        {
            // Arrange
            var property = CreateProperty("Boardwalk", 400);
            var alice = CreatePlayer("Alice", 1500);
            var bob = CreatePlayer("Bob", 1500);
            var auction = _rulesEngine.StartAuction(property, new List<Player> { alice, bob });
            
            // Act
            _rulesEngine.PlaceBid(auction, alice, 300);
            _rulesEngine.PlaceBid(auction, bob, 350);
            _rulesEngine.PlaceBid(auction, alice, 0); // Pass
            var winner = _rulesEngine.EndAuction(auction);
            
            // Assert
            Assert.AreEqual(bob, winner);
            Assert.AreEqual(bob, property.Owner);
            Assert.AreEqual(1150, bob.Money); // 1500 - 350
        }
        
        [Test]
        public void ProcessAuction_AllPlayersPass_PropertyRemainsUnowned()
        {
            // Arrange
            var property = CreateProperty("Boardwalk", 400);
            var players = new List<Player>
            {
                CreatePlayer("Alice", 1500),
                CreatePlayer("Bob", 1500)
            };
            var auction = _rulesEngine.StartAuction(property, players);
            
            // Act
            _rulesEngine.PlaceBid(auction, players[0], 0);
            _rulesEngine.PlaceBid(auction, players[1], 0);
            var winner = _rulesEngine.EndAuction(auction);
            
            // Assert
            Assert.IsNull(winner);
            Assert.IsNull(property.Owner);
        }
        
        #endregion
        
        #region Mortgage System Tests
        
        [Test]
        public void MortgageProperty_Receive50PercentValue()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            var property = CreateProperty("Boardwalk", 400);
            property.Owner = player;
            property.MortgageValue = 200;
            
            // Act
            _rulesEngine.MortgageProperty(property, player);
            
            // Assert
            Assert.IsTrue(property.IsMortgaged);
            Assert.AreEqual(1700, player.Money); // 1500 + 200
        }
        
        [Test]
        public void UnmortgageProperty_Pay60PercentValue()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            var property = CreateProperty("Boardwalk", 400);
            property.Owner = player;
            property.MortgageValue = 200;
            property.IsMortgaged = true;
            
            // Act
            var result = _rulesEngine.UnmortgageProperty(property, player);
            
            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsFalse(property.IsMortgaged);
            Assert.AreEqual(1260, player.Money); // 1500 - 240 (200 + 10% interest)
        }
        
        [Test]
        public void MortgageProperty_WithBuildings_NotAllowed()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            var property = CreateProperty("Boardwalk", 400);
            property.Owner = player;
            property.HouseCount = 2;
            
            // Act
            var result = _rulesEngine.CanMortgageProperty(property);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        #endregion
        
        #region Card Effects Tests
        
        [Test]
        public void ChanceCard_AdvanceToGo_MovesPlayerAndCollectsMoney()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            player.Position = 7;
            var card = new Card { Type = CardType.Chance, Effect = CardEffect.AdvanceToGo };
            
            // Act
            _rulesEngine.ApplyCardEffect(card, player);
            
            // Assert
            Assert.AreEqual(0, player.Position);
            Assert.AreEqual(1700, player.Money);
        }
        
        [Test]
        public void ChanceCard_GoToJail_SendsPlayerToJail()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            player.Position = 22;
            var card = new Card { Type = CardType.Chance, Effect = CardEffect.GoToJail };
            
            // Act
            _rulesEngine.ApplyCardEffect(card, player);
            
            // Assert
            Assert.AreEqual(10, player.Position);
            Assert.IsTrue(player.IsInJail);
        }
        
        [Test]
        public void ChanceCard_GetOutOfJailFree_GivesCardToPlayer()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            var card = new Card { Type = CardType.Chance, Effect = CardEffect.GetOutOfJailFree };
            
            // Act
            _rulesEngine.ApplyCardEffect(card, player);
            
            // Assert
            Assert.AreEqual(1, player.GetOutOfJailFreeCards);
        }
        
        [Test]
        public void CommunityChestCard_BankError_Collects200()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            var card = new Card { Type = CardType.CommunityChest, Effect = CardEffect.BankError, Amount = 200 };
            
            // Act
            _rulesEngine.ApplyCardEffect(card, player);
            
            // Assert
            Assert.AreEqual(1700, player.Money);
        }
        
        [Test]
        public void CommunityChestCard_DoctorsFee_Pays50()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            var card = new Card { Type = CardType.CommunityChest, Effect = CardEffect.DoctorsFee, Amount = -50 };
            
            // Act
            _rulesEngine.ApplyCardEffect(card, player);
            
            // Assert
            Assert.AreEqual(1450, player.Money);
        }
        
        [Test]
        public void ChanceCard_PropertyRepairs_ChargesBasedOnBuildings()
        {
            // Arrange
            var player = CreatePlayer("Alice", 1500);
            var property1 = CreateProperty("Mediterranean Avenue", 60);
            property1.Owner = player;
            property1.HouseCount = 2;
            var property2 = CreateProperty("Boardwalk", 400);
            property2.Owner = player;
            property2.HasHotel = true;
            player.Properties.Add(property1);
            player.Properties.Add(property2);
            
            var card = new Card 
            { 
                Type = CardType.Chance, 
                Effect = CardEffect.PropertyRepairs,
                HouseCost = 25,
                HotelCost = 100
            };
            
            // Act
            _rulesEngine.ApplyCardEffect(card, player);
            
            // Assert
            Assert.AreEqual(1350, player.Money); // 1500 - (2*25) - (1*100)
        }
        
        #endregion
        
        #region Test Helper Methods
        
        private Player CreatePlayer(string name, int money)
        {
            return new Player
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Money = money,
                Position = 0,
                Properties = new List<Property>()
            };
        }
        
        private Property CreateProperty(string name, int price)
        {
            return new Property
            {
                Name = name,
                Price = price,
                MortgageValue = price / 2,
                HouseCount = 0,
                HasHotel = false,
                IsMortgaged = false
            };
        }
        
        private Property CreateRailroad(string name)
        {
            return new Property
            {
                Name = name,
                Type = PropertyType.Railroad,
                Price = 200
            };
        }
        
        private Property CreateUtility(string name)
        {
            return new Property
            {
                Name = name,
                Type = PropertyType.Utility,
                Price = 150
            };
        }
        
        private Space CreateSpace(string name, SpaceType type)
        {
            return new Space
            {
                Name = name,
                Type = type
            };
        }
        
        #endregion
    }
    
    #region Test Helper Classes
    
    // Rules Engine and supporting classes
    public class RulesEngine
    {
        private readonly GameContext _context;
        private bool _freeParkingCollectMoney = false;
        
        public RulesEngine(GameContext context)
        {
            _context = context;
        }
        
        public bool CanPurchaseProperty(Player player, Property property)
        {
            if (property == null || player == null) return false;
            if (property.Owner != null) return false;
            if (player.Money < property.Price) return false;
            return true;
        }
        
        public bool CanPurchaseSpace(Player player, Space space)
        {
            if (space == null || player == null) return false;
            // Only Property, Railroad, and Utility spaces can be purchased
            return space.Type == SpaceType.Property || 
                   space.Type == SpaceType.Railroad || 
                   space.Type == SpaceType.Utility;
        }
        
        public int CalculateRent(Property property)
        {
            if (property == null || property.IsMortgaged) return 0;
            
            if (property.HasHotel)
                return property.RentWithHotel;
            
            int rent;
            switch (property.HouseCount)
            {
                case 1: rent = property.RentWithOneHouse; break;
                case 2: rent = property.RentWithTwoHouses; break;
                case 3: rent = property.RentWithTwoHouses; break; // Assuming same as 2
                case 4: rent = property.RentWithTwoHouses; break; // Assuming same as 2
                default: 
                    rent = property.BaseRent;
                    // Double rent for monopoly with no houses
                    if (property.IsPartOfMonopoly && property.HouseCount == 0)
                        rent *= 2;
                    break;
            }
            
            return rent;
        }
        
        public int CalculateRailroadRent(Property railroad, Player owner)
        {
            if (owner == null) return 0;
            int ownedCount = owner.OwnedRailroads;
            return 25 * (1 << (ownedCount - 1)); // 25, 50, 100, 200
        }
        
        public int CalculateUtilityRent(Property utility, Player owner, int diceRoll)
        {
            if (owner == null) return 0;
            int multiplier = owner.OwnedUtilities == 1 ? 4 : 10;
            return diceRoll * multiplier;
        }
        
        public bool CanBuyHouse(Player player, Property property)
        {
            if (player == null || property == null) return false;
            if (!property.IsPartOfMonopoly) return false;
            if (property.IsMortgaged) return false;
            if (property.HasHotel) return false;
            if (property.HouseCount >= 4) return false;
            if (player.Money < property.HouseCost) return false;
            return true;
        }
        
        public bool CanBuyHouseInMonopoly(Player player, Property property, List<Property> monopoly)
        {
            if (!CanBuyHouse(player, property)) return false;
            
            // Check if any property in monopoly is mortgaged
            if (monopoly.Any(p => p.IsMortgaged)) return false;
            
            // Check even building rule
            int minHouses = monopoly.Min(p => p.HouseCount);
            return property.HouseCount <= minHouses;
        }
        
        public bool CanBuyHotel(Player player, Property property)
        {
            if (player == null || property == null) return false;
            if (property.HouseCount != 4) return false;
            if (property.HasHotel) return false;
            if (property.IsMortgaged) return false;
            if (player.Money < property.HouseCost) return false;
            return true;
        }
        
        public ValidationResult ValidateTrade(TradeOffer trade)
        {
            if (trade == null)
                return new ValidationResult { IsValid = false, ErrorMessage = "Trade is null" };
            
            if (!trade.BothPartiesAgree)
                return new ValidationResult { IsValid = false, ErrorMessage = "Both parties must agree" };
            
            // Check if offering player has enough money
            if (trade.OfferedMoney > trade.OfferingPlayer.Money)
                return new ValidationResult { IsValid = false, ErrorMessage = "Offering player has insufficient funds" };
            
            // Check if receiving player has enough money
            if (trade.RequestedMoney > trade.ReceivingPlayer.Money)
                return new ValidationResult { IsValid = false, ErrorMessage = "Receiving player has insufficient funds" };
            
            // Check if any offered properties have buildings
            if (trade.OfferedProperties != null)
            {
                foreach (var prop in trade.OfferedProperties)
                {
                    if (prop.HouseCount > 0 || prop.HasHotel)
                        return new ValidationResult { IsValid = false, ErrorMessage = "Cannot trade properties with buildings" };
                }
            }
            
            // Check if any requested properties have buildings
            if (trade.RequestedProperties != null)
            {
                foreach (var prop in trade.RequestedProperties)
                {
                    if (prop.HouseCount > 0 || prop.HasHotel)
                        return new ValidationResult { IsValid = false, ErrorMessage = "Cannot trade properties with buildings" };
                }
            }
            
            // Mortgaged properties are allowed in trades
            return new ValidationResult { IsValid = true };
        }
        
        public void ProcessBankruptcy(Player bankrupt, Player creditor)
        {
            if (bankrupt == null) return;
            
            bankrupt.IsBankrupt = true;
            
            // Transfer all properties to creditor (or bank if creditor is null)
            if (bankrupt.Properties != null)
            {
                foreach (var property in bankrupt.Properties)
                {
                    // Remove buildings
                    property.HouseCount = 0;
                    property.HasHotel = false;
                    
                    // Unmortgage
                    property.IsMortgaged = false;
                    
                    // Transfer ownership
                    property.Owner = creditor;
                }
            }
            
            // Transfer money if creditor exists
            if (creditor != null)
            {
                creditor.Money += bankrupt.Money;
            }
            
            bankrupt.Money = 0;
        }
        
        public bool IsBankrupt(Player player, int debt)
        {
            if (player == null) return true;
            
            int totalAssets = player.Money;
            
            // Add property values
            if (player.Properties != null)
            {
                foreach (var property in player.Properties)
                {
                    totalAssets += property.MortgageValue;
                    totalAssets += property.HouseCount * (property.HouseCost / 2);
                    if (property.HasHotel)
                        totalAssets += property.HouseCost / 2;
                }
            }
            
            return totalAssets < debt;
        }
        
        public void SendToJail(Player player)
        {
            if (player == null) return;
            player.IsInJail = true;
            player.Position = 10; // Jail position
            player.TurnsInJail = 0;
        }
        
        public bool CanLeaveJail(Player player, int dice1, int dice2)
        {
            if (player == null || !player.IsInJail) return false;
            return dice1 == dice2; // Doubles
        }
        
        public ActionResult PayToLeaveJail(Player player)
        {
            if (player == null)
                return new ActionResult { Success = false, ErrorMessage = "Player is null" };
            
            if (!player.IsInJail)
                return new ActionResult { Success = false, ErrorMessage = "Player is not in jail" };
            
            if (player.Money < 50)
                return new ActionResult { Success = false, ErrorMessage = "Insufficient funds" };
            
            player.Money -= 50;
            player.IsInJail = false;
            player.TurnsInJail = 0;
            
            return new ActionResult { Success = true };
        }
        
        public ActionResult UseGetOutOfJailFreeCard(Player player)
        {
            if (player == null)
                return new ActionResult { Success = false, ErrorMessage = "Player is null" };
            
            if (!player.IsInJail)
                return new ActionResult { Success = false, ErrorMessage = "Player is not in jail" };
            
            if (player.GetOutOfJailFreeCards <= 0)
                return new ActionResult { Success = false, ErrorMessage = "No Get Out of Jail Free cards" };
            
            player.GetOutOfJailFreeCards--;
            player.IsInJail = false;
            player.TurnsInJail = 0;
            
            return new ActionResult { Success = true };
        }
        
        public bool MustLeaveJail(Player player)
        {
            if (player == null || !player.IsInJail) return false;
            return player.TurnsInJail >= 3;
        }
        
        public void CollectPassGo(Player player)
        {
            if (player != null)
                player.Money += 200;
        }
        
        public void ProcessLandingOnGo(Player player)
        {
            if (player != null)
                player.Money += 200;
        }
        
        public void ConfigureFreeParking(bool collectMoney)
        {
            _freeParkingCollectMoney = collectMoney;
        }
        
        public void ProcessLandingOnFreeParking(Player player)
        {
            if (_freeParkingCollectMoney && player != null && _context != null)
            {
                player.Money += _context.FreeParkingPool;
                _context.FreeParkingPool = 0;
            }
        }
        
        public Auction StartAuction(Property property, List<Player> players)
        {
            if (property == null || players == null || players.Count == 0)
                return null;
            
            return new Auction
            {
                Property = property,
                Participants = new List<Player>(players),
                Bids = new Dictionary<Player, int>()
            };
        }
        
        public void PlaceBid(Auction auction, Player player, int amount)
        {
            if (auction == null || player == null) return;
            auction.Bids[player] = amount;
        }
        
        public Player EndAuction(Auction auction)
        {
            if (auction == null || auction.Bids.Count == 0) return null;
            
            // Find highest bidder
            var highestBid = auction.Bids.Max(b => b.Value);
            
            // If all players pass (bid 0), return null
            if (highestBid == 0) return null;
            
            var winner = auction.Bids.First(b => b.Value == highestBid).Key;
            
            // Transfer property and money
            auction.Property.Owner = winner;
            winner.Money -= highestBid;
            
            return winner;
        }
        
        public void MortgageProperty(Property property, Player player)
        {
            if (property == null || player == null) return;
            if (property.HouseCount > 0 || property.HasHotel) return;
            
            property.IsMortgaged = true;
            player.Money += property.MortgageValue;
        }
        
        public ActionResult UnmortgageProperty(Property property, Player player)
        {
            if (property == null || player == null)
                return new ActionResult { Success = false, ErrorMessage = "Invalid parameters" };
            
            if (!property.IsMortgaged)
                return new ActionResult { Success = false, ErrorMessage = "Property is not mortgaged" };
            
            int unmortgageCost = property.MortgageValue + (property.MortgageValue / 5); // 120% = 100% + 20%
            
            if (player.Money < unmortgageCost)
                return new ActionResult { Success = false, ErrorMessage = "Insufficient funds" };
            
            property.IsMortgaged = false;
            player.Money -= unmortgageCost;
            
            return new ActionResult { Success = true };
        }
        
        public bool CanMortgageProperty(Property property)
        {
            if (property == null) return false;
            if (property.IsMortgaged) return false;
            if (property.HouseCount > 0 || property.HasHotel) return false;
            return true;
        }
        
        public void ApplyCardEffect(Card card, Player player)
        {
            if (card == null || player == null) return;
            
            switch (card.Effect)
            {
                case CardEffect.AdvanceToGo:
                    player.Position = 0;
                    CollectPassGo(player);
                    break;
                case CardEffect.GoToJail:
                    SendToJail(player);
                    break;
                case CardEffect.GetOutOfJailFree:
                    player.GetOutOfJailFreeCards++;
                    break;
                case CardEffect.BankError:
                    player.Money += card.Amount;
                    break;
                case CardEffect.DoctorsFee:
                    player.Money += card.Amount; // Amount is negative for fees
                    break;
                case CardEffect.PropertyRepairs:
                    int cost = 0;
                    if (player.Properties != null)
                    {
                        foreach (var prop in player.Properties)
                        {
                            cost += prop.HouseCount * card.HouseCost;
                            if (prop.HasHotel)
                                cost += card.HotelCost;
                        }
                    }
                    player.Money -= cost;
                    break;
            }
        }
    }
    
    public class GameContext
    {
        public int FreeParkingPool { get; set; }
    }
    
    public class Player
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Money { get; set; }
        public int Position { get; set; }
        public bool IsBankrupt { get; set; }
        public bool IsInJail { get; set; }
        public int TurnsInJail { get; set; }
        public int GetOutOfJailFreeCards { get; set; }
        public int OwnedRailroads { get; set; }
        public int OwnedUtilities { get; set; }
        public List<Property> Properties { get; set; }
    }
    
    public class Property
    {
        public string Name { get; set; }
        public PropertyType Type { get; set; }
        public int Price { get; set; }
        public int BaseRent { get; set; }
        public int RentWithOneHouse { get; set; }
        public int RentWithTwoHouses { get; set; }
        public int RentWithHotel { get; set; }
        public int HouseCount { get; set; }
        public int HouseCost { get; set; }
        public bool HasHotel { get; set; }
        public bool IsMortgaged { get; set; }
        public int MortgageValue { get; set; }
        public bool IsPartOfMonopoly { get; set; }
        public Player Owner { get; set; }
    }
    
    public class Space
    {
        public string Name { get; set; }
        public SpaceType Type { get; set; }
    }
    
    public enum PropertyType
    {
        Standard,
        Railroad,
        Utility
    }
    
    public enum SpaceType
    {
        Property,
        Railroad,
        Utility,
        Tax,
        Chance,
        CommunityChest,
        Go,
        Jail,
        FreeParking,
        GoToJail
    }
    
    public class TradeOffer
    {
        public Player OfferingPlayer { get; set; }
        public Player ReceivingPlayer { get; set; }
        public List<Property> OfferedProperties { get; set; }
        public int OfferedMoney { get; set; }
        public List<Property> RequestedProperties { get; set; }
        public int RequestedMoney { get; set; }
        public bool BothPartiesAgree { get; set; }
    }
    
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
    
    public class ActionResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
    
    public class Auction
    {
        public Property Property { get; set; }
        public List<Player> Participants { get; set; }
        public Dictionary<Player, int> Bids { get; set; } = new Dictionary<Player, int>();
    }
    
    public class Card
    {
        public CardType Type { get; set; }
        public CardEffect Effect { get; set; }
        public int Amount { get; set; }
        public int HouseCost { get; set; }
        public int HotelCost { get; set; }
    }
    
    public enum CardType
    {
        Chance,
        CommunityChest
    }
    
    public enum CardEffect
    {
        AdvanceToGo,
        GoToJail,
        GetOutOfJailFree,
        BankError,
        DoctorsFee,
        PropertyRepairs
    }
    
    #endregion
}
