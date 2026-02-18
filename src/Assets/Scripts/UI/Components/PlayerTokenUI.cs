using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MonopolyFrenzy.Core;
using MonopolyFrenzy.Events;
using System.Collections;

namespace MonopolyFrenzy.UI.Components
{
    /// <summary>
    /// UI component representing a player's token on the board.
    /// Handles token positioning and visual state.
    /// </summary>
    public class PlayerTokenUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image _tokenImage;
        [SerializeField] private TextMeshProUGUI _playerInitialsText;
        [SerializeField] private Image _highlightBorder;
        
        [Header("Animation Settings")]
        [SerializeField] private float _moveDuration = 0.3f;
        [SerializeField] private float _highlightDuration = 0.5f;
        
        private Player _player;
        private int _currentPosition;
        private bool _isCurrentPlayer;
        private Coroutine _moveCoroutine;
        private Coroutine _highlightCoroutine;
        
        /// <summary>
        /// Gets the player associated with this token.
        /// </summary>
        public Player Player => _player;
        
        /// <summary>
        /// Gets the current board position of this token.
        /// </summary>
        public int CurrentPosition => _currentPosition;
        
        /// <summary>
        /// Initializes the player token.
        /// </summary>
        /// <param name="player">The player data.</param>
        /// <param name="tokenSprite">The sprite to use for the token (optional).</param>
        public void Initialize(Player player, Sprite tokenSprite = null)
        {
            _player = player;
            _currentPosition = player.Position;
            
            // Set player initials
            if (_playerInitialsText != null && !string.IsNullOrEmpty(player.Name))
            {
                string initials = GetInitials(player.Name);
                _playerInitialsText.text = initials;
            }
            
            // Set token sprite if provided
            if (_tokenImage != null && tokenSprite != null)
            {
                _tokenImage.sprite = tokenSprite;
            }
            
            // Set player color
            if (_tokenImage != null)
            {
                _tokenImage.color = GetPlayerColor(player.Id);
            }
            
            // Hide highlight initially
            if (_highlightBorder != null)
                _highlightBorder.gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Moves the token to a new position.
        /// </summary>
        /// <param name="newPosition">The new board position (0-39).</param>
        /// <param name="worldPosition">The world position to move to.</param>
        /// <param name="animate">Whether to animate the movement.</param>
        public void MoveTo(int newPosition, Vector3 worldPosition, bool animate = true)
        {
            _currentPosition = newPosition;
            
            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);
            
            if (animate)
            {
                _moveCoroutine = StartCoroutine(MoveToCoroutine(worldPosition));
            }
            else
            {
                transform.position = worldPosition;
            }
        }
        
        /// <summary>
        /// Sets whether this token represents the current player.
        /// </summary>
        /// <param name="isCurrent">True if this is the current player's token.</param>
        public void SetCurrentPlayer(bool isCurrent)
        {
            _isCurrentPlayer = isCurrent;
            
            if (_highlightBorder != null)
            {
                _highlightBorder.gameObject.SetActive(isCurrent);
            }
            
            // Make current player's token slightly larger
            if (isCurrent)
            {
                transform.localScale = Vector3.one * 1.2f;
            }
            else
            {
                transform.localScale = Vector3.one;
            }
        }
        
        /// <summary>
        /// Briefly highlights the token.
        /// </summary>
        public void Highlight()
        {
            if (_highlightCoroutine != null)
                StopCoroutine(_highlightCoroutine);
            
            _highlightCoroutine = StartCoroutine(HighlightCoroutine());
        }
        
        /// <summary>
        /// Coroutine for smooth movement animation.
        /// </summary>
        private IEnumerator MoveToCoroutine(Vector3 targetPosition)
        {
            Vector3 startPosition = transform.position;
            float elapsed = 0f;
            
            while (elapsed < _moveDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _moveDuration;
                
                // Smooth ease-in-out
                t = t * t * (3f - 2f * t);
                
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                yield return null;
            }
            
            transform.position = targetPosition;
            _moveCoroutine = null;
            
            // Brief highlight after movement
            Highlight();
        }
        
        /// <summary>
        /// Coroutine for brief highlight effect.
        /// </summary>
        private IEnumerator HighlightCoroutine()
        {
            if (_highlightBorder == null)
                yield break;
            
            _highlightBorder.gameObject.SetActive(true);
            Color originalColor = _highlightBorder.color;
            
            // Fade in
            float elapsed = 0f;
            while (elapsed < _highlightDuration / 2)
            {
                elapsed += Time.deltaTime;
                float alpha = elapsed / (_highlightDuration / 2);
                _highlightBorder.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }
            
            // Fade out
            elapsed = 0f;
            while (elapsed < _highlightDuration / 2)
            {
                elapsed += Time.deltaTime;
                float alpha = 1f - (elapsed / (_highlightDuration / 2));
                _highlightBorder.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }
            
            // Restore to current player state
            if (!_isCurrentPlayer)
                _highlightBorder.gameObject.SetActive(false);
            
            _highlightCoroutine = null;
        }
        
        /// <summary>
        /// Gets initials from a player name.
        /// </summary>
        /// <param name="name">The player name.</param>
        /// <returns>1-2 character initials.</returns>
        private string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "?";
            
            string[] parts = name.Trim().Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length == 0)
                return "?";
            
            if (parts.Length == 1)
                return parts[0].Substring(0, Mathf.Min(2, parts[0].Length)).ToUpper();
            
            // Use first letter of first and last name
            return $"{parts[0][0]}{parts[parts.Length - 1][0]}".ToUpper();
        }
        
        /// <summary>
        /// Gets a color for a player ID.
        /// </summary>
        /// <param name="playerId">The player ID.</param>
        /// <returns>The color to use.</returns>
        private Color GetPlayerColor(string playerId)
        {
            // Simple hash-based color generation
            int hash = playerId.GetHashCode();
            float hue = (hash % 360) / 360f;
            return Color.HSVToRGB(hue, 0.7f, 0.9f);
        }
    }
}
