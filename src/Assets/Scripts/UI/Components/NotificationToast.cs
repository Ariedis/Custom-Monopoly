using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MonopolyFrenzy.UI.Components
{
    /// <summary>
    /// Simple notification/toast message component.
    /// Shows brief messages that auto-dismiss.
    /// </summary>
    public class NotificationToast : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private Image _backgroundImage;
        
        [Header("Settings")]
        [SerializeField] private float _displayDuration = 3f;
        [SerializeField] private float _fadeInDuration = 0.3f;
        [SerializeField] private float _fadeOutDuration = 0.3f;
        
        [Header("Colors")]
        [SerializeField] private Color _infoColor = new Color(0.2f, 0.5f, 0.8f, 0.9f);
        [SerializeField] private Color _successColor = new Color(0.2f, 0.8f, 0.3f, 0.9f);
        [SerializeField] private Color _warningColor = new Color(0.9f, 0.6f, 0.2f, 0.9f);
        [SerializeField] private Color _errorColor = new Color(0.9f, 0.2f, 0.2f, 0.9f);
        
        private CanvasGroup _canvasGroup;
        private Coroutine _displayCoroutine;
        
        /// <summary>
        /// Notification type.
        /// </summary>
        public enum NotificationType
        {
            Info,
            Success,
            Warning,
            Error
        }
        
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Shows a notification message.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="type">The notification type.</param>
        /// <param name="duration">Override display duration (optional).</param>
        public void Show(string message, NotificationType type = NotificationType.Info, float? duration = null)
        {
            if (_displayCoroutine != null)
                StopCoroutine(_displayCoroutine);
            
            float actualDuration = duration ?? _displayDuration;
            _displayCoroutine = StartCoroutine(DisplayCoroutine(message, type, actualDuration));
        }
        
        /// <summary>
        /// Display coroutine with fade in, hold, and fade out.
        /// </summary>
        private IEnumerator DisplayCoroutine(string message, NotificationType type, float duration)
        {
            // Set message and color
            if (_messageText != null)
                _messageText.text = message;
            
            if (_backgroundImage != null)
            {
                _backgroundImage.color = type switch
                {
                    NotificationType.Success => _successColor,
                    NotificationType.Warning => _warningColor,
                    NotificationType.Error => _errorColor,
                    _ => _infoColor
                };
            }
            
            gameObject.SetActive(true);
            
            // Fade in
            yield return FadeToAlpha(1f, _fadeInDuration);
            
            // Hold
            yield return new WaitForSeconds(duration);
            
            // Fade out
            yield return FadeToAlpha(0f, _fadeOutDuration);
            
            gameObject.SetActive(false);
            _displayCoroutine = null;
        }
        
        /// <summary>
        /// Fades the canvas group to a target alpha.
        /// </summary>
        private IEnumerator FadeToAlpha(float targetAlpha, float duration)
        {
            if (_canvasGroup == null)
                yield break;
            
            float startAlpha = _canvasGroup.alpha;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                yield return null;
            }
            
            _canvasGroup.alpha = targetAlpha;
        }
    }
}
