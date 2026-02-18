using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using MonopolyFrenzy.UI.Panels;
using MonopolyFrenzy.UI.Controllers;
using MonopolyFrenzy.Core;
using MonopolyFrenzy.Events;

namespace MonopolyFrenzy.Tests.UI
{
    /// <summary>
    /// Test Suite for User Stories 2.5-2.10: Player HUD, Turn Controls, and Actions
    /// 
    /// Validates:
    /// - Player status display (money, properties, status)
    /// - Other players summary
    /// - Roll dice functionality
    /// - Property purchase flow
    /// - Rent payment
    /// - End turn mechanics
    /// </summary>
    [TestFixture]
    public class PlayerHUDAndTurnControlTests
    {
        private GameObject _testCanvas;
        private PlayerStatusPanel _statusPanel;
        private TurnControlPanel _turnControlPanel;
        private GameState _gameState;
        private UIController _uiController;

        [SetUp]
        public void Setup()
        {
            _testCanvas = new GameObject("TestCanvas");
            var canvas = _testCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _testCanvas.AddComponent<GraphicRaycaster>();

            var statusObject = new GameObject("PlayerStatusPanel");
            statusObject.transform.SetParent(_testCanvas.transform);
            _statusPanel = statusObject.AddComponent<PlayerStatusPanel>();

            var turnControlObject = new GameObject("TurnControlPanel");
            turnControlObject.transform.SetParent(_testCanvas.transform);
            _turnControlPanel = turnControlObject.AddComponent<TurnControlPanel>();

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

        #region User Story 2.5: Player Status Display Tests

        [UnityTest]
        public IEnumerator PlayerStatus_DisplaysMoneyProminently()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 1500;
            _statusPanel.Initialize(_gameState);
            yield return null;

            // Act
            _statusPanel.SetCurrentPlayer(player);
            yield return null;

            // Assert
            var moneyDisplay = _statusPanel.GetMoneyDisplay();
            Assert.IsNotNull(moneyDisplay, "Money display should exist");
            Assert.AreEqual("$1500", moneyDisplay.text, "Money should be displayed prominently");
        }

        [UnityTest]
        public IEnumerator PlayerStatus_ShowsPlayerNameAndToken()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice", "Car");
            _statusPanel.Initialize(_gameState);
            yield return null;

            // Act
            _statusPanel.SetCurrentPlayer(player);
            yield return null;

            // Assert
            Assert.AreEqual("Alice", _statusPanel.GetPlayerName(), 
                "Player name should be displayed");
            Assert.AreEqual("Car", _statusPanel.GetPlayerToken(), 
                "Player token should be displayed");
        }

        [UnityTest]
        public IEnumerator PlayerStatus_ListsOwnedProperties()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            var property1 = _gameState.Board.Spaces[1] as Property;
            var property2 = _gameState.Board.Spaces[3] as Property;
            property1.Owner = player;
            property2.Owner = player;
            player.OwnedProperties.Add(property1);
            player.OwnedProperties.Add(property2);
            _statusPanel.Initialize(_gameState);
            yield return null;

            // Act
            _statusPanel.SetCurrentPlayer(player);
            yield return null;

            // Assert
            var properties = _statusPanel.GetPropertyList();
            Assert.AreEqual(2, properties.Count, "Should display 2 owned properties");
        }

        [UnityTest]
        public IEnumerator PlayerStatus_ShowsPropertyCount()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            var property1 = _gameState.Board.Spaces[1] as Property;
            var property2 = _gameState.Board.Spaces[3] as Property;
            property1.Owner = player;
            property2.Owner = player;
            player.OwnedProperties.Add(property1);
            player.OwnedProperties.Add(property2);
            _statusPanel.Initialize(_gameState);
            yield return null;

            // Act
            _statusPanel.SetCurrentPlayer(player);
            yield return null;

            // Assert
            Assert.AreEqual(2, _statusPanel.GetPropertyCount(), 
                "Property count should be displayed");
        }

        [UnityTest]
        public IEnumerator PlayerStatus_ShowsGetOutOfJailFreeCards()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.GetOutOfJailFreeCards = 1;
            _statusPanel.Initialize(_gameState);
            yield return null;

            // Act
            _statusPanel.SetCurrentPlayer(player);
            yield return null;

            // Assert
            Assert.IsTrue(_statusPanel.HasGetOutOfJailFreeCard(), 
                "Should display Get Out of Jail Free card");
        }

        [UnityTest]
        public IEnumerator PlayerStatus_ShowsJailStatus()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.IsInJail = true;
            player.TurnsInJail = 2;
            _statusPanel.Initialize(_gameState);
            yield return null;

            // Act
            _statusPanel.SetCurrentPlayer(player);
            yield return null;

            // Assert
            Assert.IsTrue(_statusPanel.IsInJail(), "Should show player in jail");
            Assert.AreEqual(2, _statusPanel.GetTurnsInJail(), 
                "Should show turns remaining in jail");
        }

        [UnityTest]
        public IEnumerator PlayerStatus_ShowsNetWorth()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 1500;
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player;
            property.PurchasePrice = 60;
            player.OwnedProperties.Add(property);
            _statusPanel.Initialize(_gameState);
            yield return null;

            // Act
            _statusPanel.SetCurrentPlayer(player);
            yield return null;

            // Assert
            var netWorth = _statusPanel.GetNetWorth();
            Assert.GreaterOrEqual(netWorth, 1560, 
                "Net worth should include money and property values");
        }

        [UnityTest]
        public IEnumerator PlayerStatus_ShowsBankruptcyStatus()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.IsBankrupt = true;
            _statusPanel.Initialize(_gameState);
            yield return null;

            // Act
            _statusPanel.SetCurrentPlayer(player);
            yield return null;

            // Assert
            Assert.IsTrue(_statusPanel.IsBankrupt(), 
                "Bankruptcy status should be clearly visible");
        }

        [UnityTest]
        public IEnumerator PlayerStatus_UpdatesImmediatelyOnMoneyChange()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 1500;
            _statusPanel.Initialize(_gameState);
            _statusPanel.SetCurrentPlayer(player);
            yield return null;

            // Act
            player.Money = 1300;
            _statusPanel.UpdateDisplay();
            yield return null;

            // Assert
            var moneyDisplay = _statusPanel.GetMoneyDisplay();
            Assert.AreEqual("$1300", moneyDisplay.text, 
                "Money display should update immediately");
        }

        [UnityTest]
        public IEnumerator PlayerStatus_VisibleAtAllTimesDuringGameplay()
        {
            // Arrange
            _statusPanel.Initialize(_gameState);
            yield return null;

            // Act
            var isVisible = _statusPanel.gameObject.activeSelf;

            // Assert
            Assert.IsTrue(isVisible, "HUD should be visible at all times during gameplay");
        }

        #endregion

        #region User Story 2.6: Other Players Summary Tests

        [UnityTest]
        public IEnumerator OtherPlayersSummary_ListsAllPlayers()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            var player3 = _gameState.AddPlayer("Charlie");
            _statusPanel.Initialize(_gameState);
            yield return null;

            // Act
            var playerList = _statusPanel.GetOtherPlayersList();

            // Assert
            Assert.AreEqual(3, playerList.Count, "Should list all 3 players");
        }

        [UnityTest]
        public IEnumerator OtherPlayersSummary_ShowsMoneyAmount()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            player1.Money = 1500;
            var player2 = _gameState.AddPlayer("Bob");
            player2.Money = 800;
            _statusPanel.Initialize(_gameState);
            yield return null;

            // Act
            var playerInfo = _statusPanel.GetPlayerInfo(player2.Id);

            // Assert
            Assert.AreEqual(800, playerInfo.Money, "Should show player's money amount");
        }

        [UnityTest]
        public IEnumerator OtherPlayersSummary_ShowsPropertyCount()
        {
            // Arrange
            var player = _gameState.AddPlayer("Bob");
            var property1 = _gameState.Board.Spaces[1] as Property;
            var property2 = _gameState.Board.Spaces[3] as Property;
            property1.Owner = player;
            property2.Owner = player;
            player.OwnedProperties.Add(property1);
            player.OwnedProperties.Add(property2);
            _statusPanel.Initialize(_gameState);
            yield return null;

            // Act
            var playerInfo = _statusPanel.GetPlayerInfo(player.Id);

            // Assert
            Assert.AreEqual(2, playerInfo.PropertyCount, 
                "Should show player's property count");
        }

        [UnityTest]
        public IEnumerator OtherPlayersSummary_HighlightsCurrentPlayer()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            _gameState.CurrentPlayer = player1;
            _statusPanel.Initialize(_gameState);
            yield return null;

            // Act
            var isHighlighted = _statusPanel.IsPlayerHighlighted(player1.Id);

            // Assert
            Assert.IsTrue(isHighlighted, "Current player should be highlighted in list");
        }

        [UnityTest]
        public IEnumerator OtherPlayersSummary_ShowsBankruptPlayers()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            player2.IsBankrupt = true;
            _statusPanel.Initialize(_gameState);
            yield return null;

            // Act
            var playerInfo = _statusPanel.GetPlayerInfo(player2.Id);

            // Assert
            Assert.IsTrue(playerInfo.IsBankrupt, 
                "Bankrupt players should be marked/grayed out");
        }

        [UnityTest]
        public IEnumerator OtherPlayersSummary_ShowsJailIcon()
        {
            // Arrange
            var player = _gameState.AddPlayer("Bob");
            player.IsInJail = true;
            _statusPanel.Initialize(_gameState);
            yield return null;

            // Act
            var playerInfo = _statusPanel.GetPlayerInfo(player.Id);

            // Assert
            Assert.IsTrue(playerInfo.IsInJail, "Should show jail icon for jailed players");
        }

        [UnityTest]
        public IEnumerator OtherPlayersSummary_ShowsTurnOrder()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            var player3 = _gameState.AddPlayer("Charlie");
            _statusPanel.Initialize(_gameState);
            yield return null;

            // Act
            var turnOrder = _statusPanel.GetTurnOrderDisplay();

            // Assert
            Assert.AreEqual(3, turnOrder.Count, "Should show turn order for all players");
            Assert.AreEqual(0, turnOrder[player1.Id], "Alice should be first");
            Assert.AreEqual(1, turnOrder[player2.Id], "Bob should be second");
            Assert.AreEqual(2, turnOrder[player3.Id], "Charlie should be third");
        }

        [UnityTest]
        public IEnumerator OtherPlayersSummary_UpdatesImmediately()
        {
            // Arrange
            var player = _gameState.AddPlayer("Bob");
            player.Money = 1500;
            _statusPanel.Initialize(_gameState);
            yield return null;

            // Act
            player.Money = 1000;
            _statusPanel.UpdateDisplay();
            yield return null;

            var playerInfo = _statusPanel.GetPlayerInfo(player.Id);

            // Assert
            Assert.AreEqual(1000, playerInfo.Money, 
                "Player list should update immediately when status changes");
        }

        #endregion

        #region User Story 2.7: Roll Dice Tests

        [UnityTest]
        public IEnumerator RollDice_ButtonVisibleAtStartOfTurn()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            _gameState.CurrentPlayer = player;
            _gameState.TurnPhase = TurnPhase.RollDice;
            _turnControlPanel.Initialize(_gameState, _uiController);
            yield return null;

            // Act
            var rollButton = _turnControlPanel.GetRollDiceButton();

            // Assert
            Assert.IsNotNull(rollButton, "Roll Dice button should be visible");
            Assert.IsTrue(rollButton.interactable, "Roll Dice button should be interactable");
        }

        [UnityTest]
        public IEnumerator RollDice_ButtonDisabledWhenNotPlayerTurn()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            _gameState.CurrentPlayer = player2;
            _turnControlPanel.Initialize(_gameState, _uiController);
            _turnControlPanel.SetViewingPlayer(player1);
            yield return null;

            // Act
            var rollButton = _turnControlPanel.GetRollDiceButton();

            // Assert
            Assert.IsFalse(rollButton.interactable, 
                "Roll Dice button should be disabled when not player's turn");
        }

        [UnityTest]
        public IEnumerator RollDice_DisplaysDiceResultClearly()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            _gameState.CurrentPlayer = player;
            _turnControlPanel.Initialize(_gameState, _uiController);
            yield return null;

            // Act
            _turnControlPanel.DisplayDiceRoll(4, 3);
            yield return null;

            // Assert
            var dice1 = _turnControlPanel.GetDice1Value();
            var dice2 = _turnControlPanel.GetDice2Value();
            var total = _turnControlPanel.GetDiceTotalValue();
            
            Assert.AreEqual(4, dice1, "First die value should be displayed");
            Assert.AreEqual(3, dice2, "Second die value should be displayed");
            Assert.AreEqual(7, total, "Total should be displayed");
        }

        [UnityTest]
        public IEnumerator RollDice_DetectsDoubles()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            _gameState.CurrentPlayer = player;
            _turnControlPanel.Initialize(_gameState, _uiController);
            yield return null;

            // Act
            _turnControlPanel.DisplayDiceRoll(5, 5);
            yield return null;

            // Assert
            Assert.IsTrue(_turnControlPanel.IsShowingDoubles(), 
                "Should show 'You rolled doubles!' message");
        }

        [UnityTest]
        public IEnumerator RollDice_ThirdDoublesSendsToJail()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.ConsecutiveDoubles = 2;
            _gameState.CurrentPlayer = player;
            _turnControlPanel.Initialize(_gameState, _uiController);
            yield return null;

            // Act
            _turnControlPanel.DisplayDiceRoll(3, 3);
            yield return null;

            // Assert
            Assert.IsTrue(_turnControlPanel.IsShowingJailMessage(), 
                "Should show 'Go to Jail' message for third doubles");
        }

        [UnityTest]
        public IEnumerator RollDice_PassGOCollects200()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Position = 38;
            player.Money = 1500;
            _gameState.CurrentPlayer = player;
            _turnControlPanel.Initialize(_gameState, _uiController);
            yield return null;

            // Act
            // Simulate rolling and passing GO
            player.Position = 2; // Passed GO
            player.Money = 1700;
            _turnControlPanel.DisplayPassGONotification();
            yield return null;

            // Assert
            Assert.IsTrue(_turnControlPanel.IsShowingPassGOMessage(), 
                "Should show 'Passed GO, collect $200' notification");
        }

        [UnityTest]
        public IEnumerator RollDice_SpacebarShortcut()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            _gameState.CurrentPlayer = player;
            _gameState.TurnPhase = TurnPhase.RollDice;
            _turnControlPanel.Initialize(_gameState, _uiController);
            yield return null;

            // Act
            var rollButton = _turnControlPanel.GetRollDiceButton();
            var hasSpaceShortcut = _turnControlPanel.HasKeyboardShortcut(KeyCode.Space, "RollDice");

            // Assert
            Assert.IsTrue(hasSpaceShortcut, 
                "Space bar should be keyboard shortcut for rolling dice");
        }

        #endregion

        #region User Story 2.8: Property Purchase Tests

        [UnityTest]
        public IEnumerator PropertyPurchase_ModalAppearsOnUnownedProperty()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Position = 1; // Mediterranean Avenue
            player.Money = 1500;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.ShowPropertyPurchaseModal(1);
            yield return null;

            // Assert
            Assert.IsTrue(_uiController.IsModalShowing("PropertyPurchase"), 
                "Property card modal should appear");
        }

        [UnityTest]
        public IEnumerator PropertyPurchase_ShowsPropertyDetails()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 1500;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var modal = _uiController.ShowPropertyPurchaseModal(1);
            yield return null;

            // Assert
            Assert.IsNotNull(modal.GetPropertyName(), "Should show property name");
            Assert.IsNotNull(modal.GetColorGroup(), "Should show color group");
            Assert.Greater(modal.GetPrice(), 0, "Should show price");
            Assert.IsNotNull(modal.GetRentStructure(), "Should show rent structure");
        }

        [UnityTest]
        public IEnumerator PropertyPurchase_BuyButtonPurchasesProperty()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 1500;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var modal = _uiController.ShowPropertyPurchaseModal(1);
            var buyButton = modal.GetBuyButton();
            buyButton.onClick.Invoke();
            yield return null;

            var property = _gameState.Board.Spaces[1] as Property;

            // Assert
            Assert.AreEqual(player, property.Owner, "Property should be owned by player");
            Assert.AreEqual(1440, player.Money, "Money should be deducted (1500 - 60)");
        }

        [UnityTest]
        public IEnumerator PropertyPurchase_DeclineButtonSkipsPurchase()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 1500;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var modal = _uiController.ShowPropertyPurchaseModal(1);
            var declineButton = modal.GetDeclineButton();
            declineButton.onClick.Invoke();
            yield return null;

            var property = _gameState.Board.Spaces[1] as Property;

            // Assert
            Assert.IsNull(property.Owner, "Property should remain unowned");
            Assert.AreEqual(1500, player.Money, "Money should not be deducted");
            Assert.IsFalse(_uiController.IsModalShowing("PropertyPurchase"), 
                "Modal should be dismissed");
        }

        [UnityTest]
        public IEnumerator PropertyPurchase_BuyButtonDisabledWithInsufficientFunds()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 50; // Not enough for Mediterranean Avenue ($60)
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var modal = _uiController.ShowPropertyPurchaseModal(1);
            yield return null;

            var buyButton = modal.GetBuyButton();

            // Assert
            Assert.IsFalse(buyButton.interactable, 
                "Buy button should be disabled with insufficient funds");
        }

        [UnityTest]
        public IEnumerator PropertyPurchase_ShowsConfirmationMessage()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 1500;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var modal = _uiController.ShowPropertyPurchaseModal(1);
            var buyButton = modal.GetBuyButton();
            buyButton.onClick.Invoke();
            yield return null;

            // Assert
            Assert.IsTrue(_uiController.IsShowingNotification(), 
                "Should show confirmation message");
            var message = _uiController.GetLastNotificationMessage();
            Assert.IsTrue(message.Contains("purchased"), 
                "Confirmation should mention purchase");
        }

        [UnityTest]
        public IEnumerator PropertyPurchase_KeyboardShortcuts()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 1500;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var modal = _uiController.ShowPropertyPurchaseModal(1);
            yield return null;

            // Assert
            Assert.IsTrue(modal.HasKeyboardShortcut(KeyCode.Return, "Buy"), 
                "Enter key should buy property");
            Assert.IsTrue(modal.HasKeyboardShortcut(KeyCode.Escape, "Decline"), 
                "Escape key should decline purchase");
        }

        #endregion

        #region User Story 2.9: Pay Rent Tests

        [UnityTest]
        public IEnumerator PayRent_NotificationAppearsWhenLandingOnOwnedProperty()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            player1.Money = 1500;
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player2;
            _gameState.CurrentPlayer = player1;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.ShowRentPaymentNotification(player1, player2, property, 10);
            yield return null;

            // Assert
            Assert.IsTrue(_uiController.IsShowingNotification(), 
                "Rent notification should appear");
            var message = _uiController.GetLastNotificationMessage();
            Assert.IsTrue(message.Contains("$10"), "Should show rent amount");
            Assert.IsTrue(message.Contains("Bob"), "Should show owner name");
        }

        [UnityTest]
        public IEnumerator PayRent_CalculatesCorrectlyWithHouses()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            player1.Money = 1500;
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player2;
            property.Houses = 2;
            var rentAmount = 30; // Example rent with 2 houses
            yield return null;

            // Act & Assert
            Assert.AreEqual(30, _uiController.CalculateRent(property, 0), 
                "Rent should be calculated based on houses");
        }

        [UnityTest]
        public IEnumerator PayRent_NoRentOnMortgagedProperty()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player2;
            property.IsMortgaged = true;
            yield return null;

            // Act
            var rentAmount = _uiController.CalculateRent(property, 0);

            // Assert
            Assert.AreEqual(0, rentAmount, 
                "No rent should be charged on mortgaged properties");
        }

        [UnityTest]
        public IEnumerator PayRent_RailroadRentBasedOnOwnership()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            var railroad1 = _gameState.Board.Spaces[5] as Property; // Reading Railroad
            var railroad2 = _gameState.Board.Spaces[15] as Property; // Pennsylvania Railroad
            railroad1.Owner = player2;
            railroad2.Owner = player2;
            yield return null;

            // Act
            var rent = _uiController.CalculateRent(railroad1, 0);

            // Assert
            Assert.AreEqual(50, rent, 
                "Railroad rent should be $50 with 2 railroads owned");
        }

        [UnityTest]
        public IEnumerator PayRent_UtilityRentBasedOnDiceRoll()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            var utility = _gameState.Board.Spaces[12] as Property; // Electric Company
            utility.Owner = player2;
            var diceRoll = 7;
            yield return null;

            // Act
            var rent = _uiController.CalculateRent(utility, diceRoll);

            // Assert
            Assert.AreEqual(28, rent, 
                "Utility rent should be 4x dice roll with 1 utility (4 * 7 = 28)");
        }

        #endregion

        #region User Story 2.10: End Turn Tests

        [UnityTest]
        public IEnumerator EndTurn_ButtonVisibleAfterMandatoryActions()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            _gameState.CurrentPlayer = player;
            _gameState.TurnPhase = TurnPhase.EndTurn;
            _turnControlPanel.Initialize(_gameState, _uiController);
            yield return null;

            // Act
            var endTurnButton = _turnControlPanel.GetEndTurnButton();

            // Assert
            Assert.IsNotNull(endTurnButton, "End Turn button should exist");
            Assert.IsTrue(endTurnButton.interactable, 
                "End Turn button should be enabled after mandatory actions");
        }

        [UnityTest]
        public IEnumerator EndTurn_ButtonDisabledDuringMandatoryActions()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            _gameState.CurrentPlayer = player;
            _gameState.TurnPhase = TurnPhase.PayRent; // Mandatory action
            _turnControlPanel.Initialize(_gameState, _uiController);
            yield return null;

            // Act
            var endTurnButton = _turnControlPanel.GetEndTurnButton();

            // Assert
            Assert.IsFalse(endTurnButton.interactable, 
                "End Turn button should be disabled during mandatory actions");
        }

        [UnityTest]
        public IEnumerator EndTurn_AdvancesToNextPlayer()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            _gameState.CurrentPlayer = player1;
            _gameState.TurnPhase = TurnPhase.EndTurn;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.EndTurn();
            yield return null;

            // Assert
            Assert.AreEqual(player2, _gameState.CurrentPlayer, 
                "Should advance to next player");
        }

        [UnityTest]
        public IEnumerator EndTurn_IncrementsTurnCounter()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            _gameState.CurrentPlayer = player1;
            _gameState.TurnNumber = 5;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.EndTurn();
            yield return null;

            // Assert
            Assert.AreEqual(6, _gameState.TurnNumber, 
                "Turn counter should be incremented");
        }

        [UnityTest]
        public IEnumerator EndTurn_ShowsNextPlayerNotification()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            _gameState.CurrentPlayer = player1;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.EndTurn();
            yield return null;

            // Assert
            Assert.IsTrue(_uiController.IsShowingNotification(), 
                "Should show turn notification");
            var message = _uiController.GetLastNotificationMessage();
            Assert.IsTrue(message.Contains("Bob"), 
                "Should show next player's name");
        }

        [UnityTest]
        public IEnumerator EndTurn_NotAvailableWithDoubles()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.ConsecutiveDoubles = 1;
            _gameState.CurrentPlayer = player;
            _gameState.TurnPhase = TurnPhase.RollDice; // Must roll again
            _turnControlPanel.Initialize(_gameState, _uiController);
            yield return null;

            // Act
            var endTurnButton = _turnControlPanel.GetEndTurnButton();

            // Assert
            Assert.IsFalse(endTurnButton.interactable, 
                "End Turn should not be available when player rolled doubles");
        }

        [UnityTest]
        public IEnumerator EndTurn_EnterKeyShortcut()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            _gameState.CurrentPlayer = player;
            _gameState.TurnPhase = TurnPhase.EndTurn;
            _turnControlPanel.Initialize(_gameState, _uiController);
            yield return null;

            // Act
            var hasEnterShortcut = _turnControlPanel.HasKeyboardShortcut(KeyCode.Return, "EndTurn");

            // Assert
            Assert.IsTrue(hasEnterShortcut, 
                "Enter key should be keyboard shortcut for ending turn");
        }

        #endregion
    }
}
