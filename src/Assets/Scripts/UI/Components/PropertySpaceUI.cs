using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MonopolyFrenzy.Core;

namespace MonopolyFrenzy.UI.Components
{
    /// <summary>
    /// UI component representing a single space on the Monopoly board.
    /// Displays property information, ownership, houses, and mortgage status.
    /// </summary>
    public class PropertySpaceUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private Image _colorBar;
        [SerializeField] private Image _ownershipIndicator;
        [SerializeField] private GameObject _housesContainer;
        [SerializeField] private Image[] _houseIcons;
        [SerializeField] private GameObject _mortgageOverlay;
        [SerializeField] private Button _spaceButton;
        
        [Header("Colors")]
        [SerializeField] private Color _unownedColor = Color.white;
        [SerializeField] private Color _mortgagedColor = Color.gray;
        
        private Space _space;
        private int _spaceIndex;
        private string _currentOwnerId;
        private bool _isMortgaged;
        
        /// <summary>
        /// Gets the space index.
        /// </summary>
        public int SpaceIndex => _spaceIndex;
        
        /// <summary>
        /// Gets the space data.
        /// </summary>
        public Space Space => _space;
        
        /// <summary>
        /// Event fired when the space is clicked.
        /// </summary>
        public event System.Action<PropertySpaceUI> OnSpaceClicked;
        
        private void Awake()
        {
            if (_spaceButton != null)
                _spaceButton.onClick.AddListener(HandleSpaceClicked);
        }
        
        private void OnDestroy()
        {
            if (_spaceButton != null)
                _spaceButton.onClick.RemoveListener(HandleSpaceClicked);
        }
        
        /// <summary>
        /// Initializes the property space UI.
        /// </summary>
        /// <param name="space">The space data.</param>
        /// <param name="spaceIndex">The index of this space (0-39).</param>
        public void Initialize(Space space, int spaceIndex)
        {
            _space = space;
            _spaceIndex = spaceIndex;
            
            UpdateDisplay();
        }
        
        /// <summary>
        /// Updates the visual display of the space.
        /// </summary>
        private void UpdateDisplay()
        {
            if (_space == null)
                return;
            
            // Update name
            if (_nameText != null)
                _nameText.text = _space.Name;
            
            // Update color bar for properties
            if (_colorBar != null)
            {
                if (_space is Property property)
                {
                    _colorBar.gameObject.SetActive(true);
                    _colorBar.color = GetColorGroupColor(property.ColorGroup);
                }
                else
                {
                    _colorBar.gameObject.SetActive(false);
                }
            }
            
            // Initialize ownership
            if (_ownershipIndicator != null)
            {
                _ownershipIndicator.color = _unownedColor;
                _ownershipIndicator.gameObject.SetActive(false);
            }
            
            // Initialize houses
            if (_housesContainer != null)
                _housesContainer.SetActive(false);
            
            // Initialize mortgage overlay
            if (_mortgageOverlay != null)
                _mortgageOverlay.SetActive(false);
        }
        
        /// <summary>
        /// Updates the ownership indicator.
        /// </summary>
        /// <param name="ownerId">The ID of the owner, or null if unowned.</param>
        public void UpdateOwnership(string ownerId)
        {
            _currentOwnerId = ownerId;
            
            if (_ownershipIndicator != null)
            {
                if (!string.IsNullOrEmpty(ownerId))
                {
                    _ownershipIndicator.gameObject.SetActive(true);
                    _ownershipIndicator.color = GetPlayerColor(ownerId);
                }
                else
                {
                    _ownershipIndicator.gameObject.SetActive(false);
                }
            }
        }
        
        /// <summary>
        /// Updates the house/hotel display.
        /// </summary>
        /// <param name="houseCount">Number of houses (0-4).</param>
        /// <param name="hasHotel">Whether the property has a hotel.</param>
        public void UpdateHouses(int houseCount, bool hasHotel)
        {
            if (_housesContainer != null)
            {
                bool hasBuildings = houseCount > 0 || hasHotel;
                _housesContainer.SetActive(hasBuildings);
                
                if (_houseIcons != null && hasBuildings)
                {
                    for (int i = 0; i < _houseIcons.Length; i++)
                    {
                        if (_houseIcons[i] != null)
                        {
                            if (hasHotel)
                            {
                                // Show hotel icon on first icon, hide others
                                _houseIcons[i].gameObject.SetActive(i == 0);
                                // TODO: Change sprite to hotel icon
                            }
                            else
                            {
                                // Show house icons up to houseCount
                                _houseIcons[i].gameObject.SetActive(i < houseCount);
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Updates the mortgage status display.
        /// </summary>
        /// <param name="isMortgaged">Whether the property is mortgaged.</param>
        public void UpdateMortgageStatus(bool isMortgaged)
        {
            _isMortgaged = isMortgaged;
            
            if (_mortgageOverlay != null)
                _mortgageOverlay.SetActive(isMortgaged);
            
            if (_ownershipIndicator != null && isMortgaged)
                _ownershipIndicator.color = _mortgagedColor;
            else if (_ownershipIndicator != null && !string.IsNullOrEmpty(_currentOwnerId))
                _ownershipIndicator.color = GetPlayerColor(_currentOwnerId);
        }
        
        /// <summary>
        /// Handles space button click.
        /// </summary>
        private void HandleSpaceClicked()
        {
            OnSpaceClicked?.Invoke(this);
        }
        
        /// <summary>
        /// Gets the color for a property color group.
        /// </summary>
        /// <param name="colorGroup">The color group name.</param>
        /// <returns>The color to use.</returns>
        private Color GetColorGroupColor(string colorGroup)
        {
            // Standard Monopoly color groups
            return colorGroup switch
            {
                "Brown" => new Color(0.55f, 0.27f, 0.07f), // Brown
                "Light Blue" => new Color(0.68f, 0.85f, 0.90f), // Light Blue
                "Pink" => new Color(1.0f, 0.41f, 0.71f), // Pink
                "Orange" => new Color(1.0f, 0.55f, 0.0f), // Orange
                "Red" => new Color(0.93f, 0.11f, 0.14f), // Red
                "Yellow" => new Color(1.0f, 0.92f, 0.23f), // Yellow
                "Green" => new Color(0.0f, 0.62f, 0.38f), // Green
                "Dark Blue" => new Color(0.0f, 0.32f, 0.73f), // Dark Blue
                _ => Color.white
            };
        }
        
        /// <summary>
        /// Gets the color for a player ID.
        /// </summary>
        /// <param name="playerId">The player ID.</param>
        /// <returns>The color to use for the player.</returns>
        private Color GetPlayerColor(string playerId)
        {
            // Simple hash-based color generation
            // In production, use a predefined color palette per player
            int hash = playerId.GetHashCode();
            float hue = (hash % 360) / 360f;
            return Color.HSVToRGB(hue, 0.7f, 0.9f);
        }
    }
}
