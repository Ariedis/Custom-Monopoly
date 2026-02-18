using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using MonopolyFrenzy.UI.Components;
using MonopolyFrenzy.UI.Controllers;
using MonopolyFrenzy.Core;

namespace MonopolyFrenzy.Tests.UI
{
    /// <summary>
    /// Test Suite for User Stories 2.3-2.4: Game Board Display and Player Tokens
    /// 
    /// Validates:
    /// - Board displays all 40 spaces correctly
    /// - Properties show ownership, color groups, houses/hotels
    /// - Board scales and maintains legibility
    /// - Player tokens display and update correctly
    /// - Token positioning and movement
    /// </summary>
    [TestFixture]
    public class GameBoardTests
    {
        private GameObject _testCanvas;
        private GameObject _boardObject;
        private BoardController _boardController;
        private GameState _gameState;

        [SetUp]
        public void Setup()
        {
            _testCanvas = new GameObject("TestCanvas");
            var canvas = _testCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _testCanvas.AddComponent<GraphicRaycaster>();

            _boardObject = new GameObject("GameBoard");
            _boardObject.transform.SetParent(_testCanvas.transform);
            _boardController = _boardObject.AddComponent<BoardController>();

            _gameState = new GameState();
            _gameState.Initialize();
        }

        [TearDown]
        public void Teardown()
        {
            if (_testCanvas != null)
                Object.Destroy(_testCanvas);
        }

        #region User Story 2.3: Board Display Tests

        [UnityTest]
        public IEnumerator BoardDisplay_ShowsAll40Spaces()
        {
            // Arrange & Act
            _boardController.Initialize(_gameState);
            yield return null;

            // Assert
            var spaces = _boardController.GetBoardSpaces();
            Assert.AreEqual(40, spaces.Count, "Board should display exactly 40 spaces");
        }

        [UnityTest]
        public IEnumerator BoardDisplay_SpacesAreLabeledCorrectly()
        {
            // Arrange
            _boardController.Initialize(_gameState);
            yield return null;

            // Act
            var space0 = _boardController.GetSpace(0);
            var space1 = _boardController.GetSpace(1);

            // Assert
            Assert.AreEqual("GO", space0.GetLabel(), "Space 0 should be GO");
            Assert.IsNotNull(space1.GetLabel(), "Space 1 should have a label");
            Assert.IsTrue(space1.GetLabel().Length > 0, "Space label should not be empty");
        }

        [UnityTest]
        public IEnumerator BoardDisplay_PropertiesShowColorGroup()
        {
            // Arrange
            _boardController.Initialize(_gameState);
            yield return null;

            // Act
            var mediterraneanAve = _boardController.GetSpace(1);
            var balticAve = _boardController.GetSpace(3);

            // Assert
            Assert.IsNotNull(mediterraneanAve.GetColorGroup(), 
                "Mediterranean Avenue should have a color group");
            Assert.AreEqual(mediterraneanAve.GetColorGroup(), balticAve.GetColorGroup(), 
                "Mediterranean and Baltic should share same color group");
        }

        [UnityTest]
        public IEnumerator BoardDisplay_ShowsOwnershipIndicator()
        {
            // Arrange
            _boardController.Initialize(_gameState);
            var player = _gameState.AddPlayer("Alice");
            yield return null;

            // Act
            var property = _gameState.Board.Spaces[1];
            property.Owner = player;
            _boardController.UpdateSpace(1);
            yield return null;

            var spaceView = _boardController.GetSpace(1);

            // Assert
            Assert.IsTrue(spaceView.HasOwnerIndicator(), 
                "Space should show ownership indicator");
            Assert.AreEqual(player.Id, spaceView.GetOwnerId(), 
                "Ownership indicator should match player");
        }

        [UnityTest]
        public IEnumerator BoardDisplay_ShowsHouseCount()
        {
            // Arrange
            _boardController.Initialize(_gameState);
            var player = _gameState.AddPlayer("Alice");
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player;
            property.Houses = 3;
            yield return null;

            // Act
            _boardController.UpdateSpace(1);
            yield return null;

            var spaceView = _boardController.GetSpace(1);

            // Assert
            Assert.AreEqual(3, spaceView.GetHouseCount(), 
                "Space should display 3 houses");
        }

        [UnityTest]
        public IEnumerator BoardDisplay_ShowsHotelIcon()
        {
            // Arrange
            _boardController.Initialize(_gameState);
            var player = _gameState.AddPlayer("Alice");
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player;
            property.HasHotel = true;
            yield return null;

            // Act
            _boardController.UpdateSpace(1);
            yield return null;

            var spaceView = _boardController.GetSpace(1);

            // Assert
            Assert.IsTrue(spaceView.HasHotel(), 
                "Space should display hotel icon");
        }

        [UnityTest]
        public IEnumerator BoardDisplay_RailroadsAreDistinct()
        {
            // Arrange
            _boardController.Initialize(_gameState);
            yield return null;

            // Act
            var railroad1 = _boardController.GetSpace(5); // Reading Railroad
            var railroad2 = _boardController.GetSpace(15); // Pennsylvania Railroad

            // Assert
            Assert.AreEqual(SpaceType.Railroad, railroad1.GetSpaceType(), 
                "Space 5 should be a railroad");
            Assert.AreEqual(SpaceType.Railroad, railroad2.GetSpaceType(), 
                "Space 15 should be a railroad");
            Assert.IsTrue(railroad1.HasRailroadIcon(), 
                "Railroad should have distinctive icon");
        }

        [UnityTest]
        public IEnumerator BoardDisplay_UtilitiesAreDistinct()
        {
            // Arrange
            _boardController.Initialize(_gameState);
            yield return null;

            // Act
            var electric = _boardController.GetSpace(12); // Electric Company
            var water = _boardController.GetSpace(28); // Water Works

            // Assert
            Assert.AreEqual(SpaceType.Utility, electric.GetSpaceType(), 
                "Space 12 should be a utility");
            Assert.AreEqual(SpaceType.Utility, water.GetSpaceType(), 
                "Space 28 should be a utility");
            Assert.IsTrue(electric.HasUtilityIcon(), 
                "Utility should have distinctive icon");
        }

        [UnityTest]
        public IEnumerator BoardDisplay_CornerSpacesAreProminent()
        {
            // Arrange
            _boardController.Initialize(_gameState);
            yield return null;

            // Act
            var go = _boardController.GetSpace(0);
            var jail = _boardController.GetSpace(10);
            var freeParking = _boardController.GetSpace(20);
            var goToJail = _boardController.GetSpace(30);

            // Assert
            Assert.IsTrue(go.IsCornerSpace(), "GO should be a corner space");
            Assert.IsTrue(jail.IsCornerSpace(), "Jail should be a corner space");
            Assert.IsTrue(freeParking.IsCornerSpace(), "Free Parking should be a corner space");
            Assert.IsTrue(goToJail.IsCornerSpace(), "Go To Jail should be a corner space");
        }

        [UnityTest]
        public IEnumerator BoardDisplay_ChanceSpacesIdentifiable()
        {
            // Arrange
            _boardController.Initialize(_gameState);
            yield return null;

            // Act
            var chance1 = _boardController.GetSpace(7);
            var chance2 = _boardController.GetSpace(22);
            var chance3 = _boardController.GetSpace(36);

            // Assert
            Assert.AreEqual(SpaceType.Chance, chance1.GetSpaceType(), 
                "Space 7 should be Chance");
            Assert.IsTrue(chance1.HasChanceIcon(), 
                "Chance space should have distinctive icon");
        }

        [UnityTest]
        public IEnumerator BoardDisplay_CommunityChestSpacesIdentifiable()
        {
            // Arrange
            _boardController.Initialize(_gameState);
            yield return null;

            // Act
            var cc1 = _boardController.GetSpace(2);
            var cc2 = _boardController.GetSpace(17);
            var cc3 = _boardController.GetSpace(33);

            // Assert
            Assert.AreEqual(SpaceType.CommunityChest, cc1.GetSpaceType(), 
                "Space 2 should be Community Chest");
            Assert.IsTrue(cc1.HasCommunityChestIcon(), 
                "Community Chest space should have distinctive icon");
        }

        [UnityTest]
        public IEnumerator BoardDisplay_TaxSpacesClearlyMarked()
        {
            // Arrange
            _boardController.Initialize(_gameState);
            yield return null;

            // Act
            var incomeTax = _boardController.GetSpace(4);
            var luxuryTax = _boardController.GetSpace(38);

            // Assert
            Assert.AreEqual(SpaceType.Tax, incomeTax.GetSpaceType(), 
                "Space 4 should be Income Tax");
            Assert.IsTrue(incomeTax.IsTaxSpace(), 
                "Tax space should be clearly marked");
        }

        [UnityTest]
        public IEnumerator BoardDisplay_ScalesWithWindowSize()
        {
            // Arrange
            _boardController.Initialize(_gameState);
            yield return null;

            var originalScale = _boardController.GetBoardScale();

            // Act
            _boardController.SetCanvasSize(1280, 720);
            yield return null;
            var smallScale = _boardController.GetBoardScale();

            _boardController.SetCanvasSize(3840, 2160);
            yield return null;
            var largeScale = _boardController.GetBoardScale();

            // Assert
            Assert.Greater(largeScale, smallScale, 
                "Board should scale up for larger resolutions");
        }

        [UnityTest]
        public IEnumerator BoardDisplay_MaintainsAspectRatio()
        {
            // Arrange
            _boardController.Initialize(_gameState);
            yield return null;

            // Act
            var aspectRatio = _boardController.GetAspectRatio();

            // Assert
            Assert.AreEqual(1.0f, aspectRatio, 0.1f, 
                "Board should maintain square aspect ratio");
        }

        [UnityTest]
        public IEnumerator BoardDisplay_MortgagedPropertyGrayedOut()
        {
            // Arrange
            _boardController.Initialize(_gameState);
            var player = _gameState.AddPlayer("Alice");
            var property = _gameState.Board.Spaces[1] as Property;
            property.Owner = player;
            property.IsMortgaged = true;
            yield return null;

            // Act
            _boardController.UpdateSpace(1);
            yield return null;

            var spaceView = _boardController.GetSpace(1);

            // Assert
            Assert.IsTrue(spaceView.IsMortgaged(), 
                "Mortgaged property should be visually indicated");
            Assert.IsTrue(spaceView.HasMortgageLabel(), 
                "Mortgaged property should have MORTGAGED label");
        }

        #endregion

        #region User Story 2.4: Player Token Tests

        [UnityTest]
        public IEnumerator PlayerTokens_DisplayDistinctIcons()
        {
            // Arrange
            _boardController.Initialize(_gameState);
            var player1 = _gameState.AddPlayer("Alice", "Car");
            var player2 = _gameState.AddPlayer("Bob", "Hat");
            yield return null;

            // Act
            _boardController.AddPlayerToken(player1);
            _boardController.AddPlayerToken(player2);
            yield return null;

            var token1 = _boardController.GetPlayerToken(player1.Id);
            var token2 = _boardController.GetPlayerToken(player2.Id);

            // Assert
            Assert.AreNotEqual(token1.GetTokenSprite(), token2.GetTokenSprite(), 
                "Players should have distinct token icons");
        }

        [UnityTest]
        public IEnumerator PlayerTokens_PositionedOnCorrectSpace()
        {
            // Arrange
            _boardController.Initialize(_gameState);
            var player = _gameState.AddPlayer("Alice");
            player.Position = 5;
            yield return null;

            // Act
            _boardController.AddPlayerToken(player);
            yield return null;

            var token = _boardController.GetPlayerToken(player.Id);

            // Assert
            Assert.AreEqual(5, token.GetCurrentSpace(), 
                "Token should be positioned on space 5");
        }

        [UnityTest]
        public IEnumerator PlayerTokens_MultipleOnSameSpace_AllVisible()
        {
            // Arrange
            _boardController.Initialize(_gameState);
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            var player3 = _gameState.AddPlayer("Charlie");
            player1.Position = 10;
            player2.Position = 10;
            player3.Position = 10;
            yield return null;

            // Act
            _boardController.AddPlayerToken(player1);
            _boardController.AddPlayerToken(player2);
            _boardController.AddPlayerToken(player3);
            yield return null;

            var token1 = _boardController.GetPlayerToken(player1.Id);
            var token2 = _boardController.GetPlayerToken(player2.Id);
            var token3 = _boardController.GetPlayerToken(player3.Id);

            // Assert
            Assert.IsTrue(token1.IsVisible(), "Token 1 should be visible");
            Assert.IsTrue(token2.IsVisible(), "Token 2 should be visible");
            Assert.IsTrue(token3.IsVisible(), "Token 3 should be visible");
            Assert.IsTrue(_boardController.AreTokensStackedOrOffset(10), 
                "Tokens on same space should be stacked or offset");
        }

        [UnityTest]
        public IEnumerator PlayerTokens_CurrentPlayerHighlighted()
        {
            // Arrange
            _boardController.Initialize(_gameState);
            var player1 = _gameState.AddPlayer("Alice");
            var player2 = _gameState.AddPlayer("Bob");
            _gameState.CurrentPlayer = player1;
            yield return null;

            // Act
            _boardController.AddPlayerToken(player1);
            _boardController.AddPlayerToken(player2);
            _boardController.UpdateCurrentPlayer(player1.Id);
            yield return null;

            var token1 = _boardController.GetPlayerToken(player1.Id);
            var token2 = _boardController.GetPlayerToken(player2.Id);

            // Assert
            Assert.IsTrue(token1.IsHighlighted(), 
                "Current player's token should be highlighted");
            Assert.IsFalse(token2.IsHighlighted(), 
                "Other players' tokens should not be highlighted");
        }

        [UnityTest]
        public IEnumerator PlayerTokens_UpdatePositionImmediately()
        {
            // Arrange
            _boardController.Initialize(_gameState);
            var player = _gameState.AddPlayer("Alice");
            player.Position = 0;
            _boardController.AddPlayerToken(player);
            yield return null;

            // Act
            player.Position = 7;
            _boardController.MoveToken(player.Id, 7);
            yield return null;

            var token = _boardController.GetPlayerToken(player.Id);

            // Assert
            Assert.AreEqual(7, token.GetCurrentSpace(), 
                "Token position should update immediately");
        }

        [UnityTest]
        public IEnumerator PlayerTokens_MovementPathHighlighted()
        {
            // Arrange
            _boardController.Initialize(_gameState);
            var player = _gameState.AddPlayer("Alice");
            player.Position = 0;
            _boardController.AddPlayerToken(player);
            yield return null;

            // Act
            _boardController.MoveToken(player.Id, 7);
            yield return new WaitForSeconds(0.5f);

            // Assert
            Assert.IsTrue(_boardController.WasPathHighlighted(0, 7), 
                "Movement path should be briefly highlighted");
        }

        [UnityTest]
        public IEnumerator PlayerTokens_InJail_PositionedInJustVisiting()
        {
            // Arrange
            _boardController.Initialize(_gameState);
            var player = _gameState.AddPlayer("Alice");
            player.Position = 10;
            player.IsInJail = true;
            yield return null;

            // Act
            _boardController.AddPlayerToken(player);
            _boardController.UpdateTokenJailStatus(player.Id, true);
            yield return null;

            var token = _boardController.GetPlayerToken(player.Id);

            // Assert
            Assert.IsTrue(token.IsInJailArea(), 
                "Token in jail should be positioned in jail area");
            Assert.AreNotEqual(token.GetVisualPosition(), 
                _boardController.GetSpace(10).GetTokenPosition(), 
                "Jail position should differ from Just Visiting");
        }

        [UnityTest]
        public IEnumerator PlayerTokens_8ClassicTokenChoicesAvailable()
        {
            // Arrange & Act
            var availableTokens = _boardController.GetAvailableTokenChoices();

            // Assert
            Assert.AreEqual(8, availableTokens.Count, 
                "Should have 8 classic token choices");
            Assert.Contains("Car", availableTokens);
            Assert.Contains("Hat", availableTokens);
            Assert.Contains("Ship", availableTokens);
        }

        #endregion
    }
}
