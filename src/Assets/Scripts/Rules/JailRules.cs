using System;
using MonopolyFrenzy.Core;
using MonopolyFrenzy.Events;

namespace MonopolyFrenzy.Rules
{
    /// <summary>
    /// Handles jail-related game rules and mechanics.
    /// </summary>
    public class JailRules
    {
        private readonly GameState _gameState;
        private readonly EventBus _eventBus;
        
        /// <summary>
        /// The position of the Jail space on the board.
        /// </summary>
        public const int JailPosition = 10;
        
        /// <summary>
        /// The cost to pay to get out of jail.
        /// </summary>
        public const int JailPaymentAmount = 50;
        
        /// <summary>
        /// The maximum number of turns a player can stay in jail.
        /// </summary>
        public const int MaxJailTurns = 3;
        
        /// <summary>
        /// Initializes a new instance of the JailRules class.
        /// </summary>
        /// <param name="gameState">The current game state.</param>
        /// <param name="eventBus">The event bus for publishing jail events.</param>
        public JailRules(GameState gameState, EventBus eventBus = null)
        {
            _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
            _eventBus = eventBus;
        }
        
        /// <summary>
        /// Sends a player to jail.
        /// </summary>
        /// <param name="player">The player to jail.</param>
        /// <param name="reason">The reason for jailing (e.g., "Go To Jail space", "Rolled doubles 3 times").</param>
        /// <returns>True if the player was successfully jailed.</returns>
        public bool SendToJail(Player player, string reason = "Go To Jail")
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));
            
            if (player.IsBankrupt)
                return false;
            
            player.IsInJail = true;
            player.JailTurns = 0;
            player.Position = JailPosition;
            
            _eventBus?.Publish(new PlayerJailedEvent
            {
                PlayerId = player.Id,
                Reason = reason
            });
            
            return true;
        }
        
        /// <summary>
        /// Attempts to get out of jail by rolling doubles.
        /// </summary>
        /// <param name="player">The player attempting to escape.</param>
        /// <param name="die1">First die value.</param>
        /// <param name="die2">Second die value.</param>
        /// <returns>True if the player rolled doubles and escaped jail.</returns>
        public bool TryEscapeByRollingDoubles(Player player, int die1, int die2)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));
            
            if (!player.IsInJail)
                return false;
            
            bool isDoubles = die1 == die2;
            
            if (isDoubles)
            {
                ReleaseFromJail(player, "Rolled doubles");
                return true;
            }
            
            // Increment jail turns
            player.JailTurns++;
            
            // Force release after 3 turns
            if (player.JailTurns >= MaxJailTurns)
            {
                // Must pay to get out
                if (player.Money >= JailPaymentAmount)
                {
                    player.RemoveMoney(JailPaymentAmount);
                    ReleaseFromJail(player, "Forced payment after 3 turns");
                    return true;
                }
                else
                {
                    // Player must liquidate assets or go bankrupt
                    return false;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Attempts to get out of jail by paying $50.
        /// </summary>
        /// <param name="player">The player attempting to pay.</param>
        /// <returns>True if payment was successful and player was released.</returns>
        public bool PayToGetOutOfJail(Player player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));
            
            if (!player.IsInJail)
                return false;
            
            if (player.Money < JailPaymentAmount)
                return false;
            
            player.RemoveMoney(JailPaymentAmount);
            ReleaseFromJail(player, "Paid $50");
            
            return true;
        }
        
        /// <summary>
        /// Uses a "Get Out of Jail Free" card to release the player from jail.
        /// </summary>
        /// <param name="player">The player using the card.</param>
        /// <returns>True if the card was used successfully.</returns>
        public bool UseGetOutOfJailFreeCard(Player player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));
            
            if (!player.IsInJail)
                return false;
            
            if (player.GetOutOfJailFreeCards <= 0)
                return false;
            
            player.GetOutOfJailFreeCards--;
            ReleaseFromJail(player, "Used Get Out of Jail Free card");
            
            return true;
        }
        
        /// <summary>
        /// Releases a player from jail.
        /// </summary>
        /// <param name="player">The player to release.</param>
        /// <param name="method">The method of release (for event notification).</param>
        private void ReleaseFromJail(Player player, string method)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));
            
            player.IsInJail = false;
            player.JailTurns = 0;
            
            _eventBus?.Publish(new PlayerReleasedFromJailEvent
            {
                PlayerId = player.Id,
                Method = method
            });
        }
        
        /// <summary>
        /// Checks if a player can take normal turn actions.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <returns>True if the player can take actions (not in jail or just released).</returns>
        public bool CanTakeNormalActions(Player player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));
            
            return !player.IsInJail;
        }
        
        /// <summary>
        /// Gets the number of turns remaining before forced release.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <returns>Number of turns remaining, or 0 if not in jail.</returns>
        public int GetTurnsUntilForcedRelease(Player player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));
            
            if (!player.IsInJail)
                return 0;
            
            return Math.Max(0, MaxJailTurns - player.JailTurns);
        }
    }
}
