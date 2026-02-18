using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using MonopolyFrenzy.UI.Controllers;
using MonopolyFrenzy.Core;
using MonopolyFrenzy.Commands;

namespace MonopolyFrenzy.Tests.UI
{
    /// <summary>
    /// Phase 2 Integration Tests - Complete UI Workflows
    /// 
    /// Tests complete user workflows from start to finish, ensuring all UI components
    /// work together seamlessly with Phase 1 game logic.
    /// </summary>
    [TestFixture]
    public class Phase2IntegrationTests
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

        #region Complete Game Workflow Tests

        [UnityTest]
        public IEnumerator CompleteWorkflow_NewGameToFirstTurn()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act - Main Menu
            var mainMenu = _uiController.ShowMainMenu();
            mainMenu.ClickNewGame();
            yield return null;

            // Act - Game Setup
            var setupScreen = _uiController.GetCurrentScreen() as GameSetupScreen;
            setupScreen.AddPlayer("Alice");
            setupScreen.AddPlayer("Bob");
            setupScreen.ClickStartGame();
            yield return null;

            // Assert - Game Board Loaded
            Assert.IsTrue(_uiController.IsGameBoardVisible(), 
                "Game board should be displayed");
            Assert.AreEqual(2, _gameState.Players.Count, 
                "Should have 2 players");
            Assert.IsNotNull(_gameState.CurrentPlayer, 
                "Should have a current player");
        }

        [UnityTest]
        public IEnumerator CompleteWorkflow_FullTurnSequence()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            _gameState.CurrentPlayer = player;
            _gameState.TurnPhase = TurnPhase.RollDice;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act - Roll Dice
            var turnControl = _uiController.GetTurnControlPanel();
            var rollButton = turnControl.GetRollDiceButton();
            rollButton.onClick.Invoke();
            yield return null;

            // Simulate dice roll and movement
            player.Position = 1; // Mediterranean Avenue
            _gameState.TurnPhase = TurnPhase.TakeTurnAction;
            yield return null;

            // Act - Property Purchase
            var propertyModal = _uiController.ShowPropertyPurchaseModal(1);
            var buyButton = propertyModal.GetBuyButton();
            buyButton.onClick.Invoke();
            yield return null;

            // Act - End Turn
            _gameState.TurnPhase = TurnPhase.EndTurn;
            var endTurnButton = turnControl.GetEndTurnButton();
            endTurnButton.onClick.Invoke();
            yield return null;

            // Assert
            var property = _gameState.Board.Spaces[1] as Property;
            Assert.AreEqual(player, property.Owner, 
                "Player should own the property");
            Assert.Less(player.Money, 1500, 
                "Money should be deducted");
        }

        [UnityTest]
        public IEnumerator CompleteWorkflow_MultiplePlayerTurns()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            var player3 = _gameState.AddPlayer("Charlie");
            _gameState.CurrentPlayer = player1;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act - Player 1 Turn
            _uiController.ExecuteTurn(player1.Id);
            yield return null;
            Assert.AreEqual(player2, _gameState.CurrentPlayer, 
                "Should advance to player 2");

            // Act - Player 2 Turn
            _uiController.ExecuteTurn(player2.Id);
            yield return null;
            Assert.AreEqual(player3, _gameState.CurrentPlayer, 
                "Should advance to player 3");

            // Act - Player 3 Turn
            _uiController.ExecuteTurn(player3.Id);
            yield return null;

            // Assert
            Assert.AreEqual(player1, _gameState.CurrentPlayer, 
                "Should wrap back to player 1");
        }

        [UnityTest]
        public IEnumerator CompleteWorkflow_PropertyPurchaseToBuilding()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 2000;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act - Purchase first property of color group
            var property1 = _gameState.Board.Spaces[1] as Property; // Mediterranean
            _uiController.ExecuteCommand(new BuyPropertyCommand(player, property1));
            yield return null;

            // Act - Purchase second property to complete monopoly
            var property2 = _gameState.Board.Spaces[3] as Property; // Baltic
            _uiController.ExecuteCommand(new BuyPropertyCommand(player, property2));
            yield return null;

            // Act - Build houses
            _uiController.BuildHouses(1, 2);
            yield return null;

            // Assert
            Assert.AreEqual(player, property1.Owner, "Should own property 1");
            Assert.AreEqual(player, property2.Owner, "Should own property 2");
            Assert.AreEqual(2, property1.Houses, "Should have 2 houses on property 1");
        }

        [UnityTest]
        public IEnumerator CompleteWorkflow_TradeExecution()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            player1.Money = 1500;
            player2.Money = 1500;
            
            var property1 = _gameState.Board.Spaces[1] as Property;
            var property2 = _gameState.Board.Spaces[3] as Property;
            property1.Owner = player1;
            property2.Owner = player2;
            player1.OwnedProperties.Add(property1);
            player2.OwnedProperties.Add(property2);
            
            _uiController.Initialize(_gameState);
            yield return null;

            // Act - Propose Trade
            var tradeModal = _uiController.OpenTradeDialog();
            tradeModal.SelectPartner(player2.Id);
            tradeModal.AddPropertyToOffer(property1.Id);
            tradeModal.AddPropertyToRequest(property2.Id);
            tradeModal.SetMoneyRequest(100);
            tradeModal.ProposeTrade();
            yield return null;

            // Act - Accept Trade
            var trade = _uiController.GetPendingTrade(player2.Id);
            _uiController.AcceptTrade(trade);
            yield return null;

            // Assert
            Assert.AreEqual(player2, property1.Owner, 
                "Property 1 should transfer to player 2");
            Assert.AreEqual(player1, property2.Owner, 
                "Property 2 should transfer to player 1");
            Assert.AreEqual(1600, player1.Money, 
                "Player 1 should receive $100");
            Assert.AreEqual(1400, player2.Money, 
                "Player 2 should pay $100");
        }

        [UnityTest]
        public IEnumerator CompleteWorkflow_JailSequence()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 1500;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act - Send to Jail
            _uiController.SendPlayerToJail(player.Id);
            yield return null;

            Assert.IsTrue(player.IsInJail, "Player should be in jail");
            Assert.AreEqual(10, player.Position, "Player should be at jail space");

            // Act - Pay to Leave
            var jailModal = _uiController.ShowJailOptionsDialog();
            var payButton = jailModal.GetPayButton();
            payButton.onClick.Invoke();
            yield return null;

            // Assert
            Assert.IsFalse(player.IsInJail, "Player should be released");
            Assert.AreEqual(1450, player.Money, "Should pay $50");
        }

        [UnityTest]
        public IEnumerator CompleteWorkflow_BankruptcyProcess()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            player1.Money = 50;
            player2.Money = 1500;
            
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player2;
            
            _gameState.CurrentPlayer = player1;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act - Land on property and cannot pay rent
            player1.Position = 1;
            var rent = 100; // More than player has
            _uiController.ProcessDebt(player1.Id, rent);
            yield return null;

            // Act - Declare Bankruptcy
            var bankruptcyModal = _uiController.GetBankruptcyModal();
            var bankruptButton = bankruptcyModal.GetDeclareBankruptcyButton();
            bankruptButton.onClick.Invoke();
            yield return null;

            // Assert
            Assert.IsTrue(player1.IsBankrupt, "Player 1 should be bankrupt");
            Assert.AreEqual(1550, player2.Money, 
                "Player 2 should receive player 1's remaining cash");
        }

        [UnityTest]
        public IEnumerator CompleteWorkflow_ChanceCardExecution()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Position = 7; // Chance space
            player.Money = 1500;
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act - Land on Chance
            _uiController.LandOnSpace(player.Id, 7);
            yield return null;

            // Card modal should appear
            Assert.IsTrue(_uiController.IsModalShowing("Card"), 
                "Chance card should be drawn");

            // Simulate card effect (e.g., "Collect $200")
            var cardModal = _uiController.GetCardModal();
            yield return new WaitForSeconds(3.5f); // Auto-dismiss

            // Assert
            Assert.IsFalse(_uiController.IsModalShowing("Card"), 
                "Card should auto-dismiss");
        }

        [UnityTest]
        public IEnumerator CompleteWorkflow_SaveAndLoadGame()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            player1.Money = 1234;
            player1.Position = 15;
            _gameState.CurrentPlayer = player1;
            _gameState.TurnNumber = 20;
            
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player1;
            player1.OwnedProperties.Add(property);
            
            _uiController.Initialize(_gameState);
            yield return null;

            // Act - Save Game
            var savePath = System.IO.Path.Combine(
                Application.persistentDataPath, "TestSaves", "IntegrationTest.json");
            _uiController.SaveGame(savePath);
            yield return null;

            // Act - Load Game
            _uiController.LoadGame(savePath);
            yield return null;

            var loadedState = _uiController.GetCurrentGameState();

            // Assert
            Assert.AreEqual(2, loadedState.Players.Count, "Should load both players");
            Assert.AreEqual(1234, loadedState.Players[0].Money, 
                "Should restore player money");
            Assert.AreEqual(15, loadedState.Players[0].Position, 
                "Should restore player position");
            Assert.AreEqual(20, loadedState.TurnNumber, 
                "Should restore turn number");
            Assert.AreEqual(player1.Id, loadedState.CurrentPlayer.Id, 
                "Should restore current player");
        }

        #endregion

        #region UI Event Integration Tests

        [UnityTest]
        public IEnumerator EventIntegration_PlayerMovesUpdatesUI()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Position = 0;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            player.Position = 7;
            _gameState.PublishEvent(new PlayerMovedEvent(player.Id, 0, 7));
            yield return null;

            var boardController = _uiController.GetBoardController();
            var token = boardController.GetPlayerToken(player.Id);

            // Assert
            Assert.AreEqual(7, token.GetCurrentSpace(), 
                "Token should move when PlayerMovedEvent is published");
        }

        [UnityTest]
        public IEnumerator EventIntegration_MoneyChangeUpdatesHUD()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 1500;
            _uiController.Initialize(_gameState);
            var statusPanel = _uiController.GetPlayerStatusPanel();
            statusPanel.SetCurrentPlayer(player);
            yield return null;

            // Act
            player.Money = 1300;
            _gameState.PublishEvent(new MoneyTransferredEvent(player.Id, null, 200));
            yield return null;

            // Assert
            var moneyDisplay = statusPanel.GetMoneyDisplay();
            Assert.AreEqual("$1300", moneyDisplay.text, 
                "Money display should update when MoneyTransferredEvent is published");
        }

        [UnityTest]
        public IEnumerator EventIntegration_PropertyPurchaseUpdatesBoard()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            var property = _gameState.Board.Spaces[1] as Property;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            property.Owner = player;
            _gameState.PublishEvent(new PropertyPurchasedEvent(player.Id, property.Id));
            yield return null;

            var boardController = _uiController.GetBoardController();
            var spaceView = boardController.GetSpace(1);

            // Assert
            Assert.IsTrue(spaceView.HasOwnerIndicator(), 
                "Property should show ownership when PropertyPurchasedEvent is published");
        }

        [UnityTest]
        public IEnumerator EventIntegration_TurnChangeUpdatesHUD()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            _gameState.CurrentPlayer = player1;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _gameState.CurrentPlayer = player2;
            _gameState.PublishEvent(new TurnStartedEvent(player2.Id));
            yield return null;

            var statusPanel = _uiController.GetPlayerStatusPanel();

            // Assert
            Assert.AreEqual(player2.Id, statusPanel.GetCurrentPlayerId(), 
                "HUD should update to show new current player");
        }

        [UnityTest]
        public IEnumerator EventIntegration_GameOverShowsWinScreen()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            _gameState.CurrentPlayer = player1;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _gameState.IsGameOver = true;
            _gameState.Winner = player1;
            _gameState.PublishEvent(new GameOverEvent(player1.Id));
            yield return null;

            // Assert
            Assert.IsTrue(_uiController.IsShowingGameOverScreen(), 
                "Game over screen should appear");
            var gameOverScreen = _uiController.GetGameOverScreen();
            Assert.IsTrue(gameOverScreen.GetWinnerText().Contains("Alice"), 
                "Should show winner's name");
        }

        #endregion

        #region Performance Tests

        [UnityTest]
        public IEnumerator Performance_Maintains60FPSDuringGameplay()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            _gameState.CurrentPlayer = player;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act - Simulate 30 seconds of gameplay
            float startTime = Time.realtimeSinceStartup;
            int frameCount = 0;
            float minFrameTime = float.MaxValue;
            float maxFrameTime = 0f;

            while (Time.realtimeSinceStartup - startTime < 5f) // 5 seconds for test
            {
                float frameStart = Time.realtimeSinceStartup;
                
                // Simulate some UI activity
                if (frameCount % 60 == 0)
                {
                    _uiController.UpdateDisplay();
                }
                
                yield return null;
                
                float frameTime = Time.realtimeSinceStartup - frameStart;
                minFrameTime = Mathf.Min(minFrameTime, frameTime);
                maxFrameTime = Mathf.Max(maxFrameTime, frameTime);
                frameCount++;
            }

            float avgFrameTime = (Time.realtimeSinceStartup - startTime) / frameCount;

            // Assert
            Assert.Less(avgFrameTime, 0.01667f, 
                "Average frame time should be under 16.67ms for 60 FPS");
            LogAssert.NoUnexpectedReceived(); // No errors or warnings
        }

        [UnityTest]
        public IEnumerator Performance_UIResponseUnder50ms()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var startTime = Time.realtimeSinceStartup;
            var modal = _uiController.ShowPropertyCard(1);
            yield return null;
            var responseTime = (Time.realtimeSinceStartup - startTime) * 1000; // Convert to ms

            // Assert
            Assert.Less(responseTime, 50f, 
                "UI response time should be under 50ms");
        }

        [UnityTest]
        public IEnumerator Performance_SceneLoadUnder3Seconds()
        {
            // Arrange
            var startTime = Time.realtimeSinceStartup;

            // Act
            _uiController.LoadScene("GameBoard");
            yield return new WaitForSeconds(0.1f);

            while (!_uiController.IsSceneLoaded())
            {
                yield return null;
                if (Time.realtimeSinceStartup - startTime > 3f)
                    break;
            }

            var loadTime = Time.realtimeSinceStartup - startTime;

            // Assert
            Assert.Less(loadTime, 3f, "Scene should load in under 3 seconds");
        }

        #endregion

        #region Stress Tests

        [UnityTest]
        public IEnumerator StressTest_RapidUIInteractions()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act - Rapid button clicks
            for (int i = 0; i < 100; i++)
            {
                _uiController.ShowPropertyCard(1);
                yield return null;
                _uiController.CloseAllModals();
                yield return null;
            }

            // Assert
            LogAssert.NoUnexpectedReceived(); // No errors
            Assert.IsFalse(_uiController.IsModalShowing("PropertyCard"), 
                "UI should remain stable after rapid interactions");
        }

        [UnityTest]
        public IEnumerator StressTest_MultipleSimultaneousEvents()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            _uiController.Initialize(_gameState);
            yield return null;

            // Act - Publish multiple events simultaneously
            _gameState.PublishEvent(new PlayerMovedEvent(player.Id, 0, 5));
            _gameState.PublishEvent(new MoneyTransferredEvent(player.Id, null, 200));
            _gameState.PublishEvent(new DiceRolledEvent(player.Id, 3, 2));
            yield return null;

            // Assert
            LogAssert.NoUnexpectedReceived(); // Should handle all events without errors
            Assert.IsTrue(_uiController.IsDisplayConsistent(), 
                "UI should remain consistent after multiple simultaneous events");
        }

        [UnityTest]
        public IEnumerator StressTest_WindowResizingDuringGameplay()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            _uiController.Initialize(_gameState);
            yield return null;

            // Act - Resize window multiple times during gameplay
            Screen.SetResolution(1920, 1080, false);
            yield return null;
            Screen.SetResolution(1280, 720, false);
            yield return null;
            Screen.SetResolution(3840, 2160, false);
            yield return null;
            Screen.SetResolution(1920, 1080, false);
            yield return null;

            // Assert
            Assert.IsTrue(_uiController.IsUIProperlyScaled(), 
                "UI should remain properly scaled after multiple resizes");
            LogAssert.NoUnexpectedReceived();
        }

        #endregion
    }
}
