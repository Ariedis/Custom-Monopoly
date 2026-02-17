using System;
using MonopolyFrenzy.Core;
using MonopolyFrenzy.Rules;
using Newtonsoft.Json;

namespace MonopolyFrenzy.Commands
{
    /// <summary>
    /// Command to build a house on a property.
    /// </summary>
    public class BuyHouseCommand : ICommand
    {
        private readonly GameState _gameState;
        private readonly Player _player;
        private readonly Property _property;
        private int _previousHouses;
        private bool _previousHadHotel;
        
        /// <summary>
        /// Gets whether a hotel was built (true) or a house (false).
        /// </summary>
        public bool IsHotel { get; private set; }
        
        /// <summary>
        /// Initializes a new instance of the BuyHouseCommand class.
        /// </summary>
        /// <param name="gameState">The current game state.</param>
        /// <param name="player">The player buying the house.</param>
        /// <param name="property">The property to build on.</param>
        public BuyHouseCommand(GameState gameState, Player player, Property property)
        {
            _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _property = property ?? throw new ArgumentNullException(nameof(property));
        }
        
        public CommandResult Execute()
        {
            try
            {
                var rules = new PropertyRules(_gameState);
                
                // Store previous state
                _previousHouses = _property.Houses;
                _previousHadHotel = _property.HasHotel;
                
                // Check if building hotel or house
                if (_property.Houses == 4 && !_property.HasHotel)
                {
                    // Building hotel
                    var (canBuild, error) = rules.CanBuildHotel(_player, _property);
                    
                    if (!canBuild)
                        return CommandResult.Failed(error);
                    
                    IsHotel = true;
                    _property.HasHotel = true;
                    _property.Houses = 0; // Remove houses when hotel is built
                    _player.RemoveMoney(_property.HouseCost);
                    
                    return CommandResult.Successful(new
                    {
                        PropertyName = _property.Name,
                        IsHotel = true,
                        Cost = _property.HouseCost,
                        PlayerMoney = _player.Money
                    });
                }
                else
                {
                    // Building house
                    var (canBuild, error) = rules.CanBuildHouse(_player, _property);
                    
                    if (!canBuild)
                        return CommandResult.Failed(error);
                    
                    IsHotel = false;
                    _property.Houses++;
                    _player.RemoveMoney(_property.HouseCost);
                    
                    return CommandResult.Successful(new
                    {
                        PropertyName = _property.Name,
                        IsHotel = false,
                        HouseCount = _property.Houses,
                        Cost = _property.HouseCost,
                        PlayerMoney = _player.Money
                    });
                }
            }
            catch (Exception ex)
            {
                return CommandResult.Failed($"Failed to build house/hotel: {ex.Message}");
            }
        }
        
        public void Undo()
        {
            if (_property == null)
                return;
            
            // Restore previous state
            _property.Houses = _previousHouses;
            _property.HasHotel = _previousHadHotel;
            _player.AddMoney(_property.HouseCost);
        }
        
        public string SerializeToJson()
        {
            return JsonConvert.SerializeObject(new
            {
                Type = "BuyHouse",
                PlayerId = _player.Id,
                PropertyIndex = _property.Index,
                PropertyName = _property.Name,
                IsHotel = IsHotel,
                HouseCount = _property.Houses,
                Cost = _property.HouseCost
            });
        }
    }
}
