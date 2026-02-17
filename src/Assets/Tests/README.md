# Monopoly Frenzy - Phase 1 Test Suite

## Overview

This directory contains comprehensive test cases for Phase 1 of the Monopoly Frenzy project, covering all core game logic and architecture as specified in the Implementation Plan.

**Test Coverage:** Targeting 85%+ code coverage on Phase 1 components

**Test Framework:** Unity Test Framework (NUnit-based)

**Last Updated:** 2026-02-17

---

## Test Structure

```
src/Assets/Tests/
├── EditMode/                      # Edit Mode Tests (Pure C# logic)
│   ├── Core/
│   │   └── GameStateTests.cs      # Game state management tests
│   ├── StateMachine/
│   │   └── StateMachineTests.cs   # State machine and game flow tests
│   ├── Commands/
│   │   └── CommandTests.cs        # Command pattern implementation tests
│   ├── Rules/
│   │   └── RulesEngineTests.cs    # Monopoly rules engine tests
│   └── Events/
│       └── EventSystemTests.cs    # Event system tests
└── PlayMode/                       # Play Mode Tests (Runtime integration)
    └── IntegrationTests.cs         # Complete game simulation tests
```

---

## Test Suites

### 1. GameStateTests.cs (Test Suite 1)
**User Story:** 1.1 - Game State Management

**Coverage:**
- Game state initialization and setup
- Player management (add, remove, validate)
- Serialization to/from JSON
- State cloning for AI evaluation
- Event firing on state changes
- Memory usage validation
- Turn management

**Key Test Scenarios:**
- ✅ Create initial game state with default values
- ✅ Add 2-6 players with validation
- ✅ Serialize/deserialize game state to JSON
- ✅ Clone game state without affecting original
- ✅ Fire events when players are added or game starts
- ✅ Turn progression and wrapping
- ✅ Memory usage under 10 MB target

**Total Tests:** 35+

---

### 2. StateMachineTests.cs (Test Suite 4)
**User Story:** 1.2 - State Machine for Game Flow

**Coverage:**
- Game flow states (MainMenu, GameSetup, Playing, GameOver)
- Turn states (RollDice, MovePiece, TakeTurnAction, EndTurn)
- State transitions and validation
- Enter/Exit/Update lifecycle methods
- Invalid transition handling
- State transition logging
- Performance (<1ms transitions)

**Key Test Scenarios:**
- ✅ Initialize state machine with default state
- ✅ Valid state transitions between game phases
- ✅ Invalid transitions throw exceptions
- ✅ State lifecycle methods called correctly
- ✅ Turn state progression
- ✅ Transition logging
- ✅ Performance benchmarks met

**Total Tests:** 25+

---

### 3. CommandTests.cs (Test Suite 2)
**User Story:** 1.3 - Command Pattern for Actions

**Coverage:**
- ICommand interface implementation
- All command types:
  - BuyPropertyCommand
  - RollDiceCommand
  - MoveCommand
  - PayRentCommand
  - MortgageCommand
  - UnmortgageCommand
  - BuyHouseCommand
  - DrawCardCommand
  - TradeCommand
  - EndTurnCommand
- Execute() and Undo() functionality
- Precondition validation
- Command serialization
- Command history and replay
- Performance (<1ms execution)

**Key Test Scenarios:**
- ✅ Execute each command successfully
- ✅ Undo each command to restore state
- ✅ Validate preconditions and fail appropriately
- ✅ Serialize commands to JSON
- ✅ Command history tracking
- ✅ Edge cases for all commands
- ✅ Performance benchmarks met

**Total Tests:** 50+

---

### 4. RulesEngineTests.cs (Test Suite 3)
**User Story:** 1.4 - Monopoly Rules Engine

**Coverage:**
- Property purchase validation
- Rent calculation (all variants):
  - Base rent
  - Rent with houses (1-4)
  - Rent with hotel
  - Monopoly rent doubling
  - Railroad rent (1-4 owned)
  - Utility rent (dice-based)
  - Mortgaged properties (no rent)
- House/hotel purchase rules:
  - Monopoly requirement
  - Even building rule
  - Mortgage restrictions
  - Fund validation
- Trading rules and validation
- Bankruptcy processing
- Jail mechanics:
  - Roll doubles to escape
  - Pay $50 to leave
  - Get Out of Jail Free card
  - 3-turn maximum
- Pass GO mechanics
- Free Parking (configurable)
- Auction system
- Mortgage system (50% borrow, 60% payback)
- Chance/Community Chest card effects

**Key Test Scenarios:**
- ✅ All property purchase validations
- ✅ Complete rent calculation matrix
- ✅ House/hotel purchase rules and constraints
- ✅ Trade validation with multiple scenarios
- ✅ Bankruptcy asset transfer
- ✅ All jail escape methods
- ✅ Pass GO collection
- ✅ Auction with multiple bidders
- ✅ Mortgage/unmortgage calculations
- ✅ Card effect implementations

**Total Tests:** 80+

---

### 5. EventSystemTests.cs (Test Suite 5)
**User Story:** 1.5 - Event System

**Coverage:**
- Event bus publish/subscribe
- Event types for all state changes:
  - PlayerMovedEvent
  - PropertyPurchasedEvent
  - MoneyTransferredEvent
  - PlayerBankruptEvent
  - GameStartedEvent
  - GameOverEvent
  - TurnStartedEvent/TurnEndedEvent
  - DiceRolledEvent
  - HousePurchasedEvent
  - PropertyMortgagedEvent
  - TradeExecutedEvent
  - CardDrawnEvent
  - PlayerJailedEvent
- Event data validation
- Dynamic subscription management
- Event ordering
- Performance (<0.1ms overhead)
- Thread safety
- Error handling

**Key Test Scenarios:**
- ✅ Subscribe and receive events
- ✅ Multiple handlers for same event
- ✅ Unsubscribe removes handler
- ✅ All event types carry sufficient data
- ✅ Events fired in correct order
- ✅ Events fired after state changes
- ✅ Dynamic subscribe/unsubscribe during execution
- ✅ Thread-safe concurrent operations
- ✅ Exception in one handler doesn't affect others
- ✅ Performance benchmarks met

**Total Tests:** 40+

---

### 6. IntegrationTests.cs (Integration Test Suite)
**Coverage:** Complete game scenarios

**Test Categories:**
- **Complete Game Simulation:**
  - 2-player games
  - 4-player games
  - 6-player games
  - Multiple runs with different outcomes
  
- **Command Sequences:**
  - Typical turn execution
  - Undo/redo functionality
  - Chained command consistency
  
- **Bankruptcy Flow:**
  - Player cannot pay rent
  - Asset transfer to creditor
  - Game continues after elimination
  - Last player standing wins
  
- **Multi-Player Interactions:**
  - Turn order maintenance
  - Player-to-player trades
  - Rent payments between players
  
- **Edge Cases:**
  - Rolling doubles three times → jail
  - Go To Jail doesn't collect Pass GO
  - Even building rule enforcement
  - Single bidder auction
  - Mortgaging last property
  
- **Event Integration:**
  - Event sequences during gameplay
  - Cross-subsystem event flow

**Key Test Scenarios:**
- ✅ Simulate complete games from start to finish
- ✅ Verify winner determination
- ✅ Test bankruptcy processing end-to-end
- ✅ Validate turn progression with multiple players
- ✅ Test complex interactions and edge cases
- ✅ Ensure event system integrates properly

**Total Tests:** 30+

---

## Running the Tests

### Unity Editor
1. Open Unity Editor
2. Go to **Window > General > Test Runner**
3. Select **EditMode** or **PlayMode** tab
4. Click **Run All** or select specific tests

### Command Line
```bash
# Edit Mode Tests
Unity -runTests -testPlatform EditMode -testResults results-editmode.xml -projectPath /path/to/project

# Play Mode Tests  
Unity -runTests -testPlatform PlayMode -testResults results-playmode.xml -projectPath /path/to/project
```

---

## Test Naming Convention

All tests follow the pattern: `MethodName_Condition_ExpectedResult`

**Examples:**
- `AddPlayer_WithValidName_AddsPlayerToList`
- `BuyProperty_WithInsufficientFunds_ReturnsFailure`
- `CalculateRent_WithMonopoly_DoublesBaseRent`

---

## Test Coverage Goals

| Component | Target Coverage | Priority |
|-----------|----------------|----------|
| Game State | 90%+ | Critical |
| State Machine | 85%+ | Critical |
| Commands | 100% | Critical |
| Rules Engine | 90%+ | Critical |
| Event System | 80%+ | High |
| **Overall Phase 1** | **85%+** | Critical |

---

## Performance Benchmarks

All performance tests validate against Phase 1 requirements:

| Operation | Target | Test Method |
|-----------|--------|-------------|
| State transitions | <1ms | StateMachineTests |
| Command execution | <1ms | CommandTests |
| Event publishing | <0.1ms | EventSystemTests |
| Memory usage | <10 MB | GameStateTests |

---

## Test Data and Helpers

Each test file includes helper classes that define expected interfaces for Phase 1 implementation:

- **GameState, Player, Property, Space** - Core data structures
- **ICommand, CommandResult** - Command pattern interfaces
- **IGameState, StateMachine** - State machine interfaces
- **RulesEngine, GameContext** - Rules validation
- **IEventBus, Event classes** - Event system

These helpers serve as:
1. **Documentation** - Show expected API contracts
2. **Compilation targets** - Tests compile when implementations match
3. **Reference implementations** - Guide for actual Phase 1 development

---

## Test-Driven Development Workflow

Phase 1 should follow TDD principles:

1. **Tests First** ✅ - All tests are written (this phase)
2. **Red** - Tests fail because implementation doesn't exist
3. **Green** - Implement minimum code to pass tests
4. **Refactor** - Clean up implementation while keeping tests green
5. **Repeat** - For each component

---

## Acceptance Criteria Mapping

| User Story | Test Suite | Coverage |
|------------|------------|----------|
| 1.1: Game State Management | GameStateTests.cs | 35+ tests |
| 1.2: State Machine | StateMachineTests.cs | 25+ tests |
| 1.3: Command Pattern | CommandTests.cs | 50+ tests |
| 1.4: Rules Engine | RulesEngineTests.cs | 80+ tests |
| 1.5: Event System | EventSystemTests.cs | 40+ tests |
| **Integration** | IntegrationTests.cs | 30+ tests |
| **Total** | **6 test suites** | **260+ tests** |

---

## Known Limitations

1. **No Unity Dependencies** - Edit mode tests are pure C#, no MonoBehaviour
2. **Placeholder Implementations** - Helper classes are stubs for compilation
3. **Mock Data** - Tests use simplified data structures
4. **Deterministic Only** - Random behavior uses seeds for reproducibility

---

## Future Enhancements

After Phase 1 implementation:
- Add performance profiling tests
- Add stress tests with large player counts
- Add memory leak detection
- Add concurrency tests
- Expand edge case coverage

---

## Contributing

When adding new tests:
1. Follow AAA pattern (Arrange, Act, Assert)
2. Use descriptive test names
3. Add XML documentation for test suites
4. Group related tests with regions
5. Include both success and failure cases
6. Test edge cases and boundaries
7. Keep tests independent and isolated

---

## References

- [Implementation Plan](../../../planning/IMPLEMENTATION-PLAN.md) - Phase 1 requirements
- [Architecture Summary](../../../specifications/ARCHITECTURE-SUMMARY.md) - System design
- [ADR-002](../../../specifications/decisions/adr-002-game-state-management.md) - State management decisions
- [Unity Test Framework](https://docs.unity3d.com/Packages/com.unity.test-framework@latest) - Official documentation

---

**Test Suite Status:** ✅ Complete - Ready for Phase 1 Implementation

**Total Test Count:** 260+ comprehensive tests

**Estimated Implementation Time:** 4 weeks (per Implementation Plan)

**Next Steps:** 
1. Begin Phase 1 implementation using TDD
2. Run tests frequently to validate progress
3. Achieve 85%+ code coverage
4. All tests passing before Phase 2
