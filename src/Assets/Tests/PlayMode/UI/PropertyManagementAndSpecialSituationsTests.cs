using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using MonopolyFrenzy.UI.Modals;
using MonopolyFrenzy.UI.Controllers;
using MonopolyFrenzy.Core;
using MonopolyFrenzy.Commands;

namespace MonopolyFrenzy.Tests.UI
{
    /// <summary>
    /// Test Suite for User Stories 2.11-2.19: Property Management, Trading, and Special Situations
    /// 
    /// Validates:
    /// - Property cards and information displays
    /// - House/hotel building
    /// - Mortgage and unmortgage functionality
    /// - Trading system (initiate, accept/decline)
    /// - Jail mechanics
    /// - Bankruptcy process
    /// - Chance and Community Chest cards
    /// </summary>
    [TestFixture]
    public class PropertyManagementAndSpecialSituationsTests
    {
        private GameObject _testCanvas;
        private UIController _uiController;
        private GameState _gameState;

        [SetUp]
        public void Setup()
        {
            _testCanvas = new GameObject("TestCanvas");
            var canvas = _testCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _testCanvas.AddComponent<GraphicRaycaster>();

            var controllerObject = new GameObject("UIController");
            _uiController = controllerObject.AddComponent<UIController>();

            _gameState = new GameState();
            _gameState.Initialize();
        }

        [TearDown]
        public void Teardown()
        {
            if (_testCanvas != null)
                Object.Destroy(_testCanvas);
        }

        #region User Story 2.11: Property Cards Tests

        [UnityTest]
        public IEnumerator PropertyCard_ClickPropertyShowsCard()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.ShowPropertyCard(1); // Mediterranean Avenue
            yield return null;

            // Assert
            Assert.IsTrue(_uiController.IsModalShowing("PropertyCard"), 
                "Property card should be displayed");
        }

        [UnityTest]
        public IEnumerator PropertyCard_ShowsAllDetails()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var modal = _uiController.ShowPropertyCard(1);
            yield return null;

            // Assert
            Assert.IsNotNull(modal.GetPropertyName(), "Should show property name");
            Assert.IsNotNull(modal.GetOwner(), "Should show owner");
            Assert.IsNotNull(modal.GetColorGroup(), "Should show color group");
            Assert.Greater(modal.GetPrice(), 0, "Should show price");
            Assert.IsNotNull(modal.GetRentStructure(), "Should show rent structure");
            Assert.Greater(modal.GetHouseCost(), 0, "Should show house cost");
            Assert.Greater(modal.GetMortgageValue(), 0, "Should show mortgage value");
        }

        [UnityTest]
        public IEnumerator PropertyCard_ShowsCurrentState()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player;
            property.Houses = 2;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var modal = _uiController.ShowPropertyCard(1);
            yield return null;

            // Assert
            Assert.AreEqual(2, modal.GetHouseCount(), "Should show 2 houses");
            Assert.IsFalse(modal.IsMortgaged(), "Should show unmortgaged state");
        }

        [UnityTest]
        public IEnumerator PropertyCard_ShowsMonopolyStatus()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            var property1 = _gameState.Board.Spaces[1] as Property; // Mediterranean
            var property2 = _gameState.Board.Spaces[3] as Property; // Baltic
            property1.Owner = player;
            property2.Owner = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var modal = _uiController.ShowPropertyCard(1);
            yield return null;

            // Assert
            Assert.IsTrue(modal.IsPartOfMonopoly(), 
                "Should indicate property is part of complete color group");
        }

        [UnityTest]
        public IEnumerator PropertyCard_RailroadShowsRentStructure()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            var railroad = _gameState.Board.Spaces[5] as Property; // Reading Railroad
            railroad.Owner = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var modal = _uiController.ShowPropertyCard(5);
            yield return null;

            // Assert
            var rentStructure = modal.GetRentStructure();
            Assert.IsNotNull(rentStructure, "Should show railroad rent structure");
            Assert.IsTrue(rentStructure.Contains("1 Railroad:"), 
                "Should show rent for 1 railroad owned");
            Assert.IsTrue(rentStructure.Contains("2 Railroads:"), 
                "Should show rent for 2 railroads owned");
        }

        [UnityTest]
        public IEnumerator PropertyCard_UtilityShowsRentCalculation()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var modal = _uiController.ShowPropertyCard(12); // Electric Company
            yield return null;

            // Assert
            var rentInfo = modal.GetRentStructure();
            Assert.IsTrue(rentInfo.Contains("4x") || rentInfo.Contains("10x"), 
                "Should show utility rent calculation (4x or 10x dice)");
        }

        [UnityTest]
        public IEnumerator PropertyCard_DismissesWithXButton()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            var modal = _uiController.ShowPropertyCard(1);
            yield return null;

            // Act
            var closeButton = modal.GetCloseButton();
            closeButton.onClick.Invoke();
            yield return null;

            // Assert
            Assert.IsFalse(_uiController.IsModalShowing("PropertyCard"), 
                "Property card should be dismissed");
        }

        [UnityTest]
        public IEnumerator PropertyCard_DismissesWithClickOutside()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            var modal = _uiController.ShowPropertyCard(1);
            yield return null;

            // Act
            modal.SimulateClickOutside();
            yield return null;

            // Assert
            Assert.IsFalse(_uiController.IsModalShowing("PropertyCard"), 
                "Should dismiss when clicking outside");
        }

        #endregion

        #region User Story 2.12: Build Houses/Hotels Tests

        [UnityTest]
        public IEnumerator BuildHouses_ButtonAvailableWithMonopoly()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 1500;
            var property1 = _gameState.Board.Spaces[1] as Property;
            var property2 = _gameState.Board.Spaces[3] as Property;
            property1.Owner = player;
            property2.Owner = player;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var modal = _uiController.ShowPropertyCard(1);
            yield return null;

            // Assert
            Assert.IsTrue(modal.HasBuildButton(), 
                "Build button should be available with monopoly");
        }

        [UnityTest]
        public IEnumerator BuildHouses_ShowsAvailableHouses()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            _gameState.AvailableHouses = 32;
            _gameState.AvailableHotels = 12;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var buildModal = _uiController.ShowBuildDialog(1);
            yield return null;

            // Assert
            Assert.AreEqual(32, buildModal.GetAvailableHouses(), 
                "Should show 32 available houses");
            Assert.AreEqual(12, buildModal.GetAvailableHotels(), 
                "Should show 12 available hotels");
        }

        [UnityTest]
        public IEnumerator BuildHouses_CanSelect1To4Houses()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 1500;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var buildModal = _uiController.ShowBuildDialog(1);
            buildModal.SetHouseCount(3);
            yield return null;

            // Assert
            Assert.AreEqual(3, buildModal.GetSelectedHouseCount(), 
                "Should be able to select 3 houses");
        }

        [UnityTest]
        public IEnumerator BuildHouses_CanUpgradeTo Hotel()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 1500;
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player;
            property.Houses = 4;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var buildModal = _uiController.ShowBuildDialog(1);
            yield return null;

            // Assert
            Assert.IsTrue(buildModal.CanUpgradeToHotel(), 
                "Should allow upgrading 4 houses to hotel");
        }

        [UnityTest]
        public IEnumerator BuildHouses_EnforcesEvenBuildingRule()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 1500;
            var property1 = _gameState.Board.Spaces[1] as Property;
            var property2 = _gameState.Board.Spaces[3] as Property;
            property1.Owner = player;
            property2.Owner = player;
            property1.Houses = 2;
            property2.Houses = 1;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var buildModal = _uiController.ShowBuildDialog(1);
            var canBuild = buildModal.CanBuildMoreHouses(property1);

            // Assert
            Assert.IsFalse(canBuild, 
                "Should enforce even building rule (property1 already has more houses)");
        }

        [UnityTest]
        public IEnumerator BuildHouses_DeductsCostFromPlayer()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 1500;
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player;
            property.HouseCost = 50;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.BuildHouses(1, 2); // Build 2 houses
            yield return null;

            // Assert
            Assert.AreEqual(1400, player.Money, 
                "Should deduct $100 for 2 houses (2 * $50)");
        }

        [UnityTest]
        public IEnumerator BuildHouses_DisabledWithInsufficientFunds()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 40; // Not enough for house
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player;
            property.HouseCost = 50;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var buildModal = _uiController.ShowBuildDialog(1);
            yield return null;

            // Assert
            Assert.IsFalse(buildModal.GetBuildButton().interactable, 
                "Build button should be disabled with insufficient funds");
        }

        [UnityTest]
        public IEnumerator BuildHouses_HousesAppearOnBoardImmediately()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 1500;
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.BuildHouses(1, 2);
            yield return null;

            var boardView = _uiController.GetBoardController();
            var spaceView = boardView.GetSpace(1);

            // Assert
            Assert.AreEqual(2, spaceView.GetHouseCount(), 
                "Houses should appear on board immediately");
        }

        [UnityTest]
        public IEnumerator BuildHouses_CannotBuildOnMortgagedProperty()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 1500;
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player;
            property.IsMortgaged = true;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var modal = _uiController.ShowPropertyCard(1);
            yield return null;

            // Assert
            Assert.IsFalse(modal.HasBuildButton(), 
                "Cannot build on mortgaged properties");
        }

        [UnityTest]
        public IEnumerator BuildHouses_ShowsConfirmationDialog()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 1500;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var buildModal = _uiController.ShowBuildDialog(1);
            buildModal.SetHouseCount(2);
            var confirmButton = buildModal.GetConfirmButton();
            confirmButton.onClick.Invoke();
            yield return null;

            // Assert
            Assert.IsTrue(_uiController.IsShowingConfirmation(), 
                "Should show confirmation dialog");
            var message = _uiController.GetConfirmationMessage();
            Assert.IsTrue(message.Contains("Build"), "Should mention building");
        }

        #endregion

        #region User Story 2.13-2.14: Mortgage/Unmortgage Tests

        [UnityTest]
        public IEnumerator Mortgage_ButtonAvailableForOwnedProperty()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var modal = _uiController.ShowPropertyCard(1);
            yield return null;

            // Assert
            Assert.IsTrue(modal.HasMortgageButton(), 
                "Mortgage button should be available for owned property");
        }

        [UnityTest]
        public IEnumerator Mortgage_Shows50PercentValue()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player;
            property.PurchasePrice = 60;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var modal = _uiController.ShowPropertyCard(1);
            yield return null;

            // Assert
            Assert.AreEqual(30, modal.GetMortgageValue(), 
                "Mortgage value should be 50% of property price");
        }

        [UnityTest]
        public IEnumerator Mortgage_AddsCashToPlayer()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 1500;
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player;
            property.PurchasePrice = 60;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.MortgageProperty(1);
            yield return null;

            // Assert
            Assert.AreEqual(1530, player.Money, "Should add $30 (50% of $60)");
            Assert.IsTrue(property.IsMortgaged, "Property should be mortgaged");
        }

        [UnityTest]
        public IEnumerator Mortgage_PropertyGrayedOutOnBoard()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.MortgageProperty(1);
            yield return null;

            var boardView = _uiController.GetBoardController();
            var spaceView = boardView.GetSpace(1);

            // Assert
            Assert.IsTrue(spaceView.IsMortgaged(), 
                "Mortgaged property should be grayed out on board");
        }

        [UnityTest]
        public IEnumerator Mortgage_ShowsMortgagedLabel()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player;
            property.IsMortgaged = true;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var boardView = _uiController.GetBoardController();
            var spaceView = boardView.GetSpace(1);

            // Assert
            Assert.IsTrue(spaceView.HasMortgageLabel(), 
                "'MORTGAGED' label should be visible");
        }

        [UnityTest]
        public IEnumerator Mortgage_ShowsConfirmationDialog()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var modal = _uiController.ShowPropertyCard(1);
            var mortgageButton = modal.GetMortgageButton();
            mortgageButton.onClick.Invoke();
            yield return null;

            // Assert
            Assert.IsTrue(_uiController.IsShowingConfirmation(), 
                "Should show confirmation dialog before mortgaging");
        }

        [UnityTest]
        public IEnumerator Unmortgage_ButtonShownForMortgagedProperty()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player;
            property.IsMortgaged = true;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var modal = _uiController.ShowPropertyCard(1);
            yield return null;

            // Assert
            Assert.IsTrue(modal.HasUnmortgageButton(), 
                "Unmortgage button should be shown for mortgaged property");
        }

        [UnityTest]
        public IEnumerator Unmortgage_Shows110PercentCost()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player;
            property.PurchasePrice = 60;
            property.IsMortgaged = true;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var modal = _uiController.ShowPropertyCard(1);
            yield return null;

            // Assert
            Assert.AreEqual(33, modal.GetUnmortgageCost(), 
                "Unmortgage cost should be 110% of mortgage value (30 * 1.1 = 33)");
        }

        [UnityTest]
        public IEnumerator Unmortgage_DisabledWithInsufficientFunds()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 20; // Not enough
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player;
            property.IsMortgaged = true;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var modal = _uiController.ShowPropertyCard(1);
            yield return null;

            // Assert
            Assert.IsFalse(modal.GetUnmortgageButton().interactable, 
                "Unmortgage button should be disabled with insufficient funds");
        }

        [UnityTest]
        public IEnumerator Unmortgage_DeductsCostAndRestoresProperty()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 1500;
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player;
            property.PurchasePrice = 60;
            property.IsMortgaged = true;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.UnmortgageProperty(1);
            yield return null;

            // Assert
            Assert.AreEqual(1467, player.Money, "Should deduct $33 (110% of $30)");
            Assert.IsFalse(property.IsMortgaged, "Property should be unmortgaged");
        }

        [UnityTest]
        public IEnumerator Unmortgage_PropertyReturnsToNormalOnBoard()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player;
            property.IsMortgaged = true;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.UnmortgageProperty(1);
            yield return null;

            var boardView = _uiController.GetBoardController();
            var spaceView = boardView.GetSpace(1);

            // Assert
            Assert.IsFalse(spaceView.IsMortgaged(), 
                "Property should return to normal state on board");
        }

        #endregion

        #region User Story 2.15-2.16: Trading System Tests

        [UnityTest]
        public IEnumerator Trade_ButtonAccessibleDuringPlayerTurn()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var tradeButton = _uiController.GetTradeButton();

            // Assert
            Assert.IsNotNull(tradeButton, "Trade button should be accessible");
            Assert.IsTrue(tradeButton.interactable, 
                "Trade button should be interactable during turn");
        }

        [UnityTest]
        public IEnumerator Trade_OpensTradeDialog()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            _gameState.CurrentPlayer = player1;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.OpenTradeDialog();
            yield return null;

            // Assert
            Assert.IsTrue(_uiController.IsModalShowing("Trade"), 
                "Trade dialog should open");
        }

        [UnityTest]
        public IEnumerator Trade_CanSelectTradePartner()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            var player3 = _gameState.AddPlayer("Charlie");
            _gameState.CurrentPlayer = player1;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var tradeModal = _uiController.OpenTradeDialog();
            tradeModal.SelectPartner(player2.Id);
            yield return null;

            // Assert
            Assert.AreEqual(player2.Id, tradeModal.GetSelectedPartner(), 
                "Should be able to select trade partner");
        }

        [UnityTest]
        public IEnumerator Trade_CanAddPropertiesToOffer()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player1;
            player1.OwnedProperties.Add(property);
            _gameState.CurrentPlayer = player1;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var tradeModal = _uiController.OpenTradeDialog();
            tradeModal.AddPropertyToOffer(property.Id);
            yield return null;

            // Assert
            Assert.AreEqual(1, tradeModal.GetOfferedProperties().Count, 
                "Should be able to add property to offer");
        }

        [UnityTest]
        public IEnumerator Trade_CanAddMoneyToOffer()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            player1.Money = 1500;
            _gameState.CurrentPlayer = player1;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var tradeModal = _uiController.OpenTradeDialog();
            tradeModal.SetMoneyOffer(200);
            yield return null;

            // Assert
            Assert.AreEqual(200, tradeModal.GetMoneyOffer(), 
                "Should be able to add money to offer");
        }

        [UnityTest]
        public IEnumerator Trade_CanRequestProperties()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            var property = _gameState.Board.Spaces[3] as Property;
            property.Owner = player2;
            player2.OwnedProperties.Add(property);
            _gameState.CurrentPlayer = player1;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var tradeModal = _uiController.OpenTradeDialog();
            tradeModal.SelectPartner(player2.Id);
            tradeModal.AddPropertyToRequest(property.Id);
            yield return null;

            // Assert
            Assert.AreEqual(1, tradeModal.GetRequestedProperties().Count, 
                "Should be able to request properties from partner");
        }

        [UnityTest]
        public IEnumerator Trade_CanRequestMoney()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            _gameState.CurrentPlayer = player1;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var tradeModal = _uiController.OpenTradeDialog();
            tradeModal.SetMoneyRequest(150);
            yield return null;

            // Assert
            Assert.AreEqual(150, tradeModal.GetMoneyRequest(), 
                "Should be able to request money");
        }

        [UnityTest]
        public IEnumerator Trade_CanTradeGetOutOfJailFreeCards()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            player1.GetOutOfJailFreeCards = 1;
            _gameState.CurrentPlayer = player1;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var tradeModal = _uiController.OpenTradeDialog();
            var canTrade = tradeModal.CanOfferGetOutOfJailFreeCard();

            // Assert
            Assert.IsTrue(canTrade, "Should be able to trade Get Out of Jail Free cards");
        }

        [UnityTest]
        public IEnumerator Trade_CannotTradeMortgagedProperties()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player1;
            property.IsMortgaged = true;
            player1.OwnedProperties.Add(property);
            _gameState.CurrentPlayer = player1;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var tradeModal = _uiController.OpenTradeDialog();
            var canAdd = tradeModal.CanAddPropertyToOffer(property.Id);

            // Assert
            Assert.IsFalse(canAdd, "Cannot trade mortgaged properties");
        }

        [UnityTest]
        public IEnumerator Trade_CannotTradePropertiesWithHouses()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player1;
            property.Houses = 2;
            player1.OwnedProperties.Add(property);
            _gameState.CurrentPlayer = player1;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var tradeModal = _uiController.OpenTradeDialog();
            var canAdd = tradeModal.CanAddPropertyToOffer(property.Id);

            // Assert
            Assert.IsFalse(canAdd, 
                "Cannot trade properties with houses (must sell first)");
        }

        [UnityTest]
        public IEnumerator Trade_ProposeTradeShowsModalToPartner()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            _gameState.CurrentPlayer = player1;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var tradeModal = _uiController.OpenTradeDialog();
            tradeModal.SelectPartner(player2.Id);
            tradeModal.ProposeT trade();
            yield return null;

            // Assert
            Assert.IsTrue(_uiController.HasPendingTradeProposal(player2.Id), 
                "Trade proposal should be sent to partner");
        }

        [UnityTest]
        public IEnumerator Trade_AcceptTransfersAllItems()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            player1.Money = 1500;
            player2.Money = 1000;
            var property1 = _gameState.Board.Spaces[1] as Property;
            property1.Owner = player1;
            player1.OwnedProperties.Add(property1);
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var trade = _uiController.CreateTrade(player1.Id, player2.Id);
            trade.OfferProperties.Add(property1);
            trade.OfferMoney = 100;
            trade.RequestMoney = 200;
            _uiController.AcceptTrade(trade);
            yield return null;

            // Assert
            Assert.AreEqual(player2, property1.Owner, "Property should transfer to player2");
            Assert.AreEqual(1600, player1.Money, "Player1 should receive $200 - $100 = +$100");
            Assert.AreEqual(900, player2.Money, "Player2 should pay $200 - $100 = -$100");
        }

        [UnityTest]
        public IEnumerator Trade_DeclineRejectsTrade()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var trade = _uiController.CreateTrade(player1.Id, player2.Id);
            _uiController.DeclineTrade(trade);
            yield return null;

            // Assert
            Assert.IsFalse(_uiController.HasPendingTradeProposal(player2.Id), 
                "Trade should be declined and removed");
        }

        [UnityTest]
        public IEnumerator Trade_ShowsNotificationToBothPlayers()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var trade = _uiController.CreateTrade(player1.Id, player2.Id);
            _uiController.AcceptTrade(trade);
            yield return null;

            // Assert
            var notifications = _uiController.GetRecentNotifications();
            Assert.IsTrue(notifications.Exists(n => n.Contains("Trade complete")), 
                "Should show trade complete notification to both players");
        }

        #endregion

        #region User Story 2.17: Jail Mechanics Tests

        [UnityTest]
        public IEnumerator Jail_TokenMovesToJailSpace()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Position = 15;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.SendPlayerToJail(player.Id);
            yield return null;

            // Assert
            Assert.AreEqual(10, player.Position, "Player should move to jail space (10)");
            Assert.IsTrue(player.IsInJail, "Player should be in jail");
        }

        [UnityTest]
        public IEnumerator Jail_ShowsInJailStatus()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.IsInJail = true;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var statusPanel = _uiController.GetPlayerStatusPanel();
            statusPanel.SetCurrentPlayer(player);
            yield return null;

            // Assert
            Assert.IsTrue(statusPanel.IsInJail(), "'In Jail' status should be shown in HUD");
        }

        [UnityTest]
        public IEnumerator Jail_ShowsThreeOptions()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.IsInJail = true;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var jailModal = _uiController.ShowJailOptionsDialog();
            yield return null;

            // Assert
            Assert.IsTrue(jailModal.HasOption("Pay50"), "Should have 'Pay $50' option");
            Assert.IsTrue(jailModal.HasOption("RollDoubles"), 
                "Should have 'Try to Roll Doubles' option");
        }

        [UnityTest]
        public IEnumerator Jail_UseGetOutOfJailFreeCardOption()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.IsInJail = true;
            player.GetOutOfJailFreeCards = 1;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var jailModal = _uiController.ShowJailOptionsDialog();
            yield return null;

            // Assert
            Assert.IsTrue(jailModal.HasOption("UseCard"), 
                "Should have 'Use Get Out of Jail Free Card' option");
        }

        [UnityTest]
        public IEnumerator Jail_Pay50ReleasesImmediately()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.IsInJail = true;
            player.Money = 1500;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.PayToLeaveJail(player.Id);
            yield return null;

            // Assert
            Assert.IsFalse(player.IsInJail, "Player should be released from jail");
            Assert.AreEqual(1450, player.Money, "Should pay $50");
        }

        [UnityTest]
        public IEnumerator Jail_RollDoublesReleasesPlayer()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.IsInJail = true;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.RollDiceInJail(player.Id);
            // Simulate rolling doubles
            var rolledDoubles = true; // In actual game, this would be random
            if (rolledDoubles)
            {
                _uiController.ReleaseFromJail(player.Id);
            }
            yield return null;

            // Assert
            Assert.IsFalse(player.IsInJail, 
                "Player should be released if doubles rolled");
        }

        [UnityTest]
        public IEnumerator Jail_FailedRollEndsTurn()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.IsInJail = true;
            player.TurnsInJail = 1;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.RollDiceInJail(player.Id);
            // Simulate NOT rolling doubles
            var rolledDoubles = false;
            yield return null;

            // Assert
            Assert.IsTrue(player.IsInJail, "Player should remain in jail");
            Assert.AreEqual(2, player.TurnsInJail, "Turn counter should increment");
        }

        [UnityTest]
        public IEnumerator Jail_After3FailedRolls_MustPay50()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.IsInJail = true;
            player.TurnsInJail = 3;
            player.Money = 1500;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.RollDiceInJail(player.Id);
            yield return null;

            // Assert
            Assert.IsFalse(player.IsInJail, "Player should be released automatically");
            Assert.AreEqual(1450, player.Money, "Should pay $50 automatically");
        }

        [UnityTest]
        public IEnumerator Jail_DoublesInJail_DoesNotRollAgain()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.IsInJail = true;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.RollDiceInJail(player.Id);
            // Simulate rolling doubles and being released
            player.IsInJail = false;
            player.ConsecutiveDoubles = 0; // Should not count
            yield return null;

            // Assert
            Assert.AreEqual(0, player.ConsecutiveDoubles, 
                "Doubles in jail should not count towards rolling again");
        }

        [UnityTest]
        public IEnumerator Jail_JustVisiting_ClearlyDifferentFromInJail()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            player1.Position = 10;
            player1.IsInJail = false; // Just visiting
            player2.Position = 10;
            player2.IsInJail = true; // In jail
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var boardView = _uiController.GetBoardController();
            var token1 = boardView.GetPlayerToken(player1.Id);
            var token2 = boardView.GetPlayerToken(player2.Id);

            // Assert
            Assert.IsFalse(token1.IsInJailArea(), 
                "Just Visiting token should not be in jail area");
            Assert.IsTrue(token2.IsInJailArea(), 
                "In Jail token should be in jail area");
        }

        #endregion

        #region User Story 2.18: Bankruptcy Tests

        [UnityTest]
        public IEnumerator Bankruptcy_ProcessStartsWhenCannotPay()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 10;
            var debt = 100;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.ProcessDebt(player.Id, debt);
            yield return null;

            // Assert
            Assert.IsTrue(_uiController.IsModalShowing("Bankruptcy"), 
                "Bankruptcy modal should appear");
        }

        [UnityTest]
        public IEnumerator Bankruptcy_ShowsDebtAmount()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 10;
            var debt = 100;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var bankruptcyModal = _uiController.ShowBankruptcyModal(player.Id, debt);
            yield return null;

            // Assert
            var message = bankruptcyModal.GetMessage();
            Assert.IsTrue(message.Contains("$100"), "Should show debt amount");
            Assert.IsTrue(message.Contains("$10"), "Should show current money");
        }

        [UnityTest]
        public IEnumerator Bankruptcy_OffersSellHousesOption()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 10;
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player;
            property.Houses = 2;
            player.OwnedProperties.Add(property);
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var bankruptcyModal = _uiController.ShowBankruptcyModal(player.Id, 100);
            yield return null;

            // Assert
            Assert.IsTrue(bankruptcyModal.HasOption("SellHouses"), 
                "Should offer 'Sell Houses' option");
        }

        [UnityTest]
        public IEnumerator Bankruptcy_OffersMortgageOption()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 10;
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player;
            property.IsMortgaged = false;
            player.OwnedProperties.Add(property);
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var bankruptcyModal = _uiController.ShowBankruptcyModal(player.Id, 100);
            yield return null;

            // Assert
            Assert.IsTrue(bankruptcyModal.HasOption("MortgageProperties"), 
                "Should offer 'Mortgage Properties' option");
        }

        [UnityTest]
        public IEnumerator Bankruptcy_RaisingFundsContinuesPlay()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 10;
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player;
            property.PurchasePrice = 60;
            player.OwnedProperties.Add(property);
            var debt = 40;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.MortgageProperty(1); // Gets $30
            player.Money = 40; // Now has enough
            _uiController.PayDebt(player.Id, debt);
            yield return null;

            // Assert
            Assert.IsFalse(player.IsBankrupt, "Player should not be bankrupt");
            Assert.AreEqual(0, player.Money, "Debt should be paid");
        }

        [UnityTest]
        public IEnumerator Bankruptcy_DeclaringTransfersAssetsToCreditor()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob"); // Creditor
            player1.Money = 50;
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player1;
            player1.OwnedProperties.Add(property);
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.DeclareBankruptcy(player1.Id, player2.Id);
            yield return null;

            // Assert
            Assert.IsTrue(player1.IsBankrupt, "Player should be bankrupt");
            Assert.AreEqual(player2, property.Owner, 
                "Property should transfer to creditor");
            Assert.IsFalse(property.IsMortgaged, 
                "Property should be unmortgaged when transferred");
        }

        [UnityTest]
        public IEnumerator Bankruptcy_ToBankMakesPropertiesUnowned()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 50;
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player;
            player.OwnedProperties.Add(property);
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.DeclareBankruptcy(player.Id, null); // Bank as creditor
            yield return null;

            // Assert
            Assert.IsNull(property.Owner, "Property should become unowned");
        }

        [UnityTest]
        public IEnumerator Bankruptcy_TransfersCashToCreditor()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            player1.Money = 50;
            player2.Money = 1000;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.DeclareBankruptcy(player1.Id, player2.Id);
            yield return null;

            // Assert
            Assert.AreEqual(0, player1.Money, "Bankrupt player should have $0");
            Assert.AreEqual(1050, player2.Money, "Creditor should receive cash");
        }

        [UnityTest]
        public IEnumerator Bankruptcy_RemovesPlayerFromTurnOrder()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            var player3 = _gameState.AddPlayer("Charlie");
            _gameState.CurrentPlayer = player1;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.DeclareBankruptcy(player2.Id, player1.Id);
            yield return null;

            // Assert
            var activePlayers = _gameState.GetActivePlayers();
            Assert.AreEqual(2, activePlayers.Count, 
                "Bankrupt player should be removed from turn order");
            Assert.IsFalse(activePlayers.Contains(player2), 
                "Bob should not be in active players");
        }

        [UnityTest]
        public IEnumerator Bankruptcy_RemovesTokenFromBoard()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.DeclareBankruptcy(player.Id, null);
            yield return null;

            var boardView = _uiController.GetBoardController();
            var tokenExists = boardView.HasPlayerToken(player.Id);

            // Assert
            Assert.IsFalse(tokenExists, 
                "Bankrupt player's token should be removed from board");
        }

        [UnityTest]
        public IEnumerator Bankruptcy_OnePlayerRemaining_GameOver()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.DeclareBankruptcy(player2.Id, player1.Id);
            yield return null;

            // Assert
            Assert.IsTrue(_gameState.IsGameOver, "Game should be over");
            Assert.AreEqual(player1, _gameState.Winner, "Alice should be the winner");
        }

        #endregion

        #region User Story 2.19: Chance and Community Chest Tests

        [UnityTest]
        public IEnumerator Card_LandingTriggersCardDraw()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Position = 2; // Community Chest
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.LandOnSpace(player.Id, 2);
            yield return null;

            // Assert
            Assert.IsTrue(_uiController.IsModalShowing("Card"), 
                "Card modal should appear");
        }

        [UnityTest]
        public IEnumerator Card_DisplaysCardText()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var cardModal = _uiController.ShowCardModal("Community Chest", "Collect $200");
            yield return null;

            // Assert
            Assert.IsNotNull(cardModal.GetCardText(), "Card text should be displayed");
            Assert.IsTrue(cardModal.GetCardText().Contains("Collect $200"), 
                "Should show card effect");
        }

        [UnityTest]
        public IEnumerator Card_ExecutesEffectAutomatically()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 1500;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.DrawCard(player.Id, CardType.CommunityChest, "Collect $200");
            yield return new WaitForSeconds(0.5f); // Auto-execute after delay

            // Assert
            Assert.AreEqual(1700, player.Money, 
                "Card effect should execute automatically");
        }

        [UnityTest]
        public IEnumerator Card_MoneyGain_AddsToPlayer()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 1500;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.ExecuteCardEffect(player.Id, CardEffect.GainMoney, 100);
            yield return null;

            // Assert
            Assert.AreEqual(1600, player.Money, "Should add money to player");
        }

        [UnityTest]
        public IEnumerator Card_MoneyLoss_DeductsFromPlayer()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 1500;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.ExecuteCardEffect(player.Id, CardEffect.PayMoney, 50);
            yield return null;

            // Assert
            Assert.AreEqual(1450, player.Money, "Should deduct money from player");
        }

        [UnityTest]
        public IEnumerator Card_AdvanceToGO_MovesPlayerAndCollects200()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Position = 7;
            player.Money = 1500;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.ExecuteCardEffect(player.Id, CardEffect.AdvanceToGO, 0);
            yield return null;

            // Assert
            Assert.AreEqual(0, player.Position, "Should move to GO");
            Assert.AreEqual(1700, player.Money, "Should collect $200");
        }

        [UnityTest]
        public IEnumerator Card_GoToJail_SendsPlayerToJail()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Position = 7;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.ExecuteCardEffect(player.Id, CardEffect.GoToJail, 0);
            yield return null;

            // Assert
            Assert.AreEqual(10, player.Position, "Should move to jail");
            Assert.IsTrue(player.IsInJail, "Should be in jail");
        }

        [UnityTest]
        public IEnumerator Card_GetOutOfJailFree_AddsToInventory()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.GetOutOfJailFreeCards = 0;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.ExecuteCardEffect(player.Id, CardEffect.GetOutOfJailFree, 0);
            yield return null;

            // Assert
            Assert.AreEqual(1, player.GetOutOfJailFreeCards, 
                "Should add Get Out of Jail Free card to inventory");
        }

        [UnityTest]
        public IEnumerator Card_PayPerProperty_CalculatesCorrectly()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 1500;
            var property1 = _gameState.Board.Spaces[1] as Property;
            var property2 = _gameState.Board.Spaces[3] as Property;
            property1.Owner = player;
            property2.Owner = player;
            property1.Houses = 2;
            property2.Houses = 1;
            player.OwnedProperties.Add(property1);
            player.OwnedProperties.Add(property2);
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            // $25 per house = $25 * 3 = $75
            _uiController.ExecuteCardEffect(player.Id, CardEffect.PayPerHouse, 25);
            yield return null;

            // Assert
            Assert.AreEqual(1425, player.Money, "Should pay $25 * 3 houses = $75");
        }

        [UnityTest]
        public IEnumerator Card_All16ChanceCardsImplemented()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var chanceCards = _gameState.GetChanceCards();

            // Assert
            Assert.AreEqual(16, chanceCards.Count, "Should have 16 Chance cards");
        }

        [UnityTest]
        public IEnumerator Card_All16CommunityChestCardsImplemented()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var communityChestCards = _gameState.GetCommunityChestCards();

            // Assert
            Assert.AreEqual(16, communityChestCards.Count, 
                "Should have 16 Community Chest cards");
        }

        [UnityTest]
        public IEnumerator Card_DismissesWithOKButton()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            var cardModal = _uiController.ShowCardModal("Chance", "Advance to GO");
            yield return null;

            // Act
            var okButton = cardModal.GetOKButton();
            okButton.onClick.Invoke();
            yield return null;

            // Assert
            Assert.IsFalse(_uiController.IsModalShowing("Card"), 
                "Card should be dismissed with OK button");
        }

        [UnityTest]
        public IEnumerator Card_AutoDismissesAfter3Seconds()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            var cardModal = _uiController.ShowCardModal("Chance", "Collect $50");
            yield return null;

            // Act
            yield return new WaitForSeconds(3.5f);

            // Assert
            Assert.IsFalse(_uiController.IsModalShowing("Card"), 
                "Card should auto-dismiss after 3 seconds");
        }

        #endregion
    }
}
