using System;
using System.Collections.Generic;

namespace MonopolyFrenzy.Events
{
    /// <summary>
    /// Interface for the event bus system.
    /// Provides publish/subscribe functionality for game events.
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Subscribes a handler to events of type T.
        /// </summary>
        /// <typeparam name="T">The event type to subscribe to.</typeparam>
        /// <param name="handler">The handler to call when the event is published.</param>
        void Subscribe<T>(Action<T> handler) where T : class;
        
        /// <summary>
        /// Unsubscribes a handler from events of type T.
        /// </summary>
        /// <typeparam name="T">The event type to unsubscribe from.</typeparam>
        /// <param name="handler">The handler to remove.</param>
        void Unsubscribe<T>(Action<T> handler) where T : class;
        
        /// <summary>
        /// Publishes an event to all subscribed handlers.
        /// </summary>
        /// <typeparam name="T">The event type.</typeparam>
        /// <param name="eventData">The event data to publish.</param>
        void Publish<T>(T eventData) where T : class;
    }
    
    /// <summary>
    /// Event bus implementation for loosely coupled event handling.
    /// Thread-safe implementation with error isolation between handlers.
    /// </summary>
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _subscribers;
        private readonly object _lockObject = new object();
        
        /// <summary>
        /// Initializes a new instance of the EventBus class.
        /// </summary>
        public EventBus()
        {
            _subscribers = new Dictionary<Type, List<Delegate>>();
        }
        
        /// <summary>
        /// Subscribes a handler to events of type T.
        /// </summary>
        /// <typeparam name="T">The event type to subscribe to.</typeparam>
        /// <param name="handler">The handler to call when the event is published.</param>
        /// <exception cref="ArgumentNullException">Thrown when handler is null.</exception>
        public void Subscribe<T>(Action<T> handler) where T : class
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            
            lock (_lockObject)
            {
                var eventType = typeof(T);
                
                if (!_subscribers.ContainsKey(eventType))
                {
                    _subscribers[eventType] = new List<Delegate>();
                }
                
                _subscribers[eventType].Add(handler);
            }
        }
        
        /// <summary>
        /// Unsubscribes a handler from events of type T.
        /// </summary>
        /// <typeparam name="T">The event type to unsubscribe from.</typeparam>
        /// <param name="handler">The handler to remove.</param>
        public void Unsubscribe<T>(Action<T> handler) where T : class
        {
            if (handler == null)
                return;
            
            lock (_lockObject)
            {
                var eventType = typeof(T);
                
                if (_subscribers.ContainsKey(eventType))
                {
                    _subscribers[eventType].Remove(handler);
                    
                    // Clean up empty lists
                    if (_subscribers[eventType].Count == 0)
                    {
                        _subscribers.Remove(eventType);
                    }
                }
            }
        }
        
        /// <summary>
        /// Publishes an event to all subscribed handlers.
        /// Errors in individual handlers are caught and logged to prevent cascade failures.
        /// </summary>
        /// <typeparam name="T">The event type.</typeparam>
        /// <param name="eventData">The event data to publish.</param>
        public void Publish<T>(T eventData) where T : class
        {
            if (eventData == null)
                return;
            
            List<Delegate> handlersToInvoke = null;
            
            lock (_lockObject)
            {
                var eventType = typeof(T);
                
                if (_subscribers.ContainsKey(eventType))
                {
                    // Create a copy to avoid issues with concurrent modification
                    handlersToInvoke = new List<Delegate>(_subscribers[eventType]);
                }
            }
            
            // Invoke handlers outside of lock for better performance and to avoid deadlocks
            if (handlersToInvoke != null)
            {
                foreach (var handler in handlersToInvoke)
                {
                    try
                    {
                        ((Action<T>)handler).Invoke(eventData);
                    }
                    catch (Exception ex)
                    {
                        // Log error but continue with other handlers
                        // In a real implementation, this would use a proper logging system
                        System.Diagnostics.Debug.WriteLine($"Error in event handler: {ex.Message}");
                    }
                }
            }
        }
    }
}
