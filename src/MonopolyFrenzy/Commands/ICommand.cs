using System;

namespace MonopolyFrenzy.Commands
{
    /// <summary>
    /// Result of a command execution.
    /// </summary>
    public class CommandResult
    {
        /// <summary>
        /// Gets or sets whether the command executed successfully.
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// Gets or sets the error message if the command failed.
        /// </summary>
        public string ErrorMessage { get; set; }
        
        /// <summary>
        /// Gets or sets additional data returned by the command.
        /// </summary>
        public object Data { get; set; }
        
        /// <summary>
        /// Creates a successful command result.
        /// </summary>
        /// <param name="data">Optional data to include.</param>
        /// <returns>A successful CommandResult.</returns>
        public static CommandResult Successful(object data = null)
        {
            return new CommandResult
            {
                Success = true,
                Data = data
            };
        }
        
        /// <summary>
        /// Creates a failed command result with an error message.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>A failed CommandResult.</returns>
        public static CommandResult Failed(string errorMessage)
        {
            return new CommandResult
            {
                Success = false,
                ErrorMessage = errorMessage
            };
        }
    }
    
    /// <summary>
    /// Interface for all game commands.
    /// Implements the Command Pattern for undo/redo and replay functionality.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns>The result of the command execution.</returns>
        CommandResult Execute();
        
        /// <summary>
        /// Undoes the command, restoring the previous state.
        /// </summary>
        void Undo();
        
        /// <summary>
        /// Serializes the command to JSON for replay or network transmission.
        /// </summary>
        /// <returns>JSON string representation of the command.</returns>
        string SerializeToJson();
    }
}
