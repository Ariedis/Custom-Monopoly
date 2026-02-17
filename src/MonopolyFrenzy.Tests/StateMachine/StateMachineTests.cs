using NUnit.Framework;
using System;

namespace MonopolyFrenzy.Tests.StateMachine
{
    /// <summary>
    /// Test Suite 4: State Machine Tests
    /// Tests for User Story 1.2: State Machine for Game Flow
    /// 
    /// Validates:
    /// - State machine with states: MainMenu, GameSetup, Playing, GameOver
    /// - Turn state machine: RollDice, MovePiece, TakeTurnAction, EndTurn
    /// - State transitions are validated and logged
    /// - Each state has clear Enter(), Update(), Exit() methods
    /// - Invalid state transitions throw exceptions
    /// - State machine is testable without Unity
    /// - State transitions complete in <1ms
    /// </summary>
    [TestFixture]
    public class StateMachineTests
    {
        private GameStateMachine _stateMachine;
        private TestGameContext _gameContext;
        
        [SetUp]
        public void Setup()
        {
            _gameContext = new TestGameContext();
            _stateMachine = new GameStateMachine(_gameContext);
        }
        
        [TearDown]
        public void Teardown()
        {
            _stateMachine = null;
            _gameContext = null;
        }
        
        #region State Machine Initialization Tests
        
        [Test]
        public void Constructor_InitializesWithMainMenuState()
        {
            // Assert
            Assert.IsNotNull(_stateMachine.CurrentState);
            Assert.IsInstanceOf<MainMenuState>(_stateMachine.CurrentState);
        }
        
        [Test]
        public void Initialize_SetsUpAllStates()
        {
            // Act
            _stateMachine.Initialize();
            
            // Assert
            Assert.IsNotNull(_stateMachine.GetState<MainMenuState>());
            Assert.IsNotNull(_stateMachine.GetState<GameSetupState>());
            Assert.IsNotNull(_stateMachine.GetState<PlayingState>());
            Assert.IsNotNull(_stateMachine.GetState<GameOverState>());
        }
        
        #endregion
        
        #region Game Flow State Transitions Tests
        
        [Test]
        public void TransitionTo_FromMainMenuToSetup_SuccessfulTransition()
        {
            // Arrange
            _stateMachine.Initialize();
            bool exitCalled = false;
            bool enterCalled = false;
            _stateMachine.OnStateExit += (state) => exitCalled = true;
            _stateMachine.OnStateEnter += (state) => enterCalled = true;
            
            // Act
            _stateMachine.TransitionTo<GameSetupState>();
            
            // Assert
            Assert.IsInstanceOf<GameSetupState>(_stateMachine.CurrentState);
            Assert.IsTrue(exitCalled, "Exit should be called on previous state");
            Assert.IsTrue(enterCalled, "Enter should be called on new state");
        }
        
        [Test]
        public void TransitionTo_FromSetupToPlaying_SuccessfulTransition()
        {
            // Arrange
            _stateMachine.Initialize();
            _stateMachine.TransitionTo<GameSetupState>();
            
            // Act
            _stateMachine.TransitionTo<PlayingState>();
            
            // Assert
            Assert.IsInstanceOf<PlayingState>(_stateMachine.CurrentState);
        }
        
        [Test]
        public void TransitionTo_FromPlayingToGameOver_SuccessfulTransition()
        {
            // Arrange
            _stateMachine.Initialize();
            _stateMachine.TransitionTo<GameSetupState>();
            _stateMachine.TransitionTo<PlayingState>();
            
            // Act
            _stateMachine.TransitionTo<GameOverState>();
            
            // Assert
            Assert.IsInstanceOf<GameOverState>(_stateMachine.CurrentState);
        }
        
        [Test]
        public void TransitionTo_InvalidTransition_ThrowsInvalidOperationException()
        {
            // Arrange
            _stateMachine.Initialize();
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(
                () => _stateMachine.TransitionTo<GameOverState>(),
                "Cannot transition from MainMenu directly to GameOver");
        }
        
        [Test]
        public void TransitionTo_SameState_ThrowsInvalidOperationException()
        {
            // Arrange
            _stateMachine.Initialize();
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(
                () => _stateMachine.TransitionTo<MainMenuState>(),
                "Cannot transition to the same state");
        }
        
        #endregion
        
        #region State Enter/Exit/Update Tests
        
        [Test]
        public void StateTransition_CallsExitOnCurrentState()
        {
            // Arrange
            _stateMachine.Initialize();
            var mainMenuState = _stateMachine.GetState<MainMenuState>() as TestableState;
            
            // Act
            _stateMachine.TransitionTo<GameSetupState>();
            
            // Assert
            Assert.IsTrue(mainMenuState.ExitCalled, "Exit should be called on current state");
        }
        
        [Test]
        public void StateTransition_CallsEnterOnNewState()
        {
            // Arrange
            _stateMachine.Initialize();
            
            // Act
            _stateMachine.TransitionTo<GameSetupState>();
            
            // Assert
            var setupState = _stateMachine.GetState<GameSetupState>() as TestableState;
            Assert.IsTrue(setupState.EnterCalled, "Enter should be called on new state");
        }
        
        [Test]
        public void Update_CallsUpdateOnCurrentState()
        {
            // Arrange
            _stateMachine.Initialize();
            var mainMenuState = _stateMachine.GetState<MainMenuState>() as TestableState;
            
            // Act
            _stateMachine.Update();
            
            // Assert
            Assert.IsTrue(mainMenuState.UpdateCalled, "Update should be called on current state");
        }
        
        [Test]
        public void Update_AfterTransition_CallsUpdateOnNewState()
        {
            // Arrange
            _stateMachine.Initialize();
            _stateMachine.TransitionTo<GameSetupState>();
            var setupState = _stateMachine.GetState<GameSetupState>() as TestableState;
            
            // Act
            _stateMachine.Update();
            
            // Assert
            Assert.IsTrue(setupState.UpdateCalled, "Update should be called on new state");
        }
        
        #endregion
        
        #region Turn State Machine Tests
        
        [Test]
        public void TurnStateMachine_InitializesWithRollDiceState()
        {
            // Arrange
            var turnStateMachine = new TurnStateMachine(_gameContext);
            
            // Act
            turnStateMachine.Initialize();
            
            // Assert
            Assert.IsInstanceOf<RollDiceState>(turnStateMachine.CurrentState);
        }
        
        [Test]
        public void TurnStateMachine_RollDiceToMovePiece_SuccessfulTransition()
        {
            // Arrange
            var turnStateMachine = new TurnStateMachine(_gameContext);
            turnStateMachine.Initialize();
            
            // Act
            turnStateMachine.TransitionTo<MovePieceState>();
            
            // Assert
            Assert.IsInstanceOf<MovePieceState>(turnStateMachine.CurrentState);
        }
        
        [Test]
        public void TurnStateMachine_MovePieceToTakeTurnAction_SuccessfulTransition()
        {
            // Arrange
            var turnStateMachine = new TurnStateMachine(_gameContext);
            turnStateMachine.Initialize();
            turnStateMachine.TransitionTo<MovePieceState>();
            
            // Act
            turnStateMachine.TransitionTo<TakeTurnActionState>();
            
            // Assert
            Assert.IsInstanceOf<TakeTurnActionState>(turnStateMachine.CurrentState);
        }
        
        [Test]
        public void TurnStateMachine_TakeTurnActionToEndTurn_SuccessfulTransition()
        {
            // Arrange
            var turnStateMachine = new TurnStateMachine(_gameContext);
            turnStateMachine.Initialize();
            turnStateMachine.TransitionTo<MovePieceState>();
            turnStateMachine.TransitionTo<TakeTurnActionState>();
            
            // Act
            turnStateMachine.TransitionTo<EndTurnState>();
            
            // Assert
            Assert.IsInstanceOf<EndTurnState>(turnStateMachine.CurrentState);
        }
        
        [Test]
        public void TurnStateMachine_EndTurnToRollDice_ResetsToBeginning()
        {
            // Arrange
            var turnStateMachine = new TurnStateMachine(_gameContext);
            turnStateMachine.Initialize();
            turnStateMachine.TransitionTo<MovePieceState>();
            turnStateMachine.TransitionTo<TakeTurnActionState>();
            turnStateMachine.TransitionTo<EndTurnState>();
            
            // Act
            turnStateMachine.TransitionTo<RollDiceState>();
            
            // Assert
            Assert.IsInstanceOf<RollDiceState>(turnStateMachine.CurrentState);
        }
        
        [Test]
        public void TurnStateMachine_InvalidTurnTransition_ThrowsException()
        {
            // Arrange
            var turnStateMachine = new TurnStateMachine(_gameContext);
            turnStateMachine.Initialize();
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(
                () => turnStateMachine.TransitionTo<EndTurnState>(),
                "Cannot skip from RollDice directly to EndTurn");
        }
        
        #endregion
        
        #region State Transition Logging Tests
        
        [Test]
        public void TransitionTo_LogsStateTransition()
        {
            // Arrange
            _stateMachine.Initialize();
            var transitions = new System.Collections.Generic.List<string>();
            _stateMachine.OnTransitionLogged += (from, to) => 
                transitions.Add($"{from.GetType().Name} -> {to.GetType().Name}");
            
            // Act
            _stateMachine.TransitionTo<GameSetupState>();
            
            // Assert
            Assert.AreEqual(1, transitions.Count);
            Assert.IsTrue(transitions[0].Contains("MainMenuState"));
            Assert.IsTrue(transitions[0].Contains("GameSetupState"));
        }
        
        [Test]
        public void TransitionTo_MultipleTransitions_LogsAllTransitions()
        {
            // Arrange
            _stateMachine.Initialize();
            var transitionCount = 0;
            _stateMachine.OnTransitionLogged += (from, to) => transitionCount++;
            
            // Act
            _stateMachine.TransitionTo<GameSetupState>();
            _stateMachine.TransitionTo<PlayingState>();
            _stateMachine.TransitionTo<GameOverState>();
            
            // Assert
            Assert.AreEqual(3, transitionCount);
        }
        
        #endregion
        
        #region Performance Tests
        
        [Test]
        public void StateTransition_CompletesInAcceptableTime()
        {
            // Arrange
            _stateMachine.Initialize();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            // Act
            for (int i = 0; i < 1000; i++)
            {
                _stateMachine.TransitionTo<GameSetupState>();
                _stateMachine.TransitionTo<PlayingState>();
                _stateMachine.TransitionTo<GameOverState>();
                _stateMachine.TransitionTo<MainMenuState>();
            }
            stopwatch.Stop();
            
            // Assert
            var averageTimeMs = stopwatch.ElapsedMilliseconds / 4000.0;
            Assert.Less(averageTimeMs, 1.0, "State transition should complete in less than 1ms");
        }
        
        #endregion
        
        #region State Validation Tests
        
        [Test]
        public void GetState_WithValidStateType_ReturnsState()
        {
            // Arrange
            _stateMachine.Initialize();
            
            // Act
            var state = _stateMachine.GetState<MainMenuState>();
            
            // Assert
            Assert.IsNotNull(state);
            Assert.IsInstanceOf<MainMenuState>(state);
        }
        
        [Test]
        public void GetState_WithInvalidStateType_ReturnsNull()
        {
            // Arrange
            _stateMachine.Initialize();
            
            // Act
            var state = _stateMachine.GetState<InvalidTestState>();
            
            // Assert
            Assert.IsNull(state);
        }
        
        [Test]
        public void CanTransitionTo_ValidTransition_ReturnsTrue()
        {
            // Arrange
            _stateMachine.Initialize();
            
            // Act
            var canTransition = _stateMachine.CanTransitionTo<GameSetupState>();
            
            // Assert
            Assert.IsTrue(canTransition);
        }
        
        [Test]
        public void CanTransitionTo_InvalidTransition_ReturnsFalse()
        {
            // Arrange
            _stateMachine.Initialize();
            
            // Act
            var canTransition = _stateMachine.CanTransitionTo<GameOverState>();
            
            // Assert
            Assert.IsFalse(canTransition);
        }
        
        #endregion
    }
    
    #region Test Helper Classes
    
    // Game State Machine implementation expectations
    public class GameStateMachine
    {
        private readonly IGameContext _context;
        private readonly Dictionary<Type, IGameState> _states = new Dictionary<Type, IGameState>();
        private readonly Dictionary<Type, HashSet<Type>> _validTransitions = new Dictionary<Type, HashSet<Type>>();
        private readonly List<(IGameState from, IGameState to)> _transitionLog = new List<(IGameState, IGameState)>();
        
        public IGameState CurrentState { get; private set; }
        
        public event Action<IGameState> OnStateExit;
        public event Action<IGameState> OnStateEnter;
        public event Action<IGameState, IGameState> OnTransitionLogged;
        
        public GameStateMachine(IGameContext context)
        {
            _context = context;
            
            // Create and register all states
            _states[typeof(MainMenuState)] = new MainMenuState();
            _states[typeof(GameSetupState)] = new GameSetupState();
            _states[typeof(PlayingState)] = new PlayingState();
            _states[typeof(GameOverState)] = new GameOverState();
            
            // Define valid transitions
            _validTransitions[typeof(MainMenuState)] = new HashSet<Type> { typeof(GameSetupState) };
            _validTransitions[typeof(GameSetupState)] = new HashSet<Type> { typeof(PlayingState), typeof(MainMenuState) };
            _validTransitions[typeof(PlayingState)] = new HashSet<Type> { typeof(GameOverState) };
            _validTransitions[typeof(GameOverState)] = new HashSet<Type> { typeof(MainMenuState) };
            
            CurrentState = _states[typeof(MainMenuState)];
        }
        
        public void Initialize()
        {
            // States are already initialized in constructor
        }
        
        public void TransitionTo<T>() where T : IGameState
        {
            var newStateType = typeof(T);
            
            if (!_states.ContainsKey(newStateType))
                throw new InvalidOperationException($"State {newStateType.Name} not registered");
            
            if (CurrentState != null && CurrentState.GetType() == newStateType)
                throw new InvalidOperationException($"Already in state {newStateType.Name}");
            
            if (CurrentState != null && !CanTransitionTo<T>())
                throw new InvalidOperationException($"Cannot transition from {CurrentState.GetType().Name} to {newStateType.Name}");
            
            var newState = _states[newStateType];
            var oldState = CurrentState;
            
            // Exit current state
            if (CurrentState != null)
            {
                CurrentState.Exit();
                OnStateExit?.Invoke(CurrentState);
            }
            
            // Transition
            CurrentState = newState;
            _transitionLog.Add((oldState, newState));
            OnTransitionLogged?.Invoke(oldState, newState);
            
            // Enter new state
            CurrentState.Enter();
            OnStateEnter?.Invoke(CurrentState);
        }
        
        public void Update()
        {
            CurrentState?.Update();
        }
        
        public T GetState<T>() where T : IGameState
        {
            var stateType = typeof(T);
            if (_states.ContainsKey(stateType))
                return (T)_states[stateType];
            return default(T);
        }
        
        public bool CanTransitionTo<T>() where T : IGameState
        {
            if (CurrentState == null)
                return true;
            
            var currentType = CurrentState.GetType();
            var newType = typeof(T);
            
            return _validTransitions.ContainsKey(currentType) && _validTransitions[currentType].Contains(newType);
        }
    }
    
    public class TurnStateMachine
    {
        private readonly IGameContext _context;
        private readonly Dictionary<Type, IGameState> _states = new Dictionary<Type, IGameState>();
        private readonly Dictionary<Type, HashSet<Type>> _validTransitions = new Dictionary<Type, HashSet<Type>>();
        
        public IGameState CurrentState { get; private set; }
        
        public TurnStateMachine(IGameContext context)
        {
            _context = context;
            
            // Create and register all turn states
            _states[typeof(RollDiceState)] = new RollDiceState();
            _states[typeof(MovePieceState)] = new MovePieceState();
            _states[typeof(TakeTurnActionState)] = new TakeTurnActionState();
            _states[typeof(EndTurnState)] = new EndTurnState();
            
            // Define valid turn state transitions
            _validTransitions[typeof(RollDiceState)] = new HashSet<Type> { typeof(MovePieceState) };
            _validTransitions[typeof(MovePieceState)] = new HashSet<Type> { typeof(TakeTurnActionState) };
            _validTransitions[typeof(TakeTurnActionState)] = new HashSet<Type> { typeof(EndTurnState) };
            _validTransitions[typeof(EndTurnState)] = new HashSet<Type> { typeof(RollDiceState) };
            
            CurrentState = _states[typeof(RollDiceState)];
        }
        
        public void Initialize()
        {
            // States are already initialized in constructor
        }
        
        public void TransitionTo<T>() where T : IGameState
        {
            var newStateType = typeof(T);
            
            if (!_states.ContainsKey(newStateType))
                throw new InvalidOperationException($"State {newStateType.Name} not registered");
            
            var currentType = CurrentState?.GetType();
            if (!_validTransitions.ContainsKey(currentType) || !_validTransitions[currentType].Contains(newStateType))
                throw new InvalidOperationException($"Cannot transition from {currentType?.Name} to {newStateType.Name}");
            
            var newState = _states[newStateType];
            
            // Exit current state
            if (CurrentState != null)
            {
                CurrentState.Exit();
            }
            
            // Transition
            CurrentState = newState;
            
            // Enter new state
            CurrentState.Enter();
        }
        
        public void Update()
        {
            CurrentState?.Update();
        }
    }
    
    // State interfaces and classes
    public interface IGameState
    {
        void Enter();
        void Update();
        void Exit();
    }
    
    public interface IGameContext { }
    
    public class TestGameContext : IGameContext { }
    
    // Game flow states
    public class MainMenuState : TestableState { }
    public class GameSetupState : TestableState { }
    public class PlayingState : TestableState { }
    public class GameOverState : TestableState { }
    
    // Turn states
    public class RollDiceState : TestableState { }
    public class MovePieceState : TestableState { }
    public class TakeTurnActionState : TestableState { }
    public class EndTurnState : TestableState { }
    
    // Test helper state
    public class TestableState : IGameState
    {
        public bool EnterCalled { get; private set; }
        public bool UpdateCalled { get; private set; }
        public bool ExitCalled { get; private set; }
        
        public virtual void Enter() { EnterCalled = true; }
        public virtual void Update() { UpdateCalled = true; }
        public virtual void Exit() { ExitCalled = true; }
    }
    
    public class InvalidTestState : IGameState
    {
        public void Enter() { }
        public void Update() { }
        public void Exit() { }
    }
    
    #endregion
}
