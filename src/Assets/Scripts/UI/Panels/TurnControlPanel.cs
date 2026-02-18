using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MonopolyFrenzy.Core;
using MonopolyFrenzy.Events;
using MonopolyFrenzy.Commands;

namespace MonopolyFrenzy.UI.Panels
{
    /// <summary>
    /// Panel for controlling turn actions (roll dice, buy property, end turn).
    /// </summary>
    public class TurnControlPanel : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _turnTitleText;
        [SerializeField] private TextMeshProUGUI _diceResultText;
        [SerializeField] private Button _rollDiceButton;
        [SerializeField] private Button _buyPropertyButton;
        [SerializeField] private Button _tradeButton;
        [SerializeField] private Button _buildButton;
        [SerializeField] private Button _endTurnButton;
        [SerializeField] private ScrollRect _messageLogScrollRect;
        [SerializeField] private TextMeshProUGUI _messageLogText;
        
        private GameState _gameState;
        private EventBus _eventBus;
        private System.Text.StringBuilder _messageLog = new System.Text.StringBuilder();
        
        /// <summary>
        /// Initializes the turn control panel.
        /// </summary>
        /// <param name="gameState">The game state.</param>
        /// <param name="eventBus">The event bus.</param>
        public void Initialize(GameState gameState, EventBus eventBus)
        {
            _gameState = gameState;
            _eventBus = eventBus;
            
            SetupButtonListeners();
            SubscribeToEvents();
            UpdateDisplay();
        }
        
        private void OnDestroy()
        {
            RemoveButtonListeners();
            UnsubscribeFromEvents();
        }
        
        /// <summary>
        /// Sets up button click listeners.
        /// </summary>
        private void SetupButtonListeners()
        {
            if (_rollDiceButton != null)
                _rollDiceButton.onClick.AddListener(OnRollDiceClicked);
            
            if (_buyPropertyButton != null)
                _buyPropertyButton.onClick.AddListener(OnBuyPropertyClicked);
            
            if (_tradeButton != null)
                _tradeButton.onClick.AddListener(OnTradeClicked);
            
            if (_buildButton != null)
                _buildButton.onClick.AddListener(OnBuildClicked);
            
            if (_endTurnButton != null)
                _endTurnButton.onClick.AddListener(OnEndTurnClicked);
        }
        
        /// <summary>
        /// Removes button click listeners.
        /// </summary>
        private void RemoveButtonListeners()
        {
            if (_rollDiceButton != null)
                _rollDiceButton.onClick.RemoveListener(OnRollDiceClicked);
            
            if (_buyPropertyButton != null)
                _buyPropertyButton.onClick.RemoveListener(OnBuyPropertyClicked);
            
            if (_tradeButton != null)
                _tradeButton.onClick.RemoveListener(OnTradeClicked);
            
            if (_buildButton != null)
                _buildButton.onClick.RemoveListener(OnBuildClicked);
            
            if (_endTurnButton != null)
                _endTurnButton.onClick.RemoveListener(OnEndTurnClicked);
        }
        
        /// <summary>
        /// Subscribes to game events.
        /// </summary>
        private void SubscribeToEvents()
        {
            if (_eventBus != null)
            {
                _eventBus.Subscribe<TurnStartedEvent>(OnTurnStarted);
                _eventBus.Subscribe<TurnEndedEvent>(OnTurnEnded);
                _eventBus.Subscribe<DiceRolledEvent>(OnDiceRolled);
                _eventBus.Subscribe<PlayerMovedEvent>(OnPlayerMoved);
                _eventBus.Subscribe<PropertyPurchasedEvent>(OnPropertyPurchased);
            }
        }
        
        /// <summary>
        /// Unsubscribes from game events.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<TurnStartedEvent>(OnTurnStarted);
                _eventBus.Unsubscribe<TurnEndedEvent>(OnTurnEnded);
                _eventBus.Unsubscribe<DiceRolledEvent>(OnDiceRolled);
                _eventBus.Unsubscribe<PlayerMovedEvent>(OnPlayerMoved);
                _eventBus.Unsubscribe<PropertyPurchasedEvent>(OnPropertyPurchased);
            }
        }
        
        /// <summary>
        /// Updates the display with current game state.
        /// </summary>
        private void UpdateDisplay()
        {
            if (_gameState == null || _gameState.CurrentPlayer == null)
                return;
            
            // Update turn title
            if (_turnTitleText != null)
                _turnTitleText.text = $"{_gameState.CurrentPlayer.Name}'s Turn";
            
            // Update button states based on turn phase
            UpdateButtonStates();
        }
        
        /// <summary>
        /// Updates button enabled/disabled states.
        /// </summary>
        private void UpdateButtonStates()
        {
            // TODO: Update based on turn state machine state
            // For now, enable roll dice at start of turn
            
            if (_rollDiceButton != null)
                _rollDiceButton.interactable = true; // TODO: Check turn state
            
            if (_buyPropertyButton != null)
                _buyPropertyButton.interactable = false; // Enabled after landing on unowned property
            
            if (_tradeButton != null)
                _tradeButton.interactable = true; // Can trade anytime during turn
            
            if (_buildButton != null)
                _buildButton.interactable = true; // Can build anytime during turn
            
            if (_endTurnButton != null)
                _endTurnButton.interactable = false; // Enabled after completing actions
        }
        
        /// <summary>
        /// Adds a message to the message log.
        /// </summary>
        /// <param name="message">The message to add.</param>
        private void AddMessage(string message)
        {
            _messageLog.AppendLine($"• {message}");
            
            // Keep only last 20 messages
            string[] lines = _messageLog.ToString().Split('\n');
            if (lines.Length > 20)
            {
                _messageLog.Clear();
                for (int i = lines.Length - 20; i < lines.Length; i++)
                {
                    _messageLog.AppendLine(lines[i]);
                }
            }
            
            if (_messageLogText != null)
            {
                _messageLogText.text = _messageLog.ToString();
            }
            
            // Auto-scroll to bottom
            if (_messageLogScrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                _messageLogScrollRect.verticalNormalizedPosition = 0f;
            }
        }
        
        #region Button Handlers
        
        /// <summary>
        /// Handles Roll Dice button click.
        /// </summary>
        private void OnRollDiceClicked()
        {
            if (_gameState == null || _gameState.CurrentPlayer == null)
                return;
            
            var command = new RollDiceCommand(_gameState.CurrentPlayer);
            var result = command.Execute(_gameState);
            
            if (result.Success && _eventBus != null)
            {
                // Command will publish DiceRolledEvent
                _eventBus.Publish(new DiceRolledEvent
                {
                    PlayerId = _gameState.CurrentPlayer.Id,
                    Die1 = command.Die1,
                    Die2 = command.Die2,
                    Total = command.Total,
                    IsDoubles = command.IsDoubles
                });
                
                // Auto-move player
                var moveCommand = new MoveCommand(_gameState.CurrentPlayer, command.Total);
                moveCommand.Execute(_gameState);
            }
        }
        
        /// <summary>
        /// Handles Buy Property button click.
        /// </summary>
        private void OnBuyPropertyClicked()
        {
            Debug.Log("Buy Property clicked");
            // TODO: Execute BuyPropertyCommand (Week 6)
        }
        
        /// <summary>
        /// Handles Trade button click.
        /// </summary>
        private void OnTradeClicked()
        {
            Debug.Log("Trade clicked");
            // TODO: Open trade modal (Week 7)
        }
        
        /// <summary>
        /// Handles Build button click.
        /// </summary>
        private void OnBuildClicked()
        {
            Debug.Log("Build clicked");
            // TODO: Open build modal (Week 7)
        }
        
        /// <summary>
        /// Handles End Turn button click.
        /// </summary>
        private void OnEndTurnClicked()
        {
            if (_gameState == null)
                return;
            
            var command = new EndTurnCommand();
            command.Execute(_gameState);
            
            UpdateDisplay();
        }
        
        #endregion
        
        #region Event Handlers
        
        /// <summary>
        /// Handles turn started event.
        /// </summary>
        private void OnTurnStarted(TurnStartedEvent evt)
        {
            UpdateDisplay();
            AddMessage($"{_gameState.CurrentPlayer.Name}'s turn started");
        }
        
        /// <summary>
        /// Handles turn ended event.
        /// </summary>
        private void OnTurnEnded(TurnEndedEvent evt)
        {
            UpdateDisplay();
        }
        
        /// <summary>
        /// Handles dice rolled event.
        /// </summary>
        private void OnDiceRolled(DiceRolledEvent evt)
        {
            if (_diceResultText != null)
            {
                string doublesText = evt.IsDoubles ? " (DOUBLES!)" : "";
                _diceResultText.text = $"🎲 {evt.Die1} + {evt.Die2} = {evt.Total}{doublesText}";
            }
            
            string playerName = _gameState?.Players.Find(p => p.Id == evt.PlayerId)?.Name ?? "Player";
            AddMessage($"{playerName} rolled {evt.Total}{(evt.IsDoubles ? " (doubles)" : "")}");
        }
        
        /// <summary>
        /// Handles player moved event.
        /// </summary>
        private void OnPlayerMoved(PlayerMovedEvent evt)
        {
            string playerName = _gameState?.Players.Find(p => p.Id == evt.PlayerId)?.Name ?? "Player";
            Space space = _gameState?.Board.GetSpace(evt.NewPosition);
            string spaceName = space?.Name ?? "Unknown";
            
            AddMessage($"{playerName} moved to {spaceName}");
            
            if (evt.PassedGo)
            {
                AddMessage($"{playerName} passed GO and collected $200!");
            }
        }
        
        /// <summary>
        /// Handles property purchased event.
        /// </summary>
        private void OnPropertyPurchased(PropertyPurchasedEvent evt)
        {
            string playerName = _gameState?.Players.Find(p => p.Id == evt.PlayerId)?.Name ?? "Player";
            AddMessage($"{playerName} purchased {evt.PropertyName} for ${evt.Price}");
        }
        
        #endregion
    }
}
