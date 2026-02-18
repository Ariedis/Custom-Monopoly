using UnityEngine;
using UnityEngine.SceneManagement;
using MonopolyFrenzy.Core;
using MonopolyFrenzy.Events;

namespace MonopolyFrenzy.UI.Controllers
{
    /// <summary>
    /// Singleton controller for managing UI state and scene transitions.
    /// Persists across scenes using DontDestroyOnLoad.
    /// </summary>
    public class UIController : MonoBehaviour
    {
        private static UIController _instance;
        
        private GameState _gameState;
        private EventBus _eventBus;
        
        /// <summary>
        /// Gets the singleton instance of UIController.
        /// </summary>
        public static UIController Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("UIController");
                    _instance = go.AddComponent<UIController>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
        
        /// <summary>
        /// Gets the current game state.
        /// </summary>
        public GameState GameState => _gameState;
        
        /// <summary>
        /// Gets the event bus for game events.
        /// </summary>
        public EventBus EventBus => _eventBus;
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Initialize event bus
            _eventBus = new EventBus();
        }
        
        /// <summary>
        /// Initializes a new game with the specified configuration.
        /// </summary>
        /// <param name="playerNames">Names of players to create.</param>
        /// <param name="playerTokens">Token choices for each player.</param>
        /// <param name="startingMoney">Starting money for each player (currently unused - Phase 1 uses default 1500).</param>
        public void InitializeNewGame(string[] playerNames, string[] playerTokens, int startingMoney = 1500)
        {
            _gameState = new GameState();
            _gameState.Initialize();
            
            // Add players (Phase 1 currently uses default $1500 starting money)
            // TODO: Update Player constructor to accept starting money in Phase 1
            for (int i = 0; i < playerNames.Length; i++)
            {
                Player player = _gameState.AddPlayer(playerNames[i]);
                // Store token choice (not yet used in Phase 1)
                // TODO: Add token property to Player class
            }
            
            // Start the game
            _gameState.StartGame();
        }
        
        /// <summary>
        /// Loads a saved game state.
        /// </summary>
        /// <param name="gameState">The game state to load.</param>
        public void LoadGame(GameState gameState)
        {
            _gameState = gameState;
        }
        
        /// <summary>
        /// Loads a scene asynchronously.
        /// </summary>
        /// <param name="sceneName">Name of the scene to load.</param>
        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }
        
        private System.Collections.IEnumerator LoadSceneAsync(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
        
        /// <summary>
        /// Quits the application.
        /// </summary>
        public void QuitApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
