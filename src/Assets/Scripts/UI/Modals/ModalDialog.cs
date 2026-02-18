using UnityEngine;
using UnityEngine.UI;

namespace MonopolyFrenzy.UI.Modals
{
    /// <summary>
    /// Base class for modal dialogs.
    /// Provides common functionality like overlay, show/hide, and dismiss on background click.
    /// </summary>
    public abstract class ModalDialog : MonoBehaviour
    {
        [Header("Modal Settings")]
        [SerializeField] protected Image _overlayImage;
        [SerializeField] protected GameObject _modalContent;
        [SerializeField] protected Button _closeButton;
        [SerializeField] protected bool _dismissOnBackgroundClick = true;
        
        [Header("Animation")]
        [SerializeField] protected float _fadeInDuration = 0.2f;
        [SerializeField] protected float _fadeOutDuration = 0.15f;
        
        /// <summary>
        /// Event fired when the modal is closed.
        /// </summary>
        public event System.Action OnClosed;
        
        protected bool _isShowing = false;
        protected CanvasGroup _canvasGroup;
        
        protected virtual void Awake()
        {
            // Get or add canvas group
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            
            // Set up close button
            if (_closeButton != null)
                _closeButton.onClick.AddListener(Hide);
            
            // Set up background click dismiss
            if (_overlayImage != null && _dismissOnBackgroundClick)
            {
                Button overlayButton = _overlayImage.GetComponent<Button>();
                if (overlayButton == null)
                    overlayButton = _overlayImage.gameObject.AddComponent<Button>();
                overlayButton.onClick.AddListener(OnOverlayClicked);
            }
            
            // Hide initially
            gameObject.SetActive(false);
        }
        
        protected virtual void OnDestroy()
        {
            if (_closeButton != null)
                _closeButton.onClick.RemoveListener(Hide);
            
            if (_overlayImage != null && _dismissOnBackgroundClick)
            {
                Button overlayButton = _overlayImage.GetComponent<Button>();
                if (overlayButton != null)
                    overlayButton.onClick.RemoveListener(OnOverlayClicked);
            }
        }
        
        /// <summary>
        /// Shows the modal dialog.
        /// </summary>
        public virtual void Show()
        {
            if (_isShowing)
                return;
            
            _isShowing = true;
            gameObject.SetActive(true);
            
            StartCoroutine(FadeIn());
        }
        
        /// <summary>
        /// Hides the modal dialog.
        /// </summary>
        public virtual void Hide()
        {
            if (!_isShowing)
                return;
            
            _isShowing = false;
            
            StartCoroutine(FadeOut());
        }
        
        /// <summary>
        /// Handles overlay background click.
        /// </summary>
        protected virtual void OnOverlayClicked()
        {
            if (_dismissOnBackgroundClick)
                Hide();
        }
        
        /// <summary>
        /// Fade in animation coroutine.
        /// </summary>
        protected virtual System.Collections.IEnumerator FadeIn()
        {
            if (_canvasGroup == null)
            {
                yield break;
            }
            
            _canvasGroup.alpha = 0f;
            float elapsed = 0f;
            
            while (elapsed < _fadeInDuration)
            {
                elapsed += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / _fadeInDuration);
                yield return null;
            }
            
            _canvasGroup.alpha = 1f;
        }
        
        /// <summary>
        /// Fade out animation coroutine.
        /// </summary>
        protected virtual System.Collections.IEnumerator FadeOut()
        {
            if (_canvasGroup == null)
            {
                gameObject.SetActive(false);
                OnClosed?.Invoke();
                yield break;
            }
            
            _canvasGroup.alpha = 1f;
            float elapsed = 0f;
            
            while (elapsed < _fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / _fadeOutDuration);
                yield return null;
            }
            
            _canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
            
            OnClosed?.Invoke();
        }
    }
}
