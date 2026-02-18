using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MonopolyFrenzy.Core;
using MonopolyFrenzy.Commands;

namespace MonopolyFrenzy.UI.Modals
{
    /// <summary>
    /// Modal dialog displaying property information and available actions.
    /// </summary>
    public class PropertyCardModal : ModalDialog
    {
        [Header("Property Info")]
        [SerializeField] private TextMeshProUGUI _propertyNameText;
        [SerializeField] private TextMeshProUGUI _colorGroupText;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private Image _colorBar;
        
        [Header("Rent Structure")]
        [SerializeField] private TextMeshProUGUI _baseRentText;
        [SerializeField] private TextMeshProUGUI _rent1HouseText;
        [SerializeField] private TextMeshProUGUI _rent2HouseText;
        [SerializeField] private TextMeshProUGUI _rent3HouseText;
        [SerializeField] private TextMeshProUGUI _rent4HouseText;
        [SerializeField] private TextMeshProUGUI _rentHotelText;
        
        [Header("Additional Info")]
        [SerializeField] private TextMeshProUGUI _houseCostText;
        [SerializeField] private TextMeshProUGUI _mortgageValueText;
        [SerializeField] private TextMeshProUGUI _ownerText;
        [SerializeField] private TextMeshProUGUI _statusText;
        
        [Header("Action Buttons")]
        [SerializeField] private Button _buyButton;
        [SerializeField] private Button _declineButton;
        [SerializeField] private Button _mortgageButton;
        [SerializeField] private Button _unmortgageButton;
        [SerializeField] private Button _buildButton;
        
        private Property _property;
        private GameState _gameState;
        private Player _currentPlayer;
        
        /// <summary>
        /// Event fired when property is purchased.
        /// </summary>
        public event System.Action<Property> OnPropertyPurchased;
        
        /// <summary>
        /// Event fired when property is mortgaged.
        /// </summary>
        public event System.Action<Property> OnPropertyMortgaged;
        
        /// <summary>
        /// Event fired when property is unmortgaged.
        /// </summary>
        public event System.Action<Property> OnPropertyUnmortgaged;
        
        protected override void Awake()
        {
            base.Awake();
            
            if (_buyButton != null)
                _buyButton.onClick.AddListener(OnBuyClicked);
            
            if (_declineButton != null)
                _declineButton.onClick.AddListener(OnDeclineClicked);
            
            if (_mortgageButton != null)
                _mortgageButton.onClick.AddListener(OnMortgageClicked);
            
            if (_unmortgageButton != null)
                _unmortgageButton.onClick.AddListener(OnUnmortgageClicked);
            
            if (_buildButton != null)
                _buildButton.onClick.AddListener(OnBuildClicked);
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if (_buyButton != null)
                _buyButton.onClick.RemoveListener(OnBuyClicked);
            
            if (_declineButton != null)
                _declineButton.onClick.RemoveListener(OnDeclineClicked);
            
            if (_mortgageButton != null)
                _mortgageButton.onClick.RemoveListener(OnMortgageClicked);
            
            if (_unmortgageButton != null)
                _unmortgageButton.onClick.RemoveListener(OnUnmortgageClicked);
            
            if (_buildButton != null)
                _buildButton.onClick.RemoveListener(OnBuildClicked);
        }
        
        /// <summary>
        /// Shows the property card modal for a specific property.
        /// </summary>
        /// <param name="property">The property to display.</param>
        /// <param name="gameState">The current game state.</param>
        /// <param name="showBuyOption">Whether to show buy/decline buttons (landing on unowned property).</param>
        public void ShowProperty(Property property, GameState gameState, bool showBuyOption = false)
        {
            _property = property;
            _gameState = gameState;
            _currentPlayer = gameState?.CurrentPlayer;
            
            UpdateDisplay(showBuyOption);
            Show();
        }
        
        /// <summary>
        /// Updates the modal display with property information.
        /// </summary>
        /// <param name="showBuyOption">Whether to show buy/decline buttons.</param>
        private void UpdateDisplay(bool showBuyOption)
        {
            if (_property == null)
                return;
            
            // Property name and color
            if (_propertyNameText != null)
                _propertyNameText.text = _property.Name;
            
            if (_colorGroupText != null)
                _colorGroupText.text = $"Color Group: {_property.ColorGroup}";
            
            if (_colorBar != null)
                _colorBar.color = GetColorGroupColor(_property.ColorGroup);
            
            // Price
            if (_priceText != null)
                _priceText.text = $"Price: ${_property.Price}";
            
            // Rent structure
            if (_baseRentText != null)
                _baseRentText.text = $"Base Rent: ${_property.BaseRent}";
            
            if (_rent1HouseText != null && _property.RentWith1House.HasValue)
                _rent1HouseText.text = $"1 House: ${_property.RentWith1House.Value}";
            
            if (_rent2HouseText != null && _property.RentWith2Houses.HasValue)
                _rent2HouseText.text = $"2 Houses: ${_property.RentWith2Houses.Value}";
            
            if (_rent3HouseText != null && _property.RentWith3Houses.HasValue)
                _rent3HouseText.text = $"3 Houses: ${_property.RentWith3Houses.Value}";
            
            if (_rent4HouseText != null && _property.RentWith4Houses.HasValue)
                _rent4HouseText.text = $"4 Houses: ${_property.RentWith4Houses.Value}";
            
            if (_rentHotelText != null && _property.RentWithHotel.HasValue)
                _rentHotelText.text = $"Hotel: ${_property.RentWithHotel.Value}";
            
            // Additional info
            if (_houseCostText != null)
                _houseCostText.text = $"House Cost: ${_property.HouseCost}";
            
            if (_mortgageValueText != null)
                _mortgageValueText.text = $"Mortgage Value: ${_property.MortgageValue}";
            
            // Owner and status
            if (_ownerText != null)
            {
                if (string.IsNullOrEmpty(_property.OwnerId))
                    _ownerText.text = "Owner: None (Available for purchase)";
                else
                {
                    Player owner = _gameState?.Players.Find(p => p.Id == _property.OwnerId);
                    _ownerText.text = owner != null ? $"Owner: {owner.Name}" : "Owner: Unknown";
                }
            }
            
            if (_statusText != null)
            {
                if (_property.IsMortgaged)
                    _statusText.text = "Status: MORTGAGED";
                else if (_property.Houses == 5)
                    _statusText.text = "Status: Hotel";
                else if (_property.Houses > 0)
                    _statusText.text = $"Status: {_property.Houses} House(s)";
                else
                    _statusText.text = "Status: Undeveloped";
            }
            
            // Update buttons
            UpdateButtons(showBuyOption);
        }
        
        /// <summary>
        /// Updates button visibility and enabled states.
        /// </summary>
        /// <param name="showBuyOption">Whether to show buy/decline buttons.</param>
        private void UpdateButtons(bool showBuyOption)
        {
            bool isUnowned = string.IsNullOrEmpty(_property.OwnerId);
            bool isOwnedByCurrentPlayer = _currentPlayer != null && _property.OwnerId == _currentPlayer.Id;
            bool canAffordPurchase = _currentPlayer != null && _currentPlayer.Money >= _property.Price;
            bool canAffordUnmortgage = _currentPlayer != null && _currentPlayer.Money >= (_property.MortgageValue * 1.1f);
            
            // Buy/Decline buttons (landing on unowned property)
            if (_buyButton != null)
            {
                _buyButton.gameObject.SetActive(showBuyOption && isUnowned);
                _buyButton.interactable = canAffordPurchase;
            }
            
            if (_declineButton != null)
                _declineButton.gameObject.SetActive(showBuyOption && isUnowned);
            
            // Mortgage/Unmortgage buttons (viewing owned property)
            if (_mortgageButton != null)
            {
                _mortgageButton.gameObject.SetActive(isOwnedByCurrentPlayer && !_property.IsMortgaged);
                _mortgageButton.interactable = _property.Houses == 0; // Can only mortgage if no houses
            }
            
            if (_unmortgageButton != null)
            {
                _unmortgageButton.gameObject.SetActive(isOwnedByCurrentPlayer && _property.IsMortgaged);
                _unmortgageButton.interactable = canAffordUnmortgage;
            }
            
            // Build button (viewing owned property in monopoly)
            if (_buildButton != null)
            {
                bool hasMonopoly = false; // TODO: Check if player owns complete color group
                _buildButton.gameObject.SetActive(isOwnedByCurrentPlayer && !_property.IsMortgaged);
                _buildButton.interactable = hasMonopoly;
            }
        }
        
        #region Button Handlers
        
        /// <summary>
        /// Handles Buy button click.
        /// </summary>
        private void OnBuyClicked()
        {
            if (_property == null || _currentPlayer == null || _gameState == null)
                return;
            
            var command = new BuyPropertyCommand(_currentPlayer, _property);
            var result = command.Execute(_gameState);
            
            if (result.Success)
            {
                OnPropertyPurchased?.Invoke(_property);
                Hide();
            }
            else
            {
                Debug.LogWarning($"Failed to buy property: {result.Message}");
            }
        }
        
        /// <summary>
        /// Handles Decline button click.
        /// </summary>
        private void OnDeclineClicked()
        {
            Hide();
        }
        
        /// <summary>
        /// Handles Mortgage button click.
        /// </summary>
        private void OnMortgageClicked()
        {
            if (_property == null || _currentPlayer == null || _gameState == null)
                return;
            
            var command = new MortgageCommand(_currentPlayer, _property);
            var result = command.Execute(_gameState);
            
            if (result.Success)
            {
                OnPropertyMortgaged?.Invoke(_property);
                UpdateDisplay(false); // Refresh display
            }
            else
            {
                Debug.LogWarning($"Failed to mortgage property: {result.Message}");
            }
        }
        
        /// <summary>
        /// Handles Unmortgage button click.
        /// </summary>
        private void OnUnmortgageClicked()
        {
            if (_property == null || _currentPlayer == null || _gameState == null)
                return;
            
            var command = new UnmortgageCommand(_currentPlayer, _property);
            var result = command.Execute(_gameState);
            
            if (result.Success)
            {
                OnPropertyUnmortgaged?.Invoke(_property);
                UpdateDisplay(false); // Refresh display
            }
            else
            {
                Debug.LogWarning($"Failed to unmortgage property: {result.Message}");
            }
        }
        
        /// <summary>
        /// Handles Build button click.
        /// </summary>
        private void OnBuildClicked()
        {
            Debug.Log("Build button clicked");
            // TODO: Open build houses modal (separate modal for house selection)
            // This will be implemented in BuildHousesModal
        }
        
        #endregion
        
        /// <summary>
        /// Gets the color for a property color group.
        /// </summary>
        private Color GetColorGroupColor(string colorGroup)
        {
            return colorGroup switch
            {
                "Brown" => new Color(0.55f, 0.27f, 0.07f),
                "Light Blue" => new Color(0.68f, 0.85f, 0.90f),
                "Pink" => new Color(1.0f, 0.41f, 0.71f),
                "Orange" => new Color(1.0f, 0.55f, 0.0f),
                "Red" => new Color(0.93f, 0.11f, 0.14f),
                "Yellow" => new Color(1.0f, 0.92f, 0.23f),
                "Green" => new Color(0.0f, 0.62f, 0.38f),
                "Dark Blue" => new Color(0.0f, 0.32f, 0.73f),
                _ => Color.white
            };
        }
    }
}
