using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MonopolyFrenzy.UI.Modals
{
    /// <summary>
    /// Generic confirmation modal dialog.
    /// Used for yes/no decisions throughout the application.
    /// </summary>
    public class ConfirmationModal : ModalDialog
    {
        [Header("Content")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _messageText;
        
        [Header("Buttons")]
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private TextMeshProUGUI _confirmButtonText;
        [SerializeField] private TextMeshProUGUI _cancelButtonText;
        
        /// <summary>
        /// Event fired when the user confirms.
        /// </summary>
        public event System.Action OnConfirmed;
        
        /// <summary>
        /// Event fired when the user cancels.
        /// </summary>
        public event System.Action OnCancelled;
        
        protected override void Awake()
        {
            base.Awake();
            
            if (_confirmButton != null)
                _confirmButton.onClick.AddListener(OnConfirmClicked);
            
            if (_cancelButton != null)
                _cancelButton.onClick.AddListener(OnCancelClicked);
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if (_confirmButton != null)
                _confirmButton.onClick.RemoveListener(OnConfirmClicked);
            
            if (_cancelButton != null)
                _cancelButton.onClick.RemoveListener(OnCancelClicked);
        }
        
        /// <summary>
        /// Shows the confirmation modal with specified content.
        /// </summary>
        /// <param name="title">The title text.</param>
        /// <param name="message">The message text.</param>
        /// <param name="confirmText">Text for the confirm button.</param>
        /// <param name="cancelText">Text for the cancel button.</param>
        public void ShowConfirmation(string title, string message, string confirmText = "Confirm", string cancelText = "Cancel")
        {
            if (_titleText != null)
                _titleText.text = title;
            
            if (_messageText != null)
                _messageText.text = message;
            
            if (_confirmButtonText != null)
                _confirmButtonText.text = confirmText;
            
            if (_cancelButtonText != null)
                _cancelButtonText.text = cancelText;
            
            Show();
        }
        
        /// <summary>
        /// Handles confirm button click.
        /// </summary>
        private void OnConfirmClicked()
        {
            OnConfirmed?.Invoke();
            Hide();
        }
        
        /// <summary>
        /// Handles cancel button click.
        /// </summary>
        private void OnCancelClicked()
        {
            OnCancelled?.Invoke();
            Hide();
        }
    }
}
