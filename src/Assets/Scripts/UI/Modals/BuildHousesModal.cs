using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MonopolyFrenzy.Core;
using MonopolyFrenzy.Commands;

namespace MonopolyFrenzy.UI.Modals
{
    /// <summary>
    /// Modal dialog for building houses and hotels on properties.
    /// Enforces even building rule and monopoly requirements.
    /// </summary>
    public class BuildHousesModal : ModalDialog
    {
        [Header("Content")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _instructionsText;
        [SerializeField] private TextMeshProUGUI _availableHousesText;
        [SerializeField] private TextMeshProUGUI _availableHotelsText;
        
        [Header("Property List")]
        [SerializeField] private Transform _propertyListContainer;
        [SerializeField] private GameObject _propertyItemPrefab;
        
        [Header("Buttons")]
        [SerializeField] private Button _buildButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private TextMeshProUGUI _totalCostText;
        
        private GameState _gameState;
        private Player _currentPlayer;
        private List<PropertyBuildItem> _buildItems = new List<PropertyBuildItem>();
        
        private const int MAX_HOUSES = 32;
        private const int MAX_HOTELS = 12;
        private int _housesInBank;
        private int _hotelsInBank;
        
        /// <summary>
        /// Event fired when houses are built.
        /// </summary>
        public event System.Action OnHousesBuilt;
        
        protected override void Awake()
        {
            base.Awake();
            
            if (_buildButton != null)
                _buildButton.onClick.AddListener(OnBuildClicked);
            
            if (_cancelButton != null)
                _cancelButton.onClick.AddListener(OnCancelClicked);
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if (_buildButton != null)
                _buildButton.onClick.RemoveListener(OnBuildClicked);
            
            if (_cancelButton != null)
                _cancelButton.onClick.RemoveListener(OnCancelClicked);
        }
        
        /// <summary>
        /// Shows the build houses modal for the current player.
        /// </summary>
        /// <param name="gameState">The current game state.</param>
        public void ShowBuildOptions(GameState gameState)
        {
            _gameState = gameState;
            _currentPlayer = gameState?.CurrentPlayer;
            
            if (_currentPlayer == null)
            {
                Debug.LogWarning("No current player to show build options!");
                return;
            }
            
            CalculateAvailableBuildings();
            PopulatePropertyList();
            UpdateDisplay();
            Show();
        }
        
        /// <summary>
        /// Calculates available houses and hotels remaining in the bank.
        /// </summary>
        private void CalculateAvailableBuildings()
        {
            // Count existing houses and hotels on the board
            int housesOnBoard = 0;
            int hotelsOnBoard = 0;
            
            if (_gameState?.Board != null)
            {
                for (int i = 0; i < 40; i++)
                {
                    if (_gameState.Board.GetSpace(i) is Property property)
                    {
                        if (property.Houses == 5)
                            hotelsOnBoard++;
                        else
                            housesOnBoard += property.Houses;
                    }
                }
            }
            
            _housesInBank = MAX_HOUSES - housesOnBoard;
            _hotelsInBank = MAX_HOTELS - hotelsOnBoard;
        }
        
        /// <summary>
        /// Populates the property list with buildable properties.
        /// </summary>
        private void PopulatePropertyList()
        {
            _buildItems.Clear();
            
            if (_propertyListContainer == null || _propertyItemPrefab == null)
                return;
            
            // Clear existing items
            foreach (Transform child in _propertyListContainer)
            {
                Destroy(child.gameObject);
            }
            
            // Get monopolies owned by player
            var monopolies = GetPlayerMonopolies();
            
            foreach (var colorGroup in monopolies)
            {
                var properties = GetPropertiesInColorGroup(colorGroup);
                
                foreach (var property in properties)
                {
                    GameObject itemObj = Instantiate(_propertyItemPrefab, _propertyListContainer);
                    PropertyBuildItem buildItem = itemObj.GetComponent<PropertyBuildItem>();
                    
                    if (buildItem != null)
                    {
                        buildItem.Initialize(property, properties, _currentPlayer.Money);
                        buildItem.OnHousesChanged += OnPropertyHousesChanged;
                        _buildItems.Add(buildItem);
                    }
                }
            }
            
            if (_buildItems.Count == 0 && _instructionsText != null)
            {
                _instructionsText.text = "You don't own any complete color groups to build on.";
            }
        }
        
        /// <summary>
        /// Gets all monopolies (complete color groups) owned by the current player.
        /// </summary>
        private List<string> GetPlayerMonopolies()
        {
            var monopolies = new List<string>();
            
            if (_gameState?.Board == null || _currentPlayer == null)
                return monopolies;
            
            // Group properties by color
            var colorGroups = new Dictionary<string, List<Property>>();
            
            for (int i = 0; i < 40; i++)
            {
                if (_gameState.Board.GetSpace(i) is Property property)
                {
                    if (!colorGroups.ContainsKey(property.ColorGroup))
                        colorGroups[property.ColorGroup] = new List<Property>();
                    
                    colorGroups[property.ColorGroup].Add(property);
                }
            }
            
            // Check each color group for monopoly
            foreach (var kvp in colorGroups)
            {
                string colorGroup = kvp.Key;
                var properties = kvp.Value;
                
                // Check if player owns all properties in this group
                bool ownsAll = properties.All(p => p.OwnerId == _currentPlayer.Id);
                
                // Check if any property is mortgaged
                bool anyMortgaged = properties.Any(p => p.IsMortgaged);
                
                if (ownsAll && !anyMortgaged)
                {
                    monopolies.Add(colorGroup);
                }
            }
            
            return monopolies;
        }
        
        /// <summary>
        /// Gets all properties in a color group.
        /// </summary>
        private List<Property> GetPropertiesInColorGroup(string colorGroup)
        {
            var properties = new List<Property>();
            
            if (_gameState?.Board == null)
                return properties;
            
            for (int i = 0; i < 40; i++)
            {
                if (_gameState.Board.GetSpace(i) is Property property && property.ColorGroup == colorGroup)
                {
                    properties.Add(property);
                }
            }
            
            return properties;
        }
        
        /// <summary>
        /// Updates the display with current state.
        /// </summary>
        private void UpdateDisplay()
        {
            if (_availableHousesText != null)
                _availableHousesText.text = $"Houses Available: {_housesInBank}";
            
            if (_availableHotelsText != null)
                _availableHotelsText.text = $"Hotels Available: {_hotelsInBank}";
            
            UpdateTotalCost();
        }
        
        /// <summary>
        /// Updates the total cost display and build button state.
        /// </summary>
        private void UpdateTotalCost()
        {
            int totalCost = 0;
            int totalHousesNeeded = 0;
            int totalHotelsNeeded = 0;
            
            foreach (var item in _buildItems)
            {
                int houseDiff = item.TargetHouses - item.Property.Houses;
                if (houseDiff > 0)
                {
                    // Building houses
                    totalCost += houseDiff * item.Property.HouseCost;
                    totalHousesNeeded += houseDiff;
                }
                else if (houseDiff == -4)
                {
                    // Upgrading 4 houses to hotel
                    totalCost += item.Property.HouseCost;
                    totalHotelsNeeded++;
                    totalHousesNeeded -= 4; // Returning 4 houses to bank
                }
            }
            
            if (_totalCostText != null)
                _totalCostText.text = $"Total Cost: ${totalCost}";
            
            // Enable/disable build button
            if (_buildButton != null)
            {
                bool canAfford = _currentPlayer != null && _currentPlayer.Money >= totalCost;
                bool hasEnoughHouses = totalHousesNeeded <= _housesInBank;
                bool hasEnoughHotels = totalHotelsNeeded <= _hotelsInBank;
                bool evenBuilding = ValidateEvenBuilding();
                
                _buildButton.interactable = canAfford && hasEnoughHouses && hasEnoughHotels && evenBuilding && totalCost > 0;
            }
        }
        
        /// <summary>
        /// Validates even building rule across color groups.
        /// </summary>
        private bool ValidateEvenBuilding()
        {
            // Group by color group
            var groupedItems = _buildItems.GroupBy(item => item.Property.ColorGroup);
            
            foreach (var group in groupedItems)
            {
                int minHouses = group.Min(item => item.TargetHouses);
                int maxHouses = group.Max(item => item.TargetHouses);
                
                // Difference can't be more than 1
                if (maxHouses - minHouses > 1)
                    return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Handles property houses changed event.
        /// </summary>
        private void OnPropertyHousesChanged(PropertyBuildItem item)
        {
            UpdateTotalCost();
        }
        
        /// <summary>
        /// Handles Build button click.
        /// </summary>
        private void OnBuildClicked()
        {
            if (_gameState == null || _currentPlayer == null)
                return;
            
            // Execute build commands for each property with changes
            bool anyBuilt = false;
            
            foreach (var item in _buildItems)
            {
                int housesToBuild = item.TargetHouses - item.Property.Houses;
                
                if (housesToBuild != 0)
                {
                    var command = new BuyHouseCommand(_currentPlayer, item.Property, housesToBuild);
                    var result = command.Execute(_gameState);
                    
                    if (result.Success)
                    {
                        anyBuilt = true;
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to build on {item.Property.Name}: {result.Message}");
                    }
                }
            }
            
            if (anyBuilt)
            {
                OnHousesBuilt?.Invoke();
                Hide();
            }
        }
        
        /// <summary>
        /// Handles Cancel button click.
        /// </summary>
        private void OnCancelClicked()
        {
            Hide();
        }
    }
    
    /// <summary>
    /// UI component for a single property in the build houses list.
    /// Allows selecting number of houses to build.
    /// </summary>
    public class PropertyBuildItem : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _propertyNameText;
        [SerializeField] private TextMeshProUGUI _currentHousesText;
        [SerializeField] private Button _minusButton;
        [SerializeField] private Button _plusButton;
        [SerializeField] private TextMeshProUGUI _targetHousesText;
        
        /// <summary>
        /// Gets the property.
        /// </summary>
        public Property Property { get; private set; }
        
        /// <summary>
        /// Gets the target number of houses.
        /// </summary>
        public int TargetHouses { get; private set; }
        
        /// <summary>
        /// Event fired when target houses changed.
        /// </summary>
        public event System.Action<PropertyBuildItem> OnHousesChanged;
        
        private List<Property> _colorGroupProperties;
        private int _playerMoney;
        
        private void Awake()
        {
            if (_minusButton != null)
                _minusButton.onClick.AddListener(OnMinusClicked);
            
            if (_plusButton != null)
                _plusButton.onClick.AddListener(OnPlusClicked);
        }
        
        private void OnDestroy()
        {
            if (_minusButton != null)
                _minusButton.onClick.RemoveListener(OnMinusClicked);
            
            if (_plusButton != null)
                _plusButton.onClick.RemoveListener(OnPlusClicked);
        }
        
        /// <summary>
        /// Initializes the property build item.
        /// </summary>
        public void Initialize(Property property, List<Property> colorGroupProperties, int playerMoney)
        {
            Property = property;
            _colorGroupProperties = colorGroupProperties;
            _playerMoney = playerMoney;
            TargetHouses = property.Houses;
            
            UpdateDisplay();
        }
        
        /// <summary>
        /// Updates the display.
        /// </summary>
        private void UpdateDisplay()
        {
            if (_propertyNameText != null)
                _propertyNameText.text = Property.Name;
            
            if (_currentHousesText != null)
                _currentHousesText.text = Property.Houses < 5 ? $"Current: {Property.Houses} houses" : "Current: Hotel";
            
            if (_targetHousesText != null)
                _targetHousesText.text = TargetHouses < 5 ? $"{TargetHouses}" : "Hotel";
            
            UpdateButtons();
        }
        
        /// <summary>
        /// Updates button states.
        /// </summary>
        private void UpdateButtons()
        {
            if (_minusButton != null)
                _minusButton.interactable = TargetHouses > Property.Houses;
            
            if (_plusButton != null)
                _plusButton.interactable = TargetHouses < 5;
        }
        
        /// <summary>
        /// Handles minus button click.
        /// </summary>
        private void OnMinusClicked()
        {
            if (TargetHouses > Property.Houses)
            {
                TargetHouses--;
                UpdateDisplay();
                OnHousesChanged?.Invoke(this);
            }
        }
        
        /// <summary>
        /// Handles plus button click.
        /// </summary>
        private void OnPlusClicked()
        {
            if (TargetHouses < 5)
            {
                TargetHouses++;
                UpdateDisplay();
                OnHousesChanged?.Invoke(this);
            }
        }
    }
}
