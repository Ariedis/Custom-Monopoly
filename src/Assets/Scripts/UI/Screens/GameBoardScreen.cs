using UnityEngine;
using UnityEngine.UI;
using MonopolyFrenzy.UI.Controllers;
using MonopolyFrenzy.Core;
using MonopolyFrenzy.Events;
using MonopolyFrenzy.Commands;

namespace MonopolyFrenzy.UI.Screens
{
    /// <summary>
    /// Main game board screen controller.
    /// Manages the game board UI and coordinates game actions.
    /// </summary>
    public class GameBoardScreen : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Transform _boardContainer;
        [SerializeField] private Transform _tokensContainer;
        
        [Header("Panels")]
        [SerializeField] private GameObject _playerStatusPanel;
        [SerializeField] private GameObject _turnControlPanel;
        
        private GameState _gameState;
        private EventBus _eventBus;
        
        private void Awake()
        {
            ValidateReferences();
        }
        
        private void Start()
        {
            InitializeGameBoard();
            SetupButtonListeners();
            SubscribeToEvents();
        }
        
        private void OnDestroy()
        {
            RemoveButtonListeners();
            UnsubscribeFromEvents();
        }
        
        /// <summary>
        /// Validates that all required UI references are assigned.
        /// </summary>
        private void ValidateReferences()
        {
            if (_pauseButton == null)
                Debug.LogError("Pause Button is not assigned in GameBoardScreen!", this);
            
            if (_boardContainer == null)
                Debug.LogError("Board Container is not assigned in GameBoardScreen!", this);
            
            if (_tokensContainer == null)
                Debug.LogError("Tokens Container is not assigned in GameBoardScreen!", this);
            
            if (_playerStatusPanel == null)
                Debug.LogWarning("Player Status Panel is not assigned in GameBoardScreen!", this);
            
            if (_turnControlPanel == null)
                Debug.LogWarning("Turn Control Panel is not assigned in GameBoardScreen!", this);
        }
        
        /// <summary>
        /// Initializes the game board from the game state.
        /// </summary>
        private void InitializeGameBoard()
        {
            _gameState = UIController.Instance.GameState;
            _eventBus = UIController.Instance.EventBus;
            
            if (_gameState == null)
            {
                Debug.LogError("Game state not initialized! Returning to main menu.");
                UIController.Instance.LoadScene("MainMenu");
                return;
            }
            
            Debug.Log($"Game board initialized with {_gameState.Players.Count} players");
            
            // TODO: Create board spaces (Week 6)
            // TODO: Create player tokens (Week 6)
        }
        
        /// <summary>
        /// Sets up button click listeners.
        /// </summary>
        private void SetupButtonListeners()
        {
            if (_pauseButton != null)
                _pauseButton.onClick.AddListener(OnPauseClicked);
        }
        
        /// <summary>
        /// Removes button click listeners to prevent memory leaks.
        /// </summary>
        private void RemoveButtonListeners()
        {
            if (_pauseButton != null)
                _pauseButton.onClick.RemoveListener(OnPauseClicked);
        }
        
        /// <summary>
        /// Subscribes to game events.
        /// </summary>
        private void SubscribeToEvents()
        {
            if (_eventBus != null)
            {
                _eventBus.Subscribe<PlayerMovedEvent>(OnPlayerMoved);
                _eventBus.Subscribe<PropertyPurchasedEvent>(OnPropertyPurchased);
                _eventBus.Subscribe<MoneyTransferredEvent>(OnMoneyTransferred);
                _eventBus.Subscribe<TurnStartedEvent>(OnTurnStarted);
                _eventBus.Subscribe<TurnEndedEvent>(OnTurnEnded);
                _eventBus.Subscribe<DiceRolledEvent>(OnDiceRolled);
                _eventBus.Subscribe<GameOverEvent>(OnGameOver);
            }
        }
        
        /// <summary>
        /// Unsubscribes from game events to prevent memory leaks.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<PlayerMovedEvent>(OnPlayerMoved);
                _eventBus.Unsubscribe<PropertyPurchasedEvent>(OnPropertyPurchased);
                _eventBus.Unsubscribe<MoneyTransferredEvent>(OnMoneyTransferred);
                _eventBus.Unsubscribe<TurnStartedEvent>(OnTurnStarted);
                _eventBus.Unsubscribe<TurnEndedEvent>(OnTurnEnded);
                _eventBus.Unsubscribe<DiceRolledEvent>(OnDiceRolled);
                _eventBus.Unsubscribe<GameOverEvent>(OnGameOver);
            }
        }
        
        /// <summary>
        /// Handles Pause button click.
        /// </summary>
        private void OnPauseClicked()
        {
            Debug.Log("Pause clicked");
            // TODO: Implement pause menu (Week 8)
        }
        
        #region Event Handlers
        
        /// <summary>
        /// Handles player moved event.
        /// </summary>
        /// <param name="evt">The player moved event.</param>
        private void OnPlayerMoved(PlayerMovedEvent evt)
        {
            Debug.Log($"Player {evt.PlayerId} moved to position {evt.NewPosition}");
            // TODO: Update token position (Week 6)
        }
        
        /// <summary>
        /// Handles property purchased event.
        /// </summary>
        /// <param name="evt">The property purchased event.</param>
        private void OnPropertyPurchased(PropertyPurchasedEvent evt)
        {
            Debug.Log($"Property {evt.PropertyName} purchased by player {evt.PlayerId}");
            // TODO: Update board visual (Week 6)
        }
        
        /// <summary>
        /// Handles money transferred event.
        /// </summary>
        /// <param name="evt">The money transferred event.</param>
        private void OnMoneyTransferred(MoneyTransferredEvent evt)
        {
            Debug.Log($"Money transferred: {evt.Amount} from player {evt.FromPlayerId} to {evt.ToPlayerId}");
            // TODO: Update player money display (Week 6)
        }
        
        /// <summary>
        /// Handles turn started event.
        /// </summary>
        /// <param name="evt">The turn started event.</param>
        private void OnTurnStarted(TurnStartedEvent evt)
        {
            Debug.Log($"Turn started for player {evt.PlayerId}");
            // TODO: Update turn indicator (Week 6)
        }
        
        /// <summary>
        /// Handles turn ended event.
        /// </summary>
        /// <param name="evt">The turn ended event.</param>
        private void OnTurnEnded(TurnEndedEvent evt)
        {
            Debug.Log($"Turn ended for player {evt.PlayerId}");
            // TODO: Update UI state (Week 6)
        }
        
        /// <summary>
        /// Handles dice rolled event.
        /// </summary>
        /// <param name="evt">The dice rolled event.</param>
        private void OnDiceRolled(DiceRolledEvent evt)
        {
            Debug.Log($"Dice rolled: {evt.Die1} + {evt.Die2} = {evt.Total}");
            // TODO: Update dice display (Week 6)
        }
        
        /// <summary>
        /// Handles game over event.
        /// </summary>
        /// <param name="evt">The game over event.</param>
        private void OnGameOver(GameOverEvent evt)
        {
            Debug.Log($"Game over! Winner: {evt.WinnerId}");
            // TODO: Show game over screen (Week 8)
        }
        
        #endregion
    }
}
