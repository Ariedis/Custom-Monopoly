using UnityEngine;
using UnityEngine.UI;
using MonopolyFrenzy.UI.Controllers;

namespace MonopolyFrenzy.UI.Screens
{
    /// <summary>
    /// Main menu screen controller.
    /// Handles navigation to game setup, load game, settings, and quit.
    /// </summary>
    public class MainMenuScreen : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _loadGameButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _quitButton;
        
        private void Awake()
        {
            ValidateReferences();
        }
        
        private void Start()
        {
            SetupButtonListeners();
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
            if (_newGameButton == null)
                Debug.LogError("New Game Button is not assigned in MainMenuScreen!", this);
            
            if (_loadGameButton == null)
                Debug.LogError("Load Game Button is not assigned in MainMenuScreen!", this);
            
            if (_settingsButton == null)
                Debug.LogError("Settings Button is not assigned in MainMenuScreen!", this);
            
            if (_quitButton == null)
                Debug.LogError("Quit Button is not assigned in MainMenuScreen!", this);
        }
        
        /// <summary>
        /// Sets up button click listeners.
        /// </summary>
        private void SetupButtonListeners()
        {
            if (_newGameButton != null)
                _newGameButton.onClick.AddListener(OnNewGameClicked);
            
            if (_loadGameButton != null)
                _loadGameButton.onClick.AddListener(OnLoadGameClicked);
            
            if (_settingsButton != null)
                _settingsButton.onClick.AddListener(OnSettingsClicked);
            
            if (_quitButton != null)
                _quitButton.onClick.AddListener(OnQuitClicked);
        }
        
        /// <summary>
        /// Removes button click listeners to prevent memory leaks.
        /// </summary>
        private void RemoveButtonListeners()
        {
            if (_newGameButton != null)
                _newGameButton.onClick.RemoveListener(OnNewGameClicked);
            
            if (_loadGameButton != null)
                _loadGameButton.onClick.RemoveListener(OnLoadGameClicked);
            
            if (_settingsButton != null)
                _settingsButton.onClick.RemoveListener(OnSettingsClicked);
            
            if (_quitButton != null)
                _quitButton.onClick.RemoveListener(OnQuitClicked);
        }
        
        /// <summary>
        /// Handles New Game button click.
        /// </summary>
        private void OnNewGameClicked()
        {
            Debug.Log("New Game clicked");
            UIController.Instance.LoadScene("GameSetup");
        }
        
        /// <summary>
        /// Handles Load Game button click.
        /// </summary>
        private void OnLoadGameClicked()
        {
            Debug.Log("Load Game clicked");
            // TODO: Implement load game screen (Week 8)
        }
        
        /// <summary>
        /// Handles Settings button click.
        /// </summary>
        private void OnSettingsClicked()
        {
            Debug.Log("Settings clicked");
            // TODO: Implement settings screen (Week 8)
        }
        
        /// <summary>
        /// Handles Quit button click.
        /// </summary>
        private void OnQuitClicked()
        {
            Debug.Log("Quit clicked");
            UIController.Instance.QuitApplication();
        }
    }
}
