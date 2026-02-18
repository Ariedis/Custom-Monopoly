using System.Collections.Generic;
using UnityEngine;
using MonopolyFrenzy.Core;
using MonopolyFrenzy.Events;
using MonopolyFrenzy.UI.Components;

namespace MonopolyFrenzy.UI.Controllers
{
    /// <summary>
    /// Controls the game board visualization and manages property spaces.
    /// Handles synchronization between game state and board UI.
    /// </summary>
    public class BoardController : MonoBehaviour
    {
        [Header("Board Settings")]
        [SerializeField] private Transform _boardContainer;
        [SerializeField] private GameObject _propertySpacePrefab;
        
        private GameState _gameState;
        private EventBus _eventBus;
        private Dictionary<int, PropertySpaceUI> _propertySpaces = new Dictionary<int, PropertySpaceUI>();
        
        /// <summary>
        /// Initializes the board controller.
        /// </summary>
        /// <param name="gameState">The game state.</param>
        /// <param name="eventBus">The event bus.</param>
        public void Initialize(GameState gameState, EventBus eventBus)
        {
            _gameState = gameState;
            _eventBus = eventBus;
            
            CreateBoardSpaces();
            SubscribeToEvents();
        }
        
        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
        
        /// <summary>
        /// Creates all 40 board space UI elements.
        /// </summary>
        private void CreateBoardSpaces()
        {
            if (_gameState == null || _gameState.Board == null)
            {
                Debug.LogError("Game state or board not initialized!");
                return;
            }
            
            if (_propertySpacePrefab == null || _boardContainer == null)
            {
                Debug.LogError("Board prefab or container not assigned!");
                return;
            }
            
            // Create UI for each of the 40 spaces
            for (int i = 0; i < 40; i++)
            {
                Space space = _gameState.Board.GetSpace(i);
                if (space != null)
                {
                    GameObject spaceObj = Instantiate(_propertySpacePrefab, _boardContainer);
                    PropertySpaceUI spaceUI = spaceObj.GetComponent<PropertySpaceUI>();
                    
                    if (spaceUI != null)
                    {
                        spaceUI.Initialize(space, i);
                        _propertySpaces[i] = spaceUI;
                    }
                }
            }
            
            Debug.Log($"Created {_propertySpaces.Count} board spaces");
        }
        
        /// <summary>
        /// Subscribes to game events.
        /// </summary>
        private void SubscribeToEvents()
        {
            if (_eventBus != null)
            {
                _eventBus.Subscribe<PropertyPurchasedEvent>(OnPropertyPurchased);
                _eventBus.Subscribe<HousePurchasedEvent>(OnHousePurchased);
                _eventBus.Subscribe<PropertyMortgagedEvent>(OnPropertyMortgaged);
                _eventBus.Subscribe<PropertyUnmortgagedEvent>(OnPropertyUnmortgaged);
            }
        }
        
        /// <summary>
        /// Unsubscribes from game events.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<PropertyPurchasedEvent>(OnPropertyPurchased);
                _eventBus.Unsubscribe<HousePurchasedEvent>(OnHousePurchased);
                _eventBus.Unsubscribe<PropertyMortgagedEvent>(OnPropertyMortgaged);
                _eventBus.Unsubscribe<PropertyUnmortgagedEvent>(OnPropertyUnmortgaged);
            }
        }
        
        /// <summary>
        /// Gets the UI position for a space index.
        /// </summary>
        /// <param name="spaceIndex">The space index (0-39).</param>
        /// <returns>The world position for the space.</returns>
        public Vector3 GetSpacePosition(int spaceIndex)
        {
            if (_propertySpaces.TryGetValue(spaceIndex, out PropertySpaceUI spaceUI))
            {
                return spaceUI.transform.position;
            }
            
            return Vector3.zero;
        }
        
        #region Event Handlers
        
        /// <summary>
        /// Handles property purchased event.
        /// </summary>
        private void OnPropertyPurchased(PropertyPurchasedEvent evt)
        {
            int spaceIndex = FindSpaceIndex(evt.PropertyName);
            if (spaceIndex >= 0 && _propertySpaces.TryGetValue(spaceIndex, out PropertySpaceUI spaceUI))
            {
                spaceUI.UpdateOwnership(evt.PlayerId);
            }
        }
        
        /// <summary>
        /// Handles house purchased event.
        /// </summary>
        private void OnHousePurchased(HousePurchasedEvent evt)
        {
            int spaceIndex = FindSpaceIndex(evt.PropertyName);
            if (spaceIndex >= 0 && _propertySpaces.TryGetValue(spaceIndex, out PropertySpaceUI spaceUI))
            {
                spaceUI.UpdateHouses(evt.HouseCount, evt.IsHotel);
            }
        }
        
        /// <summary>
        /// Handles property mortgaged event.
        /// </summary>
        private void OnPropertyMortgaged(PropertyMortgagedEvent evt)
        {
            int spaceIndex = FindSpaceIndex(evt.PropertyName);
            if (spaceIndex >= 0 && _propertySpaces.TryGetValue(spaceIndex, out PropertySpaceUI spaceUI))
            {
                spaceUI.UpdateMortgageStatus(true);
            }
        }
        
        /// <summary>
        /// Handles property unmortgaged event.
        /// </summary>
        private void OnPropertyUnmortgaged(PropertyUnmortgagedEvent evt)
        {
            int spaceIndex = FindSpaceIndex(evt.PropertyName);
            if (spaceIndex >= 0 && _propertySpaces.TryGetValue(spaceIndex, out PropertySpaceUI spaceUI))
            {
                spaceUI.UpdateMortgageStatus(false);
            }
        }
        
        #endregion
        
        /// <summary>
        /// Finds the space index for a property name.
        /// </summary>
        /// <param name="propertyName">The property name to find.</param>
        /// <returns>The space index, or -1 if not found.</returns>
        private int FindSpaceIndex(string propertyName)
        {
            if (_gameState == null || _gameState.Board == null)
                return -1;
            
            for (int i = 0; i < 40; i++)
            {
                Space space = _gameState.Board.GetSpace(i);
                if (space != null && space.Name == propertyName)
                    return i;
            }
            
            return -1;
        }
    }
}
