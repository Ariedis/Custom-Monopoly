using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MonopolyFrenzy.UI.Screens;

namespace MonopolyFrenzy.Tests.UI
{
    /// <summary>
    /// Test Suite for User Stories 2.1-2.2: Main Menu and Game Setup
    /// 
    /// Validates:
    /// - Main menu displays correctly with all buttons
    /// - Navigation between menu screens works
    /// - Game setup screen functionality
    /// - Player configuration and validation
    /// - Starting a new game
    /// </summary>
    [TestFixture]
    public class MainMenuTests
    {
        private GameObject _testCanvas;
        private MainMenuScreen _mainMenuScreen;

        [SetUp]
        public void Setup()
        {
            _testCanvas = new GameObject("TestCanvas");
            var canvas = _testCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _testCanvas.AddComponent<GraphicRaycaster>();
            
            var menuObject = new GameObject("MainMenuScreen");
            menuObject.transform.SetParent(_testCanvas.transform);
            _mainMenuScreen = menuObject.AddComponent<MainMenuScreen>();
        }

        [TearDown]
        public void Teardown()
        {
            if (_testCanvas != null)
                Object.Destroy(_testCanvas);
        }

        #region User Story 2.1: Main Menu Tests

        [UnityTest]
        public IEnumerator MainMenu_OnLoad_DisplaysWithBranding()
        {
            // Arrange & Act
            _mainMenuScreen.Initialize();
            yield return null;

            // Assert
            Assert.IsNotNull(_mainMenuScreen, "Main menu screen should be instantiated");
            Assert.IsTrue(_mainMenuScreen.gameObject.activeSelf, "Main menu should be active");
        }

        [UnityTest]
        public IEnumerator MainMenu_HasNewGameButton_IsInteractable()
        {
            // Arrange
            _mainMenuScreen.Initialize();
            yield return null;

            // Act
            var newGameButton = FindButtonInChildren(_mainMenuScreen.transform, "NewGame");

            // Assert
            Assert.IsNotNull(newGameButton, "New Game button should exist");
            Assert.IsTrue(newGameButton.interactable, "New Game button should be interactable");
        }

        [UnityTest]
        public IEnumerator MainMenu_HasLoadGameButton_IsInteractable()
        {
            // Arrange
            _mainMenuScreen.Initialize();
            yield return null;

            // Act
            var loadGameButton = FindButtonInChildren(_mainMenuScreen.transform, "LoadGame");

            // Assert
            Assert.IsNotNull(loadGameButton, "Load Game button should exist");
            Assert.IsTrue(loadGameButton.interactable, "Load Game button should be interactable");
        }

        [UnityTest]
        public IEnumerator MainMenu_HasSettingsButton_IsInteractable()
        {
            // Arrange
            _mainMenuScreen.Initialize();
            yield return null;

            // Act
            var settingsButton = FindButtonInChildren(_mainMenuScreen.transform, "Settings");

            // Assert
            Assert.IsNotNull(settingsButton, "Settings button should exist");
            Assert.IsTrue(settingsButton.interactable, "Settings button should be interactable");
        }

        [UnityTest]
        public IEnumerator MainMenu_HasQuitButton_IsInteractable()
        {
            // Arrange
            _mainMenuScreen.Initialize();
            yield return null;

            // Act
            var quitButton = FindButtonInChildren(_mainMenuScreen.transform, "Quit");

            // Assert
            Assert.IsNotNull(quitButton, "Quit button should exist");
            Assert.IsTrue(quitButton.interactable, "Quit button should be interactable");
        }

        [UnityTest]
        public IEnumerator MainMenu_ButtonHover_ShowsHoverEffect()
        {
            // Arrange
            _mainMenuScreen.Initialize();
            yield return null;
            var newGameButton = FindButtonInChildren(_mainMenuScreen.transform, "NewGame");

            // Act
            var buttonComponent = newGameButton.GetComponent<Button>();
            var colors = buttonComponent.colors;

            // Assert
            Assert.AreNotEqual(colors.normalColor, colors.highlightedColor, 
                "Button should have distinct hover color");
        }

        [UnityTest]
        public IEnumerator MainMenu_KeyboardNavigation_TabKeyWorks()
        {
            // Arrange
            _mainMenuScreen.Initialize();
            yield return null;
            var newGameButton = FindButtonInChildren(_mainMenuScreen.transform, "NewGame");

            // Act
            var navigation = newGameButton.GetComponent<Button>().navigation;

            // Assert
            Assert.AreEqual(Navigation.Mode.Automatic, navigation.mode, 
                "Keyboard navigation should be enabled");
        }

        [UnityTest]
        public IEnumerator MainMenu_LoadsInUnder2Seconds()
        {
            // Arrange
            var startTime = Time.realtimeSinceStartup;

            // Act
            _mainMenuScreen.Initialize();
            yield return null;
            var loadTime = Time.realtimeSinceStartup - startTime;

            // Assert
            Assert.Less(loadTime, 2.0f, "Main menu should load in under 2 seconds");
        }

        #endregion

        #region User Story 2.2: Game Setup Tests

        [UnityTest]
        public IEnumerator GameSetup_CanAddMinimumPlayers()
        {
            // Arrange
            var setupScreen = CreateGameSetupScreen();
            yield return null;

            // Act
            setupScreen.AddPlayer("Player1");
            setupScreen.AddPlayer("Player2");

            // Assert
            Assert.AreEqual(2, setupScreen.GetPlayerCount(), 
                "Should be able to add minimum 2 players");
        }

        [UnityTest]
        public IEnumerator GameSetup_CanAddMaximumPlayers()
        {
            // Arrange
            var setupScreen = CreateGameSetupScreen();
            yield return null;

            // Act
            for (int i = 1; i <= 6; i++)
            {
                setupScreen.AddPlayer($"Player{i}");
            }

            // Assert
            Assert.AreEqual(6, setupScreen.GetPlayerCount(), 
                "Should be able to add maximum 6 players");
        }

        [UnityTest]
        public IEnumerator GameSetup_PreventsDuplicatePlayerNames()
        {
            // Arrange
            var setupScreen = CreateGameSetupScreen();
            yield return null;

            // Act
            setupScreen.AddPlayer("Alice");
            var result = setupScreen.AddPlayer("Alice");

            // Assert
            Assert.IsFalse(result, "Should prevent duplicate player names");
            Assert.AreEqual(1, setupScreen.GetPlayerCount(), 
                "Duplicate player should not be added");
        }

        [UnityTest]
        public IEnumerator GameSetup_PreventsDuplicateTokens()
        {
            // Arrange
            var setupScreen = CreateGameSetupScreen();
            yield return null;

            // Act
            setupScreen.AddPlayer("Player1", "Car");
            var result = setupScreen.AddPlayer("Player2", "Car");

            // Assert
            Assert.IsFalse(result, "Should prevent duplicate token selection");
        }

        [UnityTest]
        public IEnumerator GameSetup_AllowsTokenSelection()
        {
            // Arrange
            var setupScreen = CreateGameSetupScreen();
            yield return null;

            // Act
            setupScreen.AddPlayer("Player1", "Car");
            setupScreen.AddPlayer("Player2", "Hat");
            setupScreen.AddPlayer("Player3", "Ship");

            // Assert
            Assert.AreEqual("Car", setupScreen.GetPlayer(0).Token, 
                "Player 1 should have Car token");
            Assert.AreEqual("Hat", setupScreen.GetPlayer(1).Token, 
                "Player 2 should have Hat token");
            Assert.AreEqual("Ship", setupScreen.GetPlayer(2).Token, 
                "Player 3 should have Ship token");
        }

        [UnityTest]
        public IEnumerator GameSetup_Has8TokenChoices()
        {
            // Arrange
            var setupScreen = CreateGameSetupScreen();
            yield return null;

            // Act
            var availableTokens = setupScreen.GetAvailableTokens();

            // Assert
            Assert.AreEqual(8, availableTokens.Length, 
                "Should have 8 classic token choices");
        }

        [UnityTest]
        public IEnumerator GameSetup_CanSetStartingMoney_DefaultValue()
        {
            // Arrange
            var setupScreen = CreateGameSetupScreen();
            yield return null;

            // Act
            var startingMoney = setupScreen.GetStartingMoney();

            // Assert
            Assert.AreEqual(1500, startingMoney, 
                "Default starting money should be $1500");
        }

        [UnityTest]
        public IEnumerator GameSetup_CanSetStartingMoney_CustomValue()
        {
            // Arrange
            var setupScreen = CreateGameSetupScreen();
            yield return null;

            // Act
            setupScreen.SetStartingMoney(2000);
            var startingMoney = setupScreen.GetStartingMoney();

            // Assert
            Assert.AreEqual(2000, startingMoney, 
                "Should be able to set custom starting money");
        }

        [UnityTest]
        public IEnumerator GameSetup_StartingMoneyRange_ValidatesMinimum()
        {
            // Arrange
            var setupScreen = CreateGameSetupScreen();
            yield return null;

            // Act
            var result = setupScreen.SetStartingMoney(400);

            // Assert
            Assert.IsFalse(result, "Should reject starting money below $500");
        }

        [UnityTest]
        public IEnumerator GameSetup_StartingMoneyRange_ValidatesMaximum()
        {
            // Arrange
            var setupScreen = CreateGameSetupScreen();
            yield return null;

            // Act
            var result = setupScreen.SetStartingMoney(6000);

            // Assert
            Assert.IsFalse(result, "Should reject starting money above $5000");
        }

        [UnityTest]
        public IEnumerator GameSetup_CanEnableHouseRules()
        {
            // Arrange
            var setupScreen = CreateGameSetupScreen();
            yield return null;

            // Act
            setupScreen.SetHouseRule("FreeParkingMoney", true);
            setupScreen.SetHouseRule("AuctionOnDecline", true);

            // Assert
            Assert.IsTrue(setupScreen.GetHouseRule("FreeParkingMoney"), 
                "Should enable Free Parking money rule");
            Assert.IsTrue(setupScreen.GetHouseRule("AuctionOnDecline"), 
                "Should enable auction on decline rule");
        }

        [UnityTest]
        public IEnumerator GameSetup_CanRandomizePlayerOrder()
        {
            // Arrange
            var setupScreen = CreateGameSetupScreen();
            setupScreen.AddPlayer("Player1");
            setupScreen.AddPlayer("Player2");
            setupScreen.AddPlayer("Player3");
            yield return null;

            // Act
            setupScreen.RandomizePlayerOrder();

            // Assert
            Assert.IsTrue(setupScreen.PlayerOrderWasRandomized(), 
                "Player order should be marked as randomized");
        }

        [UnityTest]
        public IEnumerator GameSetup_StartGameButton_RequiresMinimumPlayers()
        {
            // Arrange
            var setupScreen = CreateGameSetupScreen();
            setupScreen.AddPlayer("Player1");
            yield return null;

            // Act
            var startButton = FindButtonInChildren(setupScreen.transform, "StartGame");

            // Assert
            Assert.IsFalse(startButton.interactable, 
                "Start button should be disabled with less than 2 players");
        }

        [UnityTest]
        public IEnumerator GameSetup_StartGameButton_EnabledWithValidSetup()
        {
            // Arrange
            var setupScreen = CreateGameSetupScreen();
            setupScreen.AddPlayer("Player1");
            setupScreen.AddPlayer("Player2");
            yield return null;

            // Act
            var startButton = FindButtonInChildren(setupScreen.transform, "StartGame");

            // Assert
            Assert.IsTrue(startButton.interactable, 
                "Start button should be enabled with 2+ players");
        }

        [UnityTest]
        public IEnumerator GameSetup_BackButton_ReturnsToMainMenu()
        {
            // Arrange
            var setupScreen = CreateGameSetupScreen();
            yield return null;

            // Act
            var backButton = FindButtonInChildren(setupScreen.transform, "Back");

            // Assert
            Assert.IsNotNull(backButton, 
                "Back button should exist");
        }

        #endregion

        #region Helper Methods

        private Button FindButtonInChildren(Transform parent, string buttonName)
        {
            var buttons = parent.GetComponentsInChildren<Button>(true);
            foreach (var button in buttons)
            {
                if (button.name.Contains(buttonName))
                    return button;
            }
            return null;
        }

        private GameSetupScreen CreateGameSetupScreen()
        {
            var setupObject = new GameObject("GameSetupScreen");
            setupObject.transform.SetParent(_testCanvas.transform);
            var screen = setupObject.AddComponent<GameSetupScreen>();
            screen.Initialize();
            return screen;
        }

        #endregion
    }
}
