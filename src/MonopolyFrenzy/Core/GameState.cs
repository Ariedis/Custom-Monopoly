using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MonopolyFrenzy.Core
{
    /// <summary>
    /// Represents the current phase of the game.
    /// </summary>
    public enum GamePhase
    {
        Setup,
        Playing,
        GameOver
    }
    
    /// <summary>
    /// Represents the complete state of a Monopoly game.
    /// Contains all game data including board, players, and current turn.
    /// This class is pure C# with no Unity dependencies for testability.
    /// </summary>
    public class GameState
    {
        private int _playerIdCounter = 0;
        private int _currentPlayerIndex = 0;
        
        /// <summary>
        /// Gets the game board.
        /// </summary>
        public Board Board { get; private set; }
        
        /// <summary>
        /// Gets the list of players in the game.
        /// </summary>
        public List<Player> Players { get; private set; }
        
        /// <summary>
        /// Gets the current phase of the game.
        /// </summary>
        public GamePhase CurrentPhase { get; set; }
        
        /// <summary>
        /// Gets the current player whose turn it is.
        /// </summary>
        public Player CurrentPlayer { get; set; }
        
        /// <summary>
        /// Gets the current turn number.
        /// </summary>
        public int TurnNumber { get; set; }
        
        /// <summary>
        /// Gets whether the game can be started (2-6 players).
        /// </summary>
        public bool CanStartGame => Players != null && Players.Count >= 2 && Players.Count <= 6;
        
        /// <summary>
        /// Event fired when a player is added to the game.
        /// </summary>
        public event Action<Player> OnPlayerAdded;
        
        /// <summary>
        /// Event fired when the game starts.
        /// </summary>
        public event Action OnGameStarted;
        
        /// <summary>
        /// Event fired when the game phase changes.
        /// </summary>
        public event Action<GamePhase> OnStateChanged;
        
        /// <summary>
        /// Event fired when the game ends.
        /// </summary>
        public event Action OnGameEnded;
        
        /// <summary>
        /// Event fired when turn advances to next player.
        /// </summary>
        public event Action<Player> OnTurnChanged;
        
        /// <summary>
        /// Initializes a new instance of the GameState class.
        /// </summary>
        public GameState()
        {
            Players = new List<Player>();
            CurrentPhase = GamePhase.Setup;
            CurrentPlayer = null;
            TurnNumber = 0;
        }
        
        /// <summary>
        /// Initializes the game state with a new board.
        /// </summary>
        public void Initialize()
        {
            Board = new Board();
            Players = new List<Player>();
            CurrentPhase = GamePhase.Setup;
            CurrentPlayer = null;
            TurnNumber = 0;
            _playerIdCounter = 0;
            _currentPlayerIndex = 0;
        }
        
        /// <summary>
        /// Adds a player to the game.
        /// </summary>
        /// <param name="name">The player's name.</param>
        /// <returns>The created player.</returns>
        /// <exception cref="ArgumentNullException">Thrown when name is null.</exception>
        /// <exception cref="ArgumentException">Thrown when name is empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when maximum players (6) is exceeded.</exception>
        public Player AddPlayer(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Player name cannot be empty", nameof(name));
            
            if (Players.Count >= 6)
                throw new InvalidOperationException("Cannot add more than 6 players");
            
            // Handle duplicate names
            string uniqueName = name;
            int suffix = 2;
            while (Players.Any(p => p.Name == uniqueName))
            {
                uniqueName = $"{name} ({suffix})";
                suffix++;
            }
            
            // Create player with unique ID
            string playerId = $"player_{_playerIdCounter++}";
            var player = new Player(playerId, uniqueName, 1500);
            
            Players.Add(player);
            
            // Fire event
            OnPlayerAdded?.Invoke(player);
            
            return player;
        }
        
        /// <summary>
        /// Starts the game.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when cannot start game (need 2-6 players).</exception>
        public void StartGame()
        {
            if (!CanStartGame)
                throw new InvalidOperationException("Cannot start game. Need 2-6 players.");
            
            CurrentPhase = GamePhase.Playing;
            _currentPlayerIndex = 0;
            CurrentPlayer = Players[_currentPlayerIndex];
            TurnNumber = 0;
            
            // Fire events
            OnStateChanged?.Invoke(CurrentPhase);
            OnGameStarted?.Invoke();
        }
        
        /// <summary>
        /// Ends the game.
        /// </summary>
        public void EndGame()
        {
            CurrentPhase = GamePhase.GameOver;
            
            // Fire events
            OnStateChanged?.Invoke(CurrentPhase);
            OnGameEnded?.Invoke();
        }
        
        /// <summary>
        /// Advances to the next player's turn.
        /// Skips bankrupt players automatically.
        /// </summary>
        public void NextTurn()
        {
            if (CurrentPhase != GamePhase.Playing)
                return;
            
            // Increment turn number
            TurnNumber++;
            
            // Find next non-bankrupt player
            do
            {
                _currentPlayerIndex = (_currentPlayerIndex + 1) % Players.Count;
                CurrentPlayer = Players[_currentPlayerIndex];
                
                // Break if we found a non-bankrupt player
                if (!CurrentPlayer.IsBankrupt)
                    break;
                
                // Safety check: if all players are bankrupt, end game
                if (Players.All(p => p.IsBankrupt))
                {
                    EndGame();
                    return;
                }
                
            } while (CurrentPlayer.IsBankrupt);
            
            // Fire event
            OnTurnChanged?.Invoke(CurrentPlayer);
        }
        
        /// <summary>
        /// Serializes the game state to JSON.
        /// </summary>
        /// <returns>JSON string representation of the game state.</returns>
        public string SerializeToJson()
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            
            return JsonConvert.SerializeObject(this, settings);
        }
        
        /// <summary>
        /// Deserializes a game state from JSON.
        /// </summary>
        /// <param name="json">JSON string representation.</param>
        /// <returns>The deserialized game state.</returns>
        /// <exception cref="Exception">Thrown when JSON is invalid.</exception>
        public static GameState DeserializeFromJson(string json)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                
                return JsonConvert.DeserializeObject<GameState>(json, settings);
            }
            catch (JsonException ex)
            {
                throw new Exception("Failed to deserialize game state from JSON", ex);
            }
        }
        
        /// <summary>
        /// Creates a deep copy of this game state.
        /// Used for AI evaluation without affecting the actual game.
        /// </summary>
        /// <returns>A cloned game state.</returns>
        public GameState Clone()
        {
            // Use JSON serialization for deep cloning
            var json = SerializeToJson();
            return DeserializeFromJson(json);
        }
        
        /// <summary>
        /// Gets the number of active (non-bankrupt) players.
        /// </summary>
        /// <returns>The count of active players.</returns>
        public int GetActivePlayerCount()
        {
            return Players.Count(p => !p.IsBankrupt);
        }
        
        /// <summary>
        /// Gets the winner of the game (last remaining player).
        /// </summary>
        /// <returns>The winning player, or null if game not over.</returns>
        public Player GetWinner()
        {
            if (CurrentPhase != GamePhase.GameOver)
                return null;
            
            var activePlayers = Players.Where(p => !p.IsBankrupt).ToList();
            return activePlayers.Count == 1 ? activePlayers[0] : null;
        }
    }
}
