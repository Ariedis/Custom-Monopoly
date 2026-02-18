using UnityEngine;
using UnityEngine.UI;
using MonopolyFrenzy.UI.Controllers;
using MonopolyFrenzy.UI.Modals;

namespace MonopolyFrenzy.UI.Panels
{
    /// <summary>
    /// Pause menu panel with game options.
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject _pauseMenuPanel;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _saveGameButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private Button _quitButton;
        
        [Header("Modals")]
        [SerializeField] private ConfirmationModal _confirmationModal;
        
        private bool _isPaused = false;
        
        private void Awake()
        {
            ValidateReferences();
        }
        
        private void Start()
        {
            SetupButtonListeners();
            Hide();
        }
        
        private void Update()
        {
            // Check for Escape key to toggle pause
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_isPaused)
                    Resume();
                else
                    Show();
            }
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
            if (_pauseMenuPanel == null)
                Debug.LogError("Pause Menu Panel is not assigned in PauseMenu!", this);
            
            if (_resumeButton == null)
                Debug.LogError("Resume Button is not assigned in PauseMenu!", this);
            
            if (_confirmationModal == null)
                Debug.LogWarning("Confirmation Modal is not assigned in PauseMenu!", this);
        }
        
        /// <summary>
        /// Sets up button click listeners.
        /// </summary>
        private void SetupButtonListeners()
        {
            if (_resumeButton != null)
                _resumeButton.onClick.AddListener(OnResumeClicked);
            
            if (_saveGameButton != null)
                _saveGameButton.onClick.AddListener(OnSaveGameClicked);
            
            if (_settingsButton != null)
                _settingsButton.onClick.AddListener(OnSettingsClicked);
            
            if (_mainMenuButton != null)
                _mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            
            if (_quitButton != null)
                _quitButton.onClick.AddListener(OnQuitClicked);
        }
        
        /// <summary>
        /// Removes button click listeners.
        /// </summary>
        private void RemoveButtonListeners()
        {
            if (_resumeButton != null)
                _resumeButton.onClick.RemoveListener(OnResumeClicked);
            
            if (_saveGameButton != null)
                _saveGameButton.onClick.RemoveListener(OnSaveGameClicked);
            
            if (_settingsButton != null)
                _settingsButton.onClick.RemoveListener(OnSettingsClicked);
            
            if (_mainMenuButton != null)
                _mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
            
            if (_quitButton != null)
                _quitButton.onClick.RemoveListener(OnQuitClicked);
        }
        
        /// <summary>
        /// Shows the pause menu.
        /// </summary>
        public void Show()
        {
            if (_isPaused)
                return;
            
            _isPaused = true;
            
            if (_pauseMenuPanel != null)
                _pauseMenuPanel.SetActive(true);
            
            // Pause game (freeze time)
            Time.timeScale = 0f;
        }
        
        /// <summary>
        /// Hides the pause menu.
        /// </summary>
        public void Hide()
        {
            if (!_isPaused)
                return;
            
            _isPaused = false;
            
            if (_pauseMenuPanel != null)
                _pauseMenuPanel.SetActive(false);
            
            // Resume game
            Time.timeScale = 1f;
        }
        
        /// <summary>
        /// Resumes the game.
        /// </summary>
        public void Resume()
        {
            Hide();
        }
        
        #region Button Handlers
        
        /// <summary>
        /// Handles Resume button click.
        /// </summary>
        private void OnResumeClicked()
        {
            Resume();
        }
        
        /// <summary>
        /// Handles Save Game button click.
        /// </summary>
        private void OnSaveGameClicked()
        {
            Debug.Log("Save Game clicked");
            // TODO: Implement save game functionality (Week 8)
            // For now, just show a placeholder message
            if (_confirmationModal != null)
            {
                _confirmationModal.ShowConfirmation(
                    "Save Game",
                    "Save game functionality will be implemented in Week 8.",
                    "OK",
                    "Cancel"
                );
            }
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
        /// Handles Main Menu button click.
        /// </summary>
        private void OnMainMenuClicked()
        {
            if (_confirmationModal != null)
            {
                _confirmationModal.ShowConfirmation(
                    "Return to Main Menu",
                    "Are you sure? Current game progress will be lost if not saved.",
                    "Yes",
                    "No"
                );
                
                _confirmationModal.OnConfirmed += () =>
                {
                    // Resume time before scene change
                    Time.timeScale = 1f;
                    UIController.Instance.LoadScene(SceneNames.MainMenu);
                };
            }
            else
            {
                // No confirmation modal, just go to main menu
                Time.timeScale = 1f;
                UIController.Instance.LoadScene(SceneNames.MainMenu);
            }
        }
        
        /// <summary>
        /// Handles Quit button click.
        /// </summary>
        private void OnQuitClicked()
        {
            if (_confirmationModal != null)
            {
                _confirmationModal.ShowConfirmation(
                    "Quit Game",
                    "Are you sure you want to quit? Current game progress will be lost if not saved.",
                    "Yes, Quit",
                    "Cancel"
                );
                
                _confirmationModal.OnConfirmed += () =>
                {
                    Time.timeScale = 1f;
                    UIController.Instance.QuitApplication();
                };
            }
            else
            {
                Time.timeScale = 1f;
                UIController.Instance.QuitApplication();
            }
        }
        
        #endregion
    }
}
