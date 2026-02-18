using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MonopolyFrenzy.UI.Controllers;

namespace MonopolyFrenzy.UI.Screens
{
    /// <summary>
    /// Game setup screen controller.
    /// Allows players to configure game settings before starting.
    /// </summary>
    public class GameSetupScreen : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Button _addPlayerButton;
        [SerializeField] private Transform _playerListContainer;
        [SerializeField] private GameObject _playerItemPrefab;
        [SerializeField] private Slider _startingMoneySlider;
        [SerializeField] private TextMeshProUGUI _startingMoneyText;
        
        [Header("Settings")]
        [SerializeField] private int _minPlayers = 2;
        [SerializeField] private int _maxPlayers = 6;
        [SerializeField] private int _defaultStartingMoney = 1500;
        [SerializeField] private int _minStartingMoney = 500;
        [SerializeField] private int _maxStartingMoney = 5000;
        
        private List<PlayerSetupItem> _playerItems = new List<PlayerSetupItem>();
        private int _startingMoney;
        
        // Available token choices
        private readonly string[] _availableTokens = new string[]
        {
            "Car", "Hat", "Ship", "Dog", "Thimble", "Iron", "Shoe", "Wheelbarrow"
        };
        
        private void Awake()
        {
            ValidateReferences();
            _startingMoney = _defaultStartingMoney;
        }
        
        private void Start()
        {
            SetupButtonListeners();
            InitializeDefaultPlayers();
            UpdateStartingMoneyDisplay();
        }
        
        private void OnDestroy()
        {
            RemoveButtonListeners();
        }
        
        /// <summary>
        /// Validates that all required UI references are assigned.
        /// </summary>
        private void ValidateReferences()
        {
            if (_backButton == null)
                Debug.LogError("Back Button is not assigned in GameSetupScreen!", this);
            
            if (_startGameButton == null)
                Debug.LogError("Start Game Button is not assigned in GameSetupScreen!", this);
            
            if (_addPlayerButton == null)
                Debug.LogError("Add Player Button is not assigned in GameSetupScreen!", this);
            
            if (_playerListContainer == null)
                Debug.LogError("Player List Container is not assigned in GameSetupScreen!", this);
            
            if (_playerItemPrefab == null)
                Debug.LogError("Player Item Prefab is not assigned in GameSetupScreen!", this);
            
            if (_startingMoneySlider == null)
                Debug.LogError("Starting Money Slider is not assigned in GameSetupScreen!", this);
            
            if (_startingMoneyText == null)
                Debug.LogError("Starting Money Text is not assigned in GameSetupScreen!", this);
        }
        
        /// <summary>
        /// Sets up button click listeners.
        /// </summary>
        private void SetupButtonListeners()
        {
            if (_backButton != null)
                _backButton.onClick.AddListener(OnBackClicked);
            
            if (_startGameButton != null)
                _startGameButton.onClick.AddListener(OnStartGameClicked);
            
            if (_addPlayerButton != null)
                _addPlayerButton.onClick.AddListener(OnAddPlayerClicked);
            
            if (_startingMoneySlider != null)
            {
                _startingMoneySlider.minValue = _minStartingMoney;
                _startingMoneySlider.maxValue = _maxStartingMoney;
                _startingMoneySlider.value = _defaultStartingMoney;
                _startingMoneySlider.onValueChanged.AddListener(OnStartingMoneyChanged);
            }
        }
        
        /// <summary>
        /// Removes button click listeners to prevent memory leaks.
        /// </summary>
        private void RemoveButtonListeners()
        {
            if (_backButton != null)
                _backButton.onClick.RemoveListener(OnBackClicked);
            
            if (_startGameButton != null)
                _startGameButton.onClick.RemoveListener(OnStartGameClicked);
            
            if (_addPlayerButton != null)
                _addPlayerButton.onClick.RemoveListener(OnAddPlayerClicked);
            
            if (_startingMoneySlider != null)
                _startingMoneySlider.onValueChanged.RemoveListener(OnStartingMoneyChanged);
        }
        
        /// <summary>
        /// Initializes default players (2 players).
        /// </summary>
        private void InitializeDefaultPlayers()
        {
            AddPlayer("Player 1", _availableTokens[0]);
            AddPlayer("Player 2", _availableTokens[1]);
        }
        
        /// <summary>
        /// Adds a player to the setup list.
        /// </summary>
        /// <param name="playerName">Name of the player.</param>
        /// <param name="token">Token choice for the player.</param>
        private void AddPlayer(string playerName, string token)
        {
            if (_playerItems.Count >= _maxPlayers)
            {
                Debug.LogWarning($"Cannot add more than {_maxPlayers} players");
                return;
            }
            
            if (_playerItemPrefab == null || _playerListContainer == null)
                return;
            
            GameObject playerItemObj = Instantiate(_playerItemPrefab, _playerListContainer);
            PlayerSetupItem playerItem = playerItemObj.GetComponent<PlayerSetupItem>();
            
            if (playerItem != null)
            {
                playerItem.Initialize(playerName, _availableTokens, token);
                playerItem.OnRemoveClicked += RemovePlayer;
                _playerItems.Add(playerItem);
            }
            
            UpdateUI();
        }
        
        /// <summary>
        /// Removes a player from the setup list.
        /// </summary>
        /// <param name="playerItem">The player item to remove.</param>
        private void RemovePlayer(PlayerSetupItem playerItem)
        {
            if (_playerItems.Count <= _minPlayers)
            {
                Debug.LogWarning($"Cannot have fewer than {_minPlayers} players");
                return;
            }
            
            _playerItems.Remove(playerItem);
            playerItem.OnRemoveClicked -= RemovePlayer;
            Destroy(playerItem.gameObject);
            
            UpdateUI();
        }
        
        /// <summary>
        /// Updates UI state (button enabled states).
        /// </summary>
        private void UpdateUI()
        {
            if (_addPlayerButton != null)
                _addPlayerButton.interactable = _playerItems.Count < _maxPlayers;
            
            if (_startGameButton != null)
                _startGameButton.interactable = ValidateSetup();
        }
        
        /// <summary>
        /// Updates the starting money display text.
        /// </summary>
        private void UpdateStartingMoneyDisplay()
        {
            if (_startingMoneyText != null)
                _startingMoneyText.text = $"Starting Money: ${_startingMoney}";
        }
        
        /// <summary>
        /// Validates the current setup configuration.
        /// </summary>
        /// <returns>True if setup is valid, false otherwise.</returns>
        private bool ValidateSetup()
        {
            // Check player count
            if (_playerItems.Count < _minPlayers || _playerItems.Count > _maxPlayers)
                return false;
            
            // Check for unique names
            var names = _playerItems.Select(p => p.PlayerName).ToList();
            if (names.Distinct().Count() != names.Count)
                return false;
            
            // Check for unique tokens
            var tokens = _playerItems.Select(p => p.SelectedToken).ToList();
            if (tokens.Distinct().Count() != tokens.Count)
                return false;
            
            // Check for empty names
            if (names.Any(string.IsNullOrWhiteSpace))
                return false;
            
            return true;
        }
        
        /// <summary>
        /// Handles Back button click.
        /// </summary>
        private void OnBackClicked()
        {
            UIController.Instance.LoadScene("MainMenu");
        }
        
        /// <summary>
        /// Handles Start Game button click.
        /// </summary>
        private void OnStartGameClicked()
        {
            if (!ValidateSetup())
            {
                Debug.LogWarning("Invalid game setup configuration");
                return;
            }
            
            // Get player names and tokens
            string[] playerNames = _playerItems.Select(p => p.PlayerName).ToArray();
            string[] playerTokens = _playerItems.Select(p => p.SelectedToken).ToArray();
            
            // Initialize game
            UIController.Instance.InitializeNewGame(playerNames, playerTokens, _startingMoney);
            
            // Load game board scene
            UIController.Instance.LoadScene("GameBoard");
        }
        
        /// <summary>
        /// Handles Add Player button click.
        /// </summary>
        private void OnAddPlayerClicked()
        {
            int playerNumber = _playerItems.Count + 1;
            string defaultName = $"Player {playerNumber}";
            
            // Find first available token
            var usedTokens = _playerItems.Select(p => p.SelectedToken).ToHashSet();
            string availableToken = _availableTokens.FirstOrDefault(t => !usedTokens.Contains(t));
            
            if (availableToken != null)
            {
                AddPlayer(defaultName, availableToken);
            }
        }
        
        /// <summary>
        /// Handles starting money slider value change.
        /// </summary>
        /// <param name="value">New slider value.</param>
        private void OnStartingMoneyChanged(float value)
        {
            _startingMoney = Mathf.RoundToInt(value / 100) * 100; // Round to nearest 100
            UpdateStartingMoneyDisplay();
        }
    }
    
    /// <summary>
    /// Represents a single player item in the setup list.
    /// This would typically be in a separate file, but included here for simplicity.
    /// </summary>
    public class PlayerSetupItem : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_InputField _nameInputField;
        [SerializeField] private TMP_Dropdown _tokenDropdown;
        [SerializeField] private Button _removeButton;
        
        /// <summary>
        /// Event fired when the remove button is clicked.
        /// </summary>
        public event System.Action<PlayerSetupItem> OnRemoveClicked;
        
        /// <summary>
        /// Gets the current player name.
        /// </summary>
        public string PlayerName => _nameInputField != null ? _nameInputField.text : "";
        
        /// <summary>
        /// Gets the currently selected token.
        /// </summary>
        public string SelectedToken
        {
            get
            {
                if (_tokenDropdown != null && _tokenDropdown.options.Count > 0)
                    return _tokenDropdown.options[_tokenDropdown.value].text;
                return "";
            }
        }
        
        private void Start()
        {
            if (_removeButton != null)
                _removeButton.onClick.AddListener(HandleRemoveClicked);
        }
        
        private void OnDestroy()
        {
            if (_removeButton != null)
                _removeButton.onClick.RemoveListener(HandleRemoveClicked);
        }
        
        /// <summary>
        /// Initializes the player setup item.
        /// </summary>
        /// <param name="playerName">Default player name.</param>
        /// <param name="availableTokens">List of available tokens.</param>
        /// <param name="selectedToken">Initially selected token.</param>
        public void Initialize(string playerName, string[] availableTokens, string selectedToken)
        {
            if (_nameInputField != null)
                _nameInputField.text = playerName;
            
            if (_tokenDropdown != null)
            {
                _tokenDropdown.ClearOptions();
                _tokenDropdown.AddOptions(availableTokens.ToList());
                
                // Set selected token
                int tokenIndex = System.Array.IndexOf(availableTokens, selectedToken);
                if (tokenIndex >= 0)
                    _tokenDropdown.value = tokenIndex;
            }
        }
        
        /// <summary>
        /// Handles remove button click.
        /// </summary>
        private void HandleRemoveClicked()
        {
            OnRemoveClicked?.Invoke(this);
        }
    }
}
