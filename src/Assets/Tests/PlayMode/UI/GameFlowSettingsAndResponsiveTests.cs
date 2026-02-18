using NUnit.Framework;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using MonopolyFrenzy.UI.Screens;
using MonopolyFrenzy.UI.Controllers;
using MonopolyFrenzy.Core;

namespace MonopolyFrenzy.Tests.UI
{
    /// <summary>
    /// Test Suite for User Stories 2.20-2.25: Game Flow, Settings, and Responsive Design
    /// 
    /// Validates:
    /// - Save/load game functionality
    /// - Pause menu
    /// - Settings and preferences
    /// - Window resizing and responsive UI
    /// - Keyboard navigation and accessibility
    /// </summary>
    [TestFixture]
    public class GameFlowSettingsAndResponsiveTests
    {
        private GameObject _testCanvas;
        private UIController _uiController;
        private GameState _gameState;
        private string _testSavePath;

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

            _testSavePath = Path.Combine(Application.persistentDataPath, "TestSaves");
            if (!Directory.Exists(_testSavePath))
                Directory.CreateDirectory(_testSavePath);
        }

        [TearDown]
        public void Teardown()
        {
            if (_testCanvas != null)
                Object.Destroy(_testCanvas);

            if (Directory.Exists(_testSavePath))
                Directory.Delete(_testSavePath, true);
        }

        #region User Story 2.20: Save Game Tests

        [UnityTest]
        public IEnumerator SaveGame_OptionInPauseMenu()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var pauseMenu = _uiController.ShowPauseMenu();
            yield return null;

            // Assert
            Assert.IsTrue(pauseMenu.HasOption("SaveGame"), 
                "Save Game option should be in pause menu");
        }

        [UnityTest]
        public IEnumerator SaveGame_PromptsForName()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.InitiateSaveGame();
            yield return null;

            // Assert
            Assert.IsTrue(_uiController.IsShowingInputDialog(), 
                "Should prompt for save game name");
        }

        [UnityTest]
        public IEnumerator SaveGame_SavesCompleteGameState()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            player1.Money = 1500;
            player2.Money = 1300;
            player1.Position = 5;
            player2.Position = 12;
            _gameState.CurrentPlayer = player1;
            _gameState.TurnNumber = 10;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var savePath = Path.Combine(_testSavePath, "TestGame.json");
            _uiController.SaveGame(savePath);
            yield return null;

            // Assert
            Assert.IsTrue(File.Exists(savePath), "Save file should be created");
            
            var savedContent = File.ReadAllText(savePath);
            Assert.IsTrue(savedContent.Contains("Alice"), "Should save player data");
            Assert.IsTrue(savedContent.Contains("Bob"), "Should save all players");
        }

        [UnityTest]
        public IEnumerator SaveGame_SavesPlayerData()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            player.Money = 1234;
            player.Position = 15;
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player;
            player.OwnedProperties.Add(property);
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var savePath = Path.Combine(_testSavePath, "TestGame.json");
            _uiController.SaveGame(savePath);
            yield return null;

            var loadedState = _uiController.LoadGameState(savePath);

            // Assert
            var loadedPlayer = loadedState.Players[0];
            Assert.AreEqual(1234, loadedPlayer.Money, "Should save player money");
            Assert.AreEqual(15, loadedPlayer.Position, "Should save player position");
            Assert.AreEqual(1, loadedPlayer.OwnedProperties.Count, 
                "Should save owned properties");
        }

        [UnityTest]
        public IEnumerator SaveGame_SavesBoardState()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player;
            property.Houses = 3;
            property.IsMortgaged = false;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var savePath = Path.Combine(_testSavePath, "TestGame.json");
            _uiController.SaveGame(savePath);
            yield return null;

            var loadedState = _uiController.LoadGameState(savePath);

            // Assert
            var loadedProperty = loadedState.Board.Spaces[1] as Property;
            Assert.IsNotNull(loadedProperty.Owner, "Should save property ownership");
            Assert.AreEqual(3, loadedProperty.Houses, "Should save house count");
            Assert.IsFalse(loadedProperty.IsMortgaged, "Should save mortgage status");
        }

        [UnityTest]
        public IEnumerator SaveGame_SavesTurnOrderAndCurrentPlayer()
        {
            // Arrange
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            var player3 = _gameState.AddPlayer("Charlie");
            _gameState.CurrentPlayer = player2;
            _gameState.TurnNumber = 15;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var savePath = Path.Combine(_testSavePath, "TestGame.json");
            _uiController.SaveGame(savePath);
            yield return null;

            var loadedState = _uiController.LoadGameState(savePath);

            // Assert
            Assert.AreEqual(player2.Id, loadedState.CurrentPlayer.Id, 
                "Should save current player");
            Assert.AreEqual(15, loadedState.TurnNumber, "Should save turn number");
        }

        [UnityTest]
        public IEnumerator SaveGame_SavesCardDeckStates()
        {
            // Arrange
            _gameState.ShuffleDecks();
            // Draw a few cards to change deck state
            _gameState.DrawChanceCard();
            _gameState.DrawCommunityChestCard();
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var savePath = Path.Combine(_testSavePath, "TestGame.json");
            _uiController.SaveGame(savePath);
            yield return null;

            var loadedState = _uiController.LoadGameState(savePath);

            // Assert
            Assert.IsNotNull(loadedState.ChanceDeck, "Should save Chance deck state");
            Assert.IsNotNull(loadedState.CommunityChestDeck, 
                "Should save Community Chest deck state");
        }

        [UnityTest]
        public IEnumerator SaveGame_SavesGameSettings()
        {
            // Arrange
            _gameState.HouseRules["FreeParkingMoney"] = true;
            _gameState.HouseRules["AuctionOnDecline"] = false;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var savePath = Path.Combine(_testSavePath, "TestGame.json");
            _uiController.SaveGame(savePath);
            yield return null;

            var loadedState = _uiController.LoadGameState(savePath);

            // Assert
            Assert.IsTrue(loadedState.HouseRules["FreeParkingMoney"], 
                "Should save house rules");
            Assert.IsFalse(loadedState.HouseRules["AuctionOnDecline"], 
                "Should save all rule settings");
        }

        [UnityTest]
        public IEnumerator SaveGame_WritesToDiskSuccessfully()
        {
            // Arrange
            _gameState.AddPlayer("Alice");
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var savePath = Path.Combine(_testSavePath, "TestGame.json");
            _uiController.SaveGame(savePath);
            yield return null;

            // Assert
            Assert.IsTrue(File.Exists(savePath), "Save file should be written to disk");
            var fileInfo = new FileInfo(savePath);
            Assert.Greater(fileInfo.Length, 0, "Save file should not be empty");
        }

        [UnityTest]
        public IEnumerator SaveGame_ShowsConfirmationMessage()
        {
            // Arrange
            _gameState.AddPlayer("Alice");
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var savePath = Path.Combine(_testSavePath, "MyGame.json");
            _uiController.SaveGame(savePath);
            yield return null;

            // Assert
            Assert.IsTrue(_uiController.IsShowingNotification(), 
                "Should show confirmation message");
            var message = _uiController.GetLastNotificationMessage();
            Assert.IsTrue(message.Contains("saved"), "Should mention save success");
        }

        [UnityTest]
        public IEnumerator SaveGame_CanSaveAtAnyPoint()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            _gameState.CurrentPlayer = player;
            _gameState.TurnPhase = TurnPhase.RollDice; // Mid-turn
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var savePath = Path.Combine(_testSavePath, "MidTurnSave.json");
            var result = _uiController.SaveGame(savePath);
            yield return null;

            // Assert
            Assert.IsTrue(result, "Should be able to save at any point, even mid-turn");
        }

        [UnityTest]
        public IEnumerator SaveGame_SupportsMultipleSaveSlots()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            for (int i = 0; i < 10; i++)
            {
                var savePath = Path.Combine(_testSavePath, $"Save{i}.json");
                _uiController.SaveGame(savePath);
            }
            yield return null;

            // Assert
            var saveFiles = Directory.GetFiles(_testSavePath, "*.json");
            Assert.GreaterOrEqual(saveFiles.Length, 10, 
                "Should support at least 10 save slots");
        }

        #endregion

        #region User Story 2.21: Load Game Tests

        [UnityTest]
        public IEnumerator LoadGame_FromMainMenu_ShowsList()
        {
            // Arrange
            // Create some test save files
            for (int i = 0; i < 3; i++)
            {
                var state = new GameState();
                state.Initialize();
                state.AddPlayer($"Player{i}");
                var savePath = Path.Combine(_testSavePath, $"TestSave{i}.json");
                File.WriteAllText(savePath, state.ToJson());
            }
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var loadScreen = _uiController.ShowLoadGameScreen(_testSavePath);
            yield return null;

            // Assert
            var saveList = loadScreen.GetSaveGameList();
            Assert.GreaterOrEqual(saveList.Count, 3, 
                "Should display list of saved games");
        }

        [UnityTest]
        public IEnumerator LoadGame_ShowsSaveName()
        {
            // Arrange
            var savePath = Path.Combine(_testSavePath, "MyGame.json");
            var state = new GameState();
            state.Initialize();
            state.AddPlayer("Alice");
            File.WriteAllText(savePath, state.ToJson());
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var loadScreen = _uiController.ShowLoadGameScreen(_testSavePath);
            yield return null;

            var saveInfo = loadScreen.GetSaveInfo(0);

            // Assert
            Assert.AreEqual("MyGame", saveInfo.Name, "Should show save name");
        }

        [UnityTest]
        public IEnumerator LoadGame_ShowsDateSaved()
        {
            // Arrange
            var savePath = Path.Combine(_testSavePath, "MyGame.json");
            var state = new GameState();
            state.Initialize();
            File.WriteAllText(savePath, state.ToJson());
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var loadScreen = _uiController.ShowLoadGameScreen(_testSavePath);
            yield return null;

            var saveInfo = loadScreen.GetSaveInfo(0);

            // Assert
            Assert.IsNotNull(saveInfo.DateSaved, "Should show date saved");
        }

        [UnityTest]
        public IEnumerator LoadGame_ShowsNumberOfPlayers()
        {
            // Arrange
            var savePath = Path.Combine(_testSavePath, "MyGame.json");
            var state = new GameState();
            state.Initialize();
            state.AddPlayer("Alice");
            state.AddPlayer("Bob");
            state.AddPlayer("Charlie");
            File.WriteAllText(savePath, state.ToJson());
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var loadScreen = _uiController.ShowLoadGameScreen(_testSavePath);
            yield return null;

            var saveInfo = loadScreen.GetSaveInfo(0);

            // Assert
            Assert.AreEqual(3, saveInfo.PlayerCount, 
                "Should show number of players");
        }

        [UnityTest]
        public IEnumerator LoadGame_ShowsTurnCount()
        {
            // Arrange
            var savePath = Path.Combine(_testSavePath, "MyGame.json");
            var state = new GameState();
            state.Initialize();
            state.TurnNumber = 42;
            File.WriteAllText(savePath, state.ToJson());
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var loadScreen = _uiController.ShowLoadGameScreen(_testSavePath);
            yield return null;

            var saveInfo = loadScreen.GetSaveInfo(0);

            // Assert
            Assert.AreEqual(42, saveInfo.TurnCount, "Should show turn count");
        }

        [UnityTest]
        public IEnumerator LoadGame_LoadsCompleteGameState()
        {
            // Arrange
            var originalState = new GameState();
            originalState.Initialize();
            var player1 = originalState.AddPlayer("Alice");
            var player2 = originalState.AddPlayer("Bob");
            player1.Money = 1234;
            player2.Money = 5678;
            player1.Position = 15;
            originalState.CurrentPlayer = player1;
            
            var savePath = Path.Combine(_testSavePath, "CompleteGame.json");
            File.WriteAllText(savePath, originalState.ToJson());
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.LoadGame(savePath);
            yield return null;

            var loadedState = _uiController.GetCurrentGameState();

            // Assert
            Assert.AreEqual(2, loadedState.Players.Count, "Should load all players");
            Assert.AreEqual(1234, loadedState.Players[0].Money, 
                "Should restore player money");
            Assert.AreEqual(15, loadedState.Players[0].Position, 
                "Should restore player position");
        }

        [UnityTest]
        public IEnumerator LoadGame_GameBoardAppearsWithRestoredState()
        {
            // Arrange
            var originalState = new GameState();
            originalState.Initialize();
            originalState.AddPlayer("Alice");
            
            var savePath = Path.Combine(_testSavePath, "TestLoad.json");
            File.WriteAllText(savePath, originalState.ToJson());
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.LoadGame(savePath);
            yield return null;

            // Assert
            Assert.IsTrue(_uiController.IsGameBoardVisible(), 
                "Game board should be displayed");
            var boardController = _uiController.GetBoardController();
            Assert.IsNotNull(boardController, 
                "Board controller should be initialized");
        }

        [UnityTest]
        public IEnumerator LoadGame_CurrentPlayerTurnResumes()
        {
            // Arrange
            var originalState = new GameState();
            originalState.Initialize();
            var player1 = originalState.AddPlayer("Alice");
            var player2 = originalState.AddPlayer("Bob");
            originalState.CurrentPlayer = player2;
            
            var savePath = Path.Combine(_testSavePath, "TurnResume.json");
            File.WriteAllText(savePath, originalState.ToJson());
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.LoadGame(savePath);
            yield return null;

            var loadedState = _uiController.GetCurrentGameState();

            // Assert
            Assert.AreEqual(player2.Id, loadedState.CurrentPlayer.Id, 
                "Bob's turn should resume");
        }

        [UnityTest]
        public IEnumerator LoadGame_DeleteSaveOption()
        {
            // Arrange
            var savePath = Path.Combine(_testSavePath, "ToDelete.json");
            var state = new GameState();
            state.Initialize();
            File.WriteAllText(savePath, state.ToJson());
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var loadScreen = _uiController.ShowLoadGameScreen(_testSavePath);
            yield return null;

            // Assert
            Assert.IsTrue(loadScreen.HasDeleteButton(0), 
                "Should have delete button for each save");
        }

        [UnityTest]
        public IEnumerator LoadGame_DeleteWithConfirmation()
        {
            // Arrange
            var savePath = Path.Combine(_testSavePath, "ToDelete.json");
            var state = new GameState();
            state.Initialize();
            File.WriteAllText(savePath, state.ToJson());
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var loadScreen = _uiController.ShowLoadGameScreen(_testSavePath);
            var deleteButton = loadScreen.GetDeleteButton(0);
            deleteButton.onClick.Invoke();
            yield return null;

            // Assert
            Assert.IsTrue(_uiController.IsShowingConfirmation(), 
                "Should show confirmation before deleting");
        }

        [UnityTest]
        public IEnumerator LoadGame_CorruptedFile_ShowsError()
        {
            // Arrange
            var savePath = Path.Combine(_testSavePath, "Corrupted.json");
            File.WriteAllText(savePath, "This is not valid JSON {{{");
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var result = _uiController.LoadGame(savePath);
            yield return null;

            // Assert
            Assert.IsFalse(result, "Should fail to load corrupted file");
            Assert.IsTrue(_uiController.IsShowingError(), 
                "Should show error message");
            var errorMessage = _uiController.GetLastErrorMessage();
            Assert.IsTrue(errorMessage.Contains("invalid"), 
                "Error should mention file is invalid");
        }

        [UnityTest]
        public IEnumerator LoadGame_FromPauseMenu_AbandonsCurrentGame()
        {
            // Arrange
            _gameState.AddPlayer("Alice");
            _gameState.TurnNumber = 10;
            
            var savePath = Path.Combine(_testSavePath, "NewGame.json");
            var newState = new GameState();
            newState.Initialize();
            newState.AddPlayer("Bob");
            newState.TurnNumber = 5;
            File.WriteAllText(savePath, newState.ToJson());
            
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var pauseMenu = _uiController.ShowPauseMenu();
            pauseMenu.ClickLoadGame();
            yield return null;

            _uiController.LoadGame(savePath);
            yield return null;

            var currentState = _uiController.GetCurrentGameState();

            // Assert
            Assert.AreEqual(5, currentState.TurnNumber, 
                "Should load new game, abandoning current");
            Assert.AreEqual("Bob", currentState.Players[0].Name, 
                "Should have new game's players");
        }

        #endregion

        #region User Story 2.22: Pause Menu Tests

        [UnityTest]
        public IEnumerator PauseMenu_EscapeKeyOpens()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.SimulateKeyPress(KeyCode.Escape);
            yield return null;

            // Assert
            Assert.IsTrue(_uiController.IsModalShowing("Pause"), 
                "Escape key should open pause menu");
        }

        [UnityTest]
        public IEnumerator PauseMenu_PauseButtonOpens()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var pauseButton = _uiController.GetPauseButton();
            pauseButton.onClick.Invoke();
            yield return null;

            // Assert
            Assert.IsTrue(_uiController.IsModalShowing("Pause"), 
                "Pause button should open pause menu");
        }

        [UnityTest]
        public IEnumerator PauseMenu_OverlaysGame()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var pauseMenu = _uiController.ShowPauseMenu();
            yield return null;

            // Assert
            Assert.IsTrue(pauseMenu.HasSemiTransparentBackground(), 
                "Pause menu should have semi-transparent background overlay");
        }

        [UnityTest]
        public IEnumerator PauseMenu_HasResumeOption()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var pauseMenu = _uiController.ShowPauseMenu();
            yield return null;

            // Assert
            Assert.IsTrue(pauseMenu.HasOption("Resume"), 
                "Pause menu should have Resume option");
        }

        [UnityTest]
        public IEnumerator PauseMenu_HasSaveGameOption()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var pauseMenu = _uiController.ShowPauseMenu();
            yield return null;

            // Assert
            Assert.IsTrue(pauseMenu.HasOption("SaveGame"), 
                "Pause menu should have Save Game option");
        }

        [UnityTest]
        public IEnumerator PauseMenu_HasSettingsOption()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var pauseMenu = _uiController.ShowPauseMenu();
            yield return null;

            // Assert
            Assert.IsTrue(pauseMenu.HasOption("Settings"), 
                "Pause menu should have Settings option");
        }

        [UnityTest]
        public IEnumerator PauseMenu_HasMainMenuOption()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var pauseMenu = _uiController.ShowPauseMenu();
            yield return null;

            // Assert
            Assert.IsTrue(pauseMenu.HasOption("MainMenu"), 
                "Pause menu should have Main Menu option");
        }

        [UnityTest]
        public IEnumerator PauseMenu_HasQuitOption()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var pauseMenu = _uiController.ShowPauseMenu();
            yield return null;

            // Assert
            Assert.IsTrue(pauseMenu.HasOption("Quit"), 
                "Pause menu should have Quit option");
        }

        [UnityTest]
        public IEnumerator PauseMenu_ResumeReturnsToGame()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            var pauseMenu = _uiController.ShowPauseMenu();
            yield return null;

            // Act
            var resumeButton = pauseMenu.GetResumeButton();
            resumeButton.onClick.Invoke();
            yield return null;

            // Assert
            Assert.IsFalse(_uiController.IsModalShowing("Pause"), 
                "Pause menu should close");
            Assert.IsTrue(_uiController.IsGameBoardVisible(), 
                "Should return to game");
        }

        [UnityTest]
        public IEnumerator PauseMenu_GameStateFrozen()
        {
            // Arrange
            _gameState.AddPlayer("Alice");
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.ShowPauseMenu();
            yield return null;

            // Assert
            Assert.IsTrue(_gameState.IsPaused, 
                "Game state should be frozen while paused");
        }

        [UnityTest]
        public IEnumerator PauseMenu_KeyboardNavigationWorks()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var pauseMenu = _uiController.ShowPauseMenu();
            yield return null;

            var resumeButton = pauseMenu.GetResumeButton();

            // Assert
            Assert.AreEqual(Navigation.Mode.Automatic, 
                resumeButton.GetComponent<Button>().navigation.mode, 
                "Keyboard navigation should be enabled");
        }

        [UnityTest]
        public IEnumerator PauseMenu_CanPauseAtAnyTime()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            _gameState.CurrentPlayer = player;
            _gameState.TurnPhase = TurnPhase.MovePiece; // Mid-action
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var pauseMenu = _uiController.ShowPauseMenu();
            yield return null;

            // Assert
            Assert.IsNotNull(pauseMenu, 
                "Should be able to pause at any time during game");
        }

        #endregion

        #region User Story 2.23: Game Settings Tests

        [UnityTest]
        public IEnumerator Settings_AccessibleFromMainMenu()
        {
            // Arrange
            var mainMenu = CreateMainMenuScreen();
            yield return null;

            // Act
            var settingsButton = mainMenu.GetSettingsButton();

            // Assert
            Assert.IsNotNull(settingsButton, 
                "Settings should be accessible from main menu");
        }

        [UnityTest]
        public IEnumerator Settings_AccessibleFromPauseMenu()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var pauseMenu = _uiController.ShowPauseMenu();
            yield return null;

            // Assert
            Assert.IsTrue(pauseMenu.HasOption("Settings"), 
                "Settings should be accessible from pause menu");
        }

        [UnityTest]
        public IEnumerator Settings_HasDisplayCategory()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var settingsScreen = _uiController.ShowSettingsScreen();
            yield return null;

            // Assert
            Assert.IsTrue(settingsScreen.HasCategory("Display"), 
                "Should have Display category");
        }

        [UnityTest]
        public IEnumerator Settings_HasAudioCategory()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var settingsScreen = _uiController.ShowSettingsScreen();
            yield return null;

            // Assert
            Assert.IsTrue(settingsScreen.HasCategory("Audio"), 
                "Should have Audio category");
        }

        [UnityTest]
        public IEnumerator Settings_HasGameplayCategory()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var settingsScreen = _uiController.ShowSettingsScreen();
            yield return null;

            // Assert
            Assert.IsTrue(settingsScreen.HasCategory("Gameplay"), 
                "Should have Gameplay category");
        }

        [UnityTest]
        public IEnumerator Settings_DisplaySettings_Resolution()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var settingsScreen = _uiController.ShowSettingsScreen();
            settingsScreen.SelectCategory("Display");
            yield return null;

            // Assert
            Assert.IsTrue(settingsScreen.HasSetting("Resolution"), 
                "Should have Resolution setting");
        }

        [UnityTest]
        public IEnumerator Settings_DisplaySettings_Fullscreen()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var settingsScreen = _uiController.ShowSettingsScreen();
            settingsScreen.SelectCategory("Display");
            yield return null;

            // Assert
            Assert.IsTrue(settingsScreen.HasSetting("Fullscreen"), 
                "Should have Fullscreen toggle");
        }

        [UnityTest]
        public IEnumerator Settings_DisplaySettings_VSync()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var settingsScreen = _uiController.ShowSettingsScreen();
            settingsScreen.SelectCategory("Display");
            yield return null;

            // Assert
            Assert.IsTrue(settingsScreen.HasSetting("VSync"), 
                "Should have VSync toggle");
        }

        [UnityTest]
        public IEnumerator Settings_AudioSettings_VolumeSliders()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var settingsScreen = _uiController.ShowSettingsScreen();
            settingsScreen.SelectCategory("Audio");
            yield return null;

            // Assert
            Assert.IsTrue(settingsScreen.HasSlider("MasterVolume"), 
                "Should have Master Volume slider");
            Assert.IsTrue(settingsScreen.HasSlider("MusicVolume"), 
                "Should have Music Volume slider");
            Assert.IsTrue(settingsScreen.HasSlider("SFXVolume"), 
                "Should have SFX Volume slider");
        }

        [UnityTest]
        public IEnumerator Settings_AudioSettings_VolumeRange0To100()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var settingsScreen = _uiController.ShowSettingsScreen();
            settingsScreen.SelectCategory("Audio");
            yield return null;

            var masterVolumeSlider = settingsScreen.GetSlider("MasterVolume");

            // Assert
            Assert.AreEqual(0, masterVolumeSlider.minValue, 
                "Volume slider minimum should be 0");
            Assert.AreEqual(100, masterVolumeSlider.maxValue, 
                "Volume slider maximum should be 100");
        }

        [UnityTest]
        public IEnumerator Settings_GameplaySettings_AnimationSpeed()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var settingsScreen = _uiController.ShowSettingsScreen();
            settingsScreen.SelectCategory("Gameplay");
            yield return null;

            // Assert
            Assert.IsTrue(settingsScreen.HasSetting("AnimationSpeed"), 
                "Should have Animation Speed setting (for Phase 5)");
        }

        [UnityTest]
        public IEnumerator Settings_GameplaySettings_ConfirmActions()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            var settingsScreen = _uiController.ShowSettingsScreen();
            settingsScreen.SelectCategory("Gameplay");
            yield return null;

            // Assert
            Assert.IsTrue(settingsScreen.HasToggle("ConfirmActions"), 
                "Should have Confirm Actions toggle");
        }

        [UnityTest]
        public IEnumerator Settings_PersistBetweenSessions()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            var settingsScreen = _uiController.ShowSettingsScreen();
            yield return null;

            // Act
            settingsScreen.SetSetting("MasterVolume", 75);
            settingsScreen.Apply();
            yield return null;

            // Simulate restart
            var newSettingsScreen = _uiController.ShowSettingsScreen();
            yield return null;

            // Assert
            Assert.AreEqual(75, newSettingsScreen.GetSetting("MasterVolume"), 
                "Settings should persist between sessions");
        }

        [UnityTest]
        public IEnumerator Settings_DefaultsButtonResetsAll()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            var settingsScreen = _uiController.ShowSettingsScreen();
            settingsScreen.SetSetting("MasterVolume", 50);
            settingsScreen.Apply();
            yield return null;

            // Act
            settingsScreen.ClickDefaults();
            yield return null;

            // Assert
            Assert.AreEqual(100, settingsScreen.GetSetting("MasterVolume"), 
                "Defaults button should reset to default values");
        }

        [UnityTest]
        public IEnumerator Settings_ApplyButtonSavesChanges()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            var settingsScreen = _uiController.ShowSettingsScreen();
            yield return null;

            // Act
            settingsScreen.SetSetting("MasterVolume", 80);
            var applyButton = settingsScreen.GetApplyButton();
            applyButton.onClick.Invoke();
            yield return null;

            // Assert
            Assert.AreEqual(80, PlayerPrefs.GetInt("MasterVolume", 100), 
                "Apply should save changes");
        }

        [UnityTest]
        public IEnumerator Settings_CancelButtonDiscardsChanges()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            var settingsScreen = _uiController.ShowSettingsScreen();
            var originalVolume = settingsScreen.GetSetting("MasterVolume");
            yield return null;

            // Act
            settingsScreen.SetSetting("MasterVolume", 50);
            var cancelButton = settingsScreen.GetCancelButton();
            cancelButton.onClick.Invoke();
            yield return null;

            // Reopen settings
            settingsScreen = _uiController.ShowSettingsScreen();
            yield return null;

            // Assert
            Assert.AreEqual(originalVolume, settingsScreen.GetSetting("MasterVolume"), 
                "Cancel should discard changes");
        }

        [UnityTest]
        public IEnumerator Settings_ChangesTakeEffectImmediately()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            var settingsScreen = _uiController.ShowSettingsScreen();
            yield return null;

            // Act
            settingsScreen.SetSetting("MasterVolume", 60);
            yield return null;

            // Assert
            Assert.AreEqual(0.6f, AudioListener.volume, 0.01f, 
                "Volume changes should take effect immediately");
        }

        #endregion

        #region User Story 2.24: Window Resizing Tests

        [UnityTest]
        public IEnumerator WindowResize_Supports1280x720()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            Screen.SetResolution(1280, 720, false);
            yield return null;

            // Assert
            Assert.IsTrue(_uiController.IsUIProperlyScaled(), 
                "UI should scale properly at 1280x720");
        }

        [UnityTest]
        public IEnumerator WindowResize_Supports1920x1080()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            Screen.SetResolution(1920, 1080, false);
            yield return null;

            // Assert
            Assert.IsTrue(_uiController.IsUIProperlyScaled(), 
                "UI should scale properly at 1920x1080");
        }

        [UnityTest]
        public IEnumerator WindowResize_Supports4K()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            Screen.SetResolution(3840, 2160, false);
            yield return null;

            // Assert
            Assert.IsTrue(_uiController.IsUIProperlyScaled(), 
                "UI should scale properly at 4K (3840x2160)");
        }

        [UnityTest]
        public IEnumerator WindowResize_TextRemainsLegible()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            Screen.SetResolution(1280, 720, false);
            yield return null;

            var statusPanel = _uiController.GetPlayerStatusPanel();
            var moneyDisplay = statusPanel.GetMoneyDisplay();

            // Assert
            Assert.GreaterOrEqual(moneyDisplay.fontSize, 16, 
                "Text should remain legible (minimum 16pt)");
        }

        [UnityTest]
        public IEnumerator WindowResize_BoardScalesToFit()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            Screen.SetResolution(1280, 720, false);
            yield return null;

            var boardController = _uiController.GetBoardController();

            // Assert
            Assert.IsTrue(boardController.FitsInViewport(), 
                "Board should scale to fit window while maintaining aspect ratio");
        }

        [UnityTest]
        public IEnumerator WindowResize_NoUIElementsCutOff()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            Screen.SetResolution(1280, 720, false);
            yield return null;

            // Assert
            var allUIElements = _uiController.GetAllUIElements();
            foreach (var element in allUIElements)
            {
                Assert.IsTrue(element.IsFullyVisible(), 
                    $"UI element {element.name} should not be cut off");
            }
        }

        [UnityTest]
        public IEnumerator WindowResize_16By9AspectRatioPrimary()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            Screen.SetResolution(1920, 1080, false);
            yield return null;

            var canvasScaler = _uiController.GetCanvasScaler();

            // Assert
            Assert.AreEqual(1920, canvasScaler.referenceResolution.x, 
                "Reference resolution width should be 1920");
            Assert.AreEqual(1080, canvasScaler.referenceResolution.y, 
                "Reference resolution height should be 1080");
        }

        [UnityTest]
        public IEnumerator WindowResize_FullscreenAndWindowedWork()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act - Test windowed mode
            Screen.SetResolution(1920, 1080, false);
            yield return null;
            bool windowedWorks = _uiController.IsUIProperlyScaled();

            // Act - Test fullscreen mode
            Screen.SetResolution(1920, 1080, true);
            yield return null;
            bool fullscreenWorks = _uiController.IsUIProperlyScaled();

            // Assert
            Assert.IsTrue(windowedWorks, "Windowed mode should work correctly");
            Assert.IsTrue(fullscreenWorks, "Fullscreen mode should work correctly");
        }

        #endregion

        #region User Story 2.25: Keyboard Navigation Tests

        [UnityTest]
        public IEnumerator KeyboardNav_TabCyclesThroughElements()
        {
            // Arrange
            var mainMenu = CreateMainMenuScreen();
            yield return null;

            // Act
            mainMenu.SimulateKeyPress(KeyCode.Tab);
            yield return null;

            // Assert
            Assert.IsTrue(mainMenu.HasFocusedElement(), 
                "Tab should cycle to next element");
        }

        [UnityTest]
        public IEnumerator KeyboardNav_ShiftTabCyclesBackwards()
        {
            // Arrange
            var mainMenu = CreateMainMenuScreen();
            mainMenu.FocusLastElement();
            yield return null;

            // Act
            mainMenu.SimulateKeyPress(KeyCode.Tab, shift: true);
            yield return null;

            // Assert
            Assert.IsTrue(mainMenu.HasFocusMovedBackward(), 
                "Shift+Tab should cycle backwards");
        }

        [UnityTest]
        public IEnumerator KeyboardNav_EnterActivatesButton()
        {
            // Arrange
            var mainMenu = CreateMainMenuScreen();
            var newGameButton = mainMenu.GetNewGameButton();
            mainMenu.FocusElement(newGameButton);
            yield return null;

            // Act
            bool buttonClicked = false;
            newGameButton.onClick.AddListener(() => buttonClicked = true);
            mainMenu.SimulateKeyPress(KeyCode.Return);
            yield return null;

            // Assert
            Assert.IsTrue(buttonClicked, 
                "Enter key should activate focused button");
        }

        [UnityTest]
        public IEnumerator KeyboardNav_EscapeClosesModals()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            _uiController.ShowPropertyCard(1);
            yield return null;

            // Act
            _uiController.SimulateKeyPress(KeyCode.Escape);
            yield return null;

            // Assert
            Assert.IsFalse(_uiController.IsModalShowing("PropertyCard"), 
                "Escape key should close modals");
        }

        [UnityTest]
        public IEnumerator KeyboardNav_EscapeOpensPauseMenu()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            _uiController.SimulateKeyPress(KeyCode.Escape);
            yield return null;

            // Assert
            Assert.IsTrue(_uiController.IsModalShowing("Pause"), 
                "Escape key should open pause menu when no modal is open");
        }

        [UnityTest]
        public IEnumerator KeyboardNav_ArrowKeysNavigateLists()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            var loadScreen = _uiController.ShowLoadGameScreen(_testSavePath);
            loadScreen.FocusFirstSaveGame();
            yield return null;

            // Act
            loadScreen.SimulateKeyPress(KeyCode.DownArrow);
            yield return null;

            // Assert
            Assert.IsTrue(loadScreen.IsSecondItemFocused(), 
                "Down arrow should navigate to next item in list");
        }

        [UnityTest]
        public IEnumerator KeyboardNav_SpacebarRollsDice()
        {
            // Arrange
            var player = _gameState.AddPlayer("Alice");
            _gameState.CurrentPlayer = player;
            _gameState.TurnPhase = TurnPhase.RollDice;
            _uiController.Initialize(_gameState);
            yield return null;

            // Act
            bool diceRolled = false;
            _uiController.OnDiceRolled += () => diceRolled = true;
            _uiController.SimulateKeyPress(KeyCode.Space);
            yield return null;

            // Assert
            Assert.IsTrue(diceRolled, 
                "Space bar should roll dice during turn");
        }

        [UnityTest]
        public IEnumerator KeyboardNav_ShortcutsShownInTooltips()
        {
            // Arrange
            _uiController.Initialize(_gameState);
            var turnControlPanel = _uiController.GetTurnControlPanel();
            yield return null;

            // Act
            var rollButton = turnControlPanel.GetRollDiceButton();
            var tooltip = rollButton.GetComponent<TooltipComponent>();

            // Assert
            Assert.IsNotNull(tooltip, "Buttons should have tooltips");
            Assert.IsTrue(tooltip.GetText().Contains("Space"), 
                "Tooltip should show keyboard shortcut");
        }

        [UnityTest]
        public IEnumerator KeyboardNav_FocusVisuallyIndicated()
        {
            // Arrange
            var mainMenu = CreateMainMenuScreen();
            var newGameButton = mainMenu.GetNewGameButton();
            yield return null;

            // Act
            mainMenu.FocusElement(newGameButton);
            yield return null;

            // Assert
            Assert.IsTrue(newGameButton.HasFocusIndicator(), 
                "Focused element should have visual indicator (highlight or outline)");
        }

        [UnityTest]
        public IEnumerator KeyboardNav_WorksInAllScreens()
        {
            // Arrange & Act & Assert
            
            // Test Main Menu
            var mainMenu = CreateMainMenuScreen();
            yield return null;
            Assert.IsTrue(mainMenu.SupportsKeyboardNavigation(), 
                "Main menu should support keyboard navigation");
            
            // Test Game Setup
            var setupScreen = CreateGameSetupScreen();
            yield return null;
            Assert.IsTrue(setupScreen.SupportsKeyboardNavigation(), 
                "Game setup should support keyboard navigation");
            
            // Test Game Board
            _uiController.Initialize(_gameState);
            yield return null;
            Assert.IsTrue(_uiController.SupportsKeyboardNavigation(), 
                "Game board should support keyboard navigation");
            
            // Test Settings
            var settingsScreen = _uiController.ShowSettingsScreen();
            yield return null;
            Assert.IsTrue(settingsScreen.SupportsKeyboardNavigation(), 
                "Settings should support keyboard navigation");
        }

        #endregion

        #region Helper Methods

        private MainMenuScreen CreateMainMenuScreen()
        {
            var menuObject = new GameObject("MainMenuScreen");
            menuObject.transform.SetParent(_testCanvas.transform);
            var screen = menuObject.AddComponent<MainMenuScreen>();
            screen.Initialize();
            return screen;
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
