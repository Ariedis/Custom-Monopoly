using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MonopolyFrenzy.Core;
using MonopolyFrenzy.Events;

namespace MonopolyFrenzy.UI.Panels
{
    /// <summary>
    /// Panel displaying current player's status (money, properties, etc.).
    /// </summary>
    public class PlayerStatusPanel : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _playerNameText;
        [SerializeField] private TextMeshProUGUI _moneyText;
        [SerializeField] private TextMeshProUGUI _netWorthText;
        [SerializeField] private TextMeshProUGUI _propertyCountText;
        [SerializeField] private ScrollRect _propertiesScrollRect;
        [SerializeField] private Transform _propertiesContainer;
        [SerializeField] private GameObject _propertyItemPrefab;
        [SerializeField] private GameObject _jailIndicator;
        [SerializeField] private TextMeshProUGUI _jailTurnsText;
        
        private GameState _gameState;
        private EventBus _eventBus;
        private Player _currentPlayer;
        
        /// <summary>
        /// Initializes the player status panel.
        /// </summary>
        /// <param name="gameState">The game state.</param>
        /// <param name="eventBus">The event bus.</param>
        public void Initialize(GameState gameState, EventBus eventBus)
        {
            _gameState = gameState;
            _eventBus = eventBus;
            
            SubscribeToEvents();
            UpdateDisplay();
        }
        
        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
        
        /// <summary>
        /// Subscribes to game events.
        /// </summary>
        private void SubscribeToEvents()
        {
            if (_eventBus != null)
            {
                _eventBus.Subscribe<TurnStartedEvent>(OnTurnStarted);
                _eventBus.Subscribe<MoneyTransferredEvent>(OnMoneyTransferred);
                _eventBus.Subscribe<PropertyPurchasedEvent>(OnPropertyPurchased);
                _eventBus.Subscribe<PropertyMortgagedEvent>(OnPropertyMortgaged);
                _eventBus.Subscribe<PropertyUnmortgagedEvent>(OnPropertyUnmortgaged);
                _eventBus.Subscribe<PlayerJailedEvent>(OnPlayerJailed);
                _eventBus.Subscribe<PlayerReleasedFromJailEvent>(OnPlayerReleasedFromJail);
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
                _eventBus.Unsubscribe<MoneyTransferredEvent>(OnMoneyTransferred);
                _eventBus.Unsubscribe<PropertyPurchasedEvent>(OnPropertyPurchased);
                _eventBus.Unsubscribe<PropertyMortgagedEvent>(OnPropertyMortgaged);
                _eventBus.Unsubscribe<PropertyUnmortgagedEvent>(OnPropertyUnmortgaged);
                _eventBus.Unsubscribe<PlayerJailedEvent>(OnPlayerJailed);
                _eventBus.Unsubscribe<PlayerReleasedFromJailEvent>(OnPlayerReleasedFromJail);
            }
        }
        
        /// <summary>
        /// Updates the entire display.
        /// </summary>
        private void UpdateDisplay()
        {
            if (_gameState == null)
                return;
            
            _currentPlayer = _gameState.CurrentPlayer;
            
            if (_currentPlayer == null)
                return;
            
            UpdatePlayerInfo();
            UpdateMoneyDisplay();
            UpdatePropertiesList();
            UpdateJailStatus();
        }
        
        /// <summary>
        /// Updates player name display.
        /// </summary>
        private void UpdatePlayerInfo()
        {
            if (_playerNameText != null && _currentPlayer != null)
                _playerNameText.text = _currentPlayer.Name;
        }
        
        /// <summary>
        /// Updates money and net worth displays.
        /// </summary>
        private void UpdateMoneyDisplay()
        {
            if (_currentPlayer == null)
                return;
            
            // Update money
            if (_moneyText != null)
                _moneyText.text = $"${_currentPlayer.Money}";
            
            // Calculate and update net worth
            if (_netWorthText != null)
            {
                int netWorth = CalculateNetWorth(_currentPlayer);
                _netWorthText.text = $"Net Worth: ${netWorth}";
            }
            
            // Update property count
            if (_propertyCountText != null)
                _propertyCountText.text = $"Properties: {_currentPlayer.Properties.Count}";
        }
        
        /// <summary>
        /// Updates the properties list display.
        /// </summary>
        private void UpdatePropertiesList()
        {
            if (_propertiesContainer == null || _currentPlayer == null)
                return;
            
            // Clear existing items
            foreach (Transform child in _propertiesContainer)
            {
                Destroy(child.gameObject);
            }
            
            // Add property items
            foreach (string propertyName in _currentPlayer.Properties)
            {
                if (_propertyItemPrefab != null)
                {
                    GameObject itemObj = Instantiate(_propertyItemPrefab, _propertiesContainer);
                    TextMeshProUGUI itemText = itemObj.GetComponentInChildren<TextMeshProUGUI>();
                    
                    if (itemText != null)
                    {
                        // Get property details
                        Property property = FindProperty(propertyName);
                        string displayText = propertyName;
                        
                        if (property != null)
                        {
                            if (property.IsMortgaged)
                                displayText += " (MORTGAGED)";
                            else if (property.Houses > 0)
                                displayText += property.Houses < 5 ? $" ({property.Houses}H)" : " (HOTEL)";
                        }
                        
                        itemText.text = displayText;
                    }
                }
            }
        }
        
        /// <summary>
        /// Updates the jail status indicator.
        /// </summary>
        private void UpdateJailStatus()
        {
            if (_jailIndicator == null || _currentPlayer == null)
                return;
            
            bool isInJail = _currentPlayer.IsInJail;
            _jailIndicator.SetActive(isInJail);
            
            if (isInJail && _jailTurnsText != null)
            {
                _jailTurnsText.text = $"In Jail ({_currentPlayer.JailTurns}/3)";
            }
        }
        
        /// <summary>
        /// Calculates the net worth of a player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>The net worth.</returns>
        private int CalculateNetWorth(Player player)
        {
            int netWorth = player.Money;
            
            // Add property values
            foreach (string propertyName in player.Properties)
            {
                Property property = FindProperty(propertyName);
                if (property != null)
                {
                    netWorth += property.Price;
                    netWorth += property.Houses * property.HouseCost;
                }
            }
            
            return netWorth;
        }
        
        /// <summary>
        /// Finds a property by name in the game board.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The property, or null if not found.</returns>
        private Property FindProperty(string propertyName)
        {
            if (_gameState?.Board == null)
                return null;
            
            for (int i = 0; i < 40; i++)
            {
                Space space = _gameState.Board.GetSpace(i);
                if (space is Property property && property.Name == propertyName)
                    return property;
            }
            
            return null;
        }
        
        #region Event Handlers
        
        /// <summary>
        /// Handles turn started event.
        /// </summary>
        private void OnTurnStarted(TurnStartedEvent evt)
        {
            UpdateDisplay();
        }
        
        /// <summary>
        /// Handles money transferred event.
        /// </summary>
        private void OnMoneyTransferred(MoneyTransferredEvent evt)
        {
            // Update if current player is involved
            if (_currentPlayer != null && 
                (evt.FromPlayerId == _currentPlayer.Id || evt.ToPlayerId == _currentPlayer.Id))
            {
                UpdateMoneyDisplay();
            }
        }
        
        /// <summary>
        /// Handles property purchased event.
        /// </summary>
        private void OnPropertyPurchased(PropertyPurchasedEvent evt)
        {
            if (_currentPlayer != null && evt.PlayerId == _currentPlayer.Id)
            {
                UpdateMoneyDisplay();
                UpdatePropertiesList();
            }
        }
        
        /// <summary>
        /// Handles property mortgaged event.
        /// </summary>
        private void OnPropertyMortgaged(PropertyMortgagedEvent evt)
        {
            if (_currentPlayer != null && evt.PlayerId == _currentPlayer.Id)
            {
                UpdateMoneyDisplay();
                UpdatePropertiesList();
            }
        }
        
        /// <summary>
        /// Handles property unmortgaged event.
        /// </summary>
        private void OnPropertyUnmortgaged(PropertyUnmortgagedEvent evt)
        {
            if (_currentPlayer != null && evt.PlayerId == _currentPlayer.Id)
            {
                UpdateMoneyDisplay();
                UpdatePropertiesList();
            }
        }
        
        /// <summary>
        /// Handles player jailed event.
        /// </summary>
        private void OnPlayerJailed(PlayerJailedEvent evt)
        {
            if (_currentPlayer != null && evt.PlayerId == _currentPlayer.Id)
            {
                UpdateJailStatus();
            }
        }
        
        /// <summary>
        /// Handles player released from jail event.
        /// </summary>
        private void OnPlayerReleasedFromJail(PlayerReleasedFromJailEvent evt)
        {
            if (_currentPlayer != null && evt.PlayerId == _currentPlayer.Id)
            {
                UpdateJailStatus();
            }
        }
        
        #endregion
    }
}
