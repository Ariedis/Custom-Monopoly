using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MonopolyFrenzy.UI.Controllers;

namespace MonopolyFrenzy.UI.Screens
{
    /// <summary>
    /// Game over screen displayed when the game ends.
    /// Shows winner information and allows returning to main menu.
    /// </summary>
    public class GameOverScreen : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _winnerNameText;
        [SerializeField] private TextMeshProUGUI _winnerNetWorthText;
        [SerializeField] private TextMeshProUGUI _gameDurationText;
        [SerializeField] private TextMeshProUGUI _totalTurnsText;
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private Button _quitButton;
        
        private string _winnerId;
        private string _winnerName;
        private int _winnerNetWorth;
        private int _totalTurns;
        private float _gameDuration;
        
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
            if (_titleText == null)
                Debug.LogWarning("Title Text is not assigned in GameOverScreen!", this);
            
            if (_winnerNameText == null)
                Debug.LogError("Winner Name Text is not assigned in GameOverScreen!", this);
            
            if (_mainMenuButton == null)
                Debug.LogError("Main Menu Button is not assigned in GameOverScreen!", this);
        }
        
        /// <summary>
        /// Sets up button click listeners.
        /// </summary>
        private void SetupButtonListeners()
        {
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
            if (_mainMenuButton != null)
                _mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
            
            if (_quitButton != null)
                _quitButton.onClick.RemoveListener(OnQuitClicked);
        }
        
        /// <summary>
        /// Displays the game over screen with winner information.
        /// </summary>
        /// <param name="winnerId">The ID of the winning player.</param>
        /// <param name="winnerName">The name of the winning player.</param>
        /// <param name="winnerNetWorth">The winner's net worth.</param>
        /// <param name="totalTurns">Total turns played.</param>
        /// <param name="gameDuration">Duration of the game in seconds.</param>
        public void ShowGameOver(string winnerId, string winnerName, int winnerNetWorth, int totalTurns, float gameDuration)
        {
            _winnerId = winnerId;
            _winnerName = winnerName;
            _winnerNetWorth = winnerNetWorth;
            _totalTurns = totalTurns;
            _gameDuration = gameDuration;
            
            UpdateDisplay();
        }
        
        /// <summary>
        /// Updates the display with game over information.
        /// </summary>
        private void UpdateDisplay()
        {
            if (_titleText != null)
                _titleText.text = "GAME OVER!";
            
            if (_winnerNameText != null)
                _winnerNameText.text = $"Winner: {_winnerName}";
            
            if (_winnerNetWorthText != null)
                _winnerNetWorthText.text = $"Net Worth: ${_winnerNetWorth}";
            
            if (_gameDurationText != null)
            {
                int minutes = Mathf.FloorToInt(_gameDuration / 60f);
                int seconds = Mathf.FloorToInt(_gameDuration % 60f);
                _gameDurationText.text = $"Game Duration: {minutes:00}:{seconds:00}";
            }
            
            if (_totalTurnsText != null)
                _totalTurnsText.text = $"Total Turns: {_totalTurns}";
        }
        
        /// <summary>
        /// Handles Main Menu button click.
        /// </summary>
        private void OnMainMenuClicked()
        {
            UIController.Instance.LoadScene(SceneNames.MainMenu);
        }
        
        /// <summary>
        /// Handles Quit button click.
        /// </summary>
        private void OnQuitClicked()
        {
            UIController.Instance.QuitApplication();
        }
    }
}
