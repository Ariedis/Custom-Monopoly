# Monopoly Frenzy - Test Suite

## Overview

This directory contains comprehensive test cases for the Monopoly Frenzy project, covering both Phase 1 (Core Game Logic) and Phase 2 (User Interface & Basic Gameplay).

**Test Coverage:** 
- Phase 1: Targeting 85%+ code coverage on game logic
- Phase 2: Targeting 90%+ code coverage on UI components

**Test Framework:** Unity Test Framework (NUnit-based)

**Last Updated:** 2026-02-18

---

## Test Structure

```
src/Assets/Tests/
├── EditMode/                                # Edit Mode Tests (Pure C# logic - Phase 1)
│   ├── Core/
│   │   └── GameStateTests.cs                # Game state management tests
│   ├── StateMachine/
│   │   └── StateMachineTests.cs             # State machine and game flow tests
│   ├── Commands/
│   │   └── CommandTests.cs                  # Command pattern implementation tests
│   ├── Rules/
│   │   └── RulesEngineTests.cs              # Monopoly rules engine tests
│   └── Events/
│       └── EventSystemTests.cs              # Event system tests
├── PlayMode/                                # Play Mode Tests (Runtime integration)
│   ├── IntegrationTests.cs                  # Phase 1: Complete game simulation
│   └── UI/                                  # Phase 2: UI & Gameplay Tests
│       ├── MainMenuTests.cs                 # Main menu & game setup
│       ├── GameBoardTests.cs                # Board display & player tokens
│       ├── PlayerHUDAndTurnControlTests.cs  # HUD, turn controls, actions
│       ├── PropertyManagementAndSpecialSituationsTests.cs  # Property mgmt, trading, jail, bankruptcy
│       ├── GameFlowSettingsAndResponsiveTests.cs          # Save/load, settings, responsive UI
│       └── Phase2IntegrationTests.cs        # End-to-end UI workflows
├── TEST-COVERAGE-MATRIX.md                  # Phase 1 test coverage mapping
└── PHASE-2-TEST-COVERAGE-MATRIX.md          # Phase 2 test coverage mapping
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

## Phase 1 Test Suites

### EditMode Tests (Pure C# Logic)

| Test Suite | User Story | Test Count | Status |
|------------|------------|------------|--------|
| GameStateTests.cs | 1.1: Game State Management | 35+ | ✅ Complete |
| StateMachineTests.cs | 1.2: State Machine | 25+ | ✅ Complete |
| CommandTests.cs | 1.3: Command Pattern | 50+ | ✅ Complete |
| RulesEngineTests.cs | 1.4: Rules Engine | 80+ | ✅ Complete |
| EventSystemTests.cs | 1.5: Event System | 40+ | ✅ Complete |

### PlayMode Tests (Integration)

| Test Suite | Coverage | Test Count | Status |
|------------|----------|------------|--------|
| IntegrationTests.cs | Complete game scenarios | 30+ | ✅ Complete |

**Phase 1 Total:** 260+ tests

---

## Phase 2 Test Suites

### PlayMode/UI Tests (User Interface & Gameplay)

| Test Suite | User Stories | Test Count | Status |
|------------|--------------|------------|--------|
| MainMenuTests.cs | 2.1-2.2: Main Menu & Setup | 25+ | ✅ Complete |
| GameBoardTests.cs | 2.3-2.4: Board & Tokens | 22+ | ✅ Complete |
| PlayerHUDAndTurnControlTests.cs | 2.5-2.10: HUD & Turn Controls | 55+ | ✅ Complete |
| PropertyManagementAndSpecialSituationsTests.cs | 2.11-2.19: Property Mgmt, Trading, Jail | 80+ | ✅ Complete |
| GameFlowSettingsAndResponsiveTests.cs | 2.20-2.25: Save/Load, Settings, Responsive | 70+ | ✅ Complete |
| Phase2IntegrationTests.cs | Complete UI workflows | 20+ | ✅ Complete |

**Phase 2 Total:** 310+ tests

**Overall Total:** 570+ comprehensive tests

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

---

## Test Suite Status

### Phase 1 Tests
**Status:** ✅ Complete - Ready for Phase 1 Implementation  
**Test Count:** 260+ comprehensive tests  
**Coverage Target:** 85%+

### Phase 2 Tests  
**Status:** ✅ Complete - Ready for Phase 2 Implementation  
**Test Count:** 310+ comprehensive tests  
**Coverage Target:** 90%+

### Overall
**Total Tests:** 570+  
**Phases Covered:** Phase 1 (Core Logic) + Phase 2 (UI & Gameplay)

---

## Next Steps

### For Phase 1 Implementation:
1. Begin Phase 1 implementation using TDD approach
2. Run EditMode tests frequently to validate logic
3. Ensure all Phase 1 tests pass before starting Phase 2
4. Achieve 85%+ code coverage on core game logic

### For Phase 2 Implementation:
1. Ensure Phase 1 is complete and all tests pass
2. Begin Phase 2 UI implementation using existing tests
3. Run PlayMode/UI tests to validate UI components
4. Test integration between UI and Phase 1 logic
5. Achieve 90%+ code coverage on UI components
6. Validate performance benchmarks (60 FPS, <50ms response)
7. Test responsive design across resolutions (1280x720 to 4K)

---

## Documentation

- **[Phase 1 Test Coverage Matrix](TEST-COVERAGE-MATRIX.md)** - Detailed mapping of Phase 1 acceptance criteria to tests
- **[Phase 2 Test Coverage Matrix](PHASE-2-TEST-COVERAGE-MATRIX.md)** - Detailed mapping of Phase 2 acceptance criteria to tests
- **[Phase 1 Implementation Plan](../../../planning/IMPLEMENTATION-PLAN.md)** - Phase 1 requirements and specifications
- **[Phase 2 Implementation Plan](../../../planning/PHASE-2-IMPLEMENTATION-PLAN.md)** - Phase 2 requirements and specifications
- **[Architecture Summary](../../../specifications/ARCHITECTURE-SUMMARY.md)** - System architecture overview

---

## Test Execution

### Run All Tests
```bash
# Run all EditMode tests (Phase 1)
Unity -runTests -testPlatform EditMode -testResults results-phase1.xml -projectPath /path/to/project

# Run all PlayMode tests (Phase 1 Integration + Phase 2 UI)
Unity -runTests -testPlatform PlayMode -testResults results-phase2.xml -projectPath /path/to/project
```

### Run Specific Test Suites
```bash
# Run only Phase 1 tests
Unity -runTests -testPlatform EditMode -testFilter "MonopolyFrenzy.Tests" -projectPath /path/to/project

# Run only Phase 2 UI tests
Unity -runTests -testPlatform PlayMode -testFilter "MonopolyFrenzy.Tests.UI" -projectPath /path/to/project

# Run only integration tests
Unity -runTests -testPlatform PlayMode -testFilter "IntegrationTests" -projectPath /path/to/project
```

---

## Success Criteria

### Phase 1 Complete When:
- ✅ All 260+ Phase 1 tests passing (100%)
- ✅ Code coverage ≥85%
- ✅ Performance benchmarks met
- ✅ No critical bugs
- ✅ Documentation complete

### Phase 2 Complete When:
- ✅ All 310+ Phase 2 tests passing (100%)
- ✅ All 260+ Phase 1 tests still passing
- ✅ UI test coverage ≥90%
- ✅ Performance: 60 FPS, <50ms response, <3s scene load
- ✅ Responsive UI working (1280x720 to 4K)
- ✅ Keyboard navigation functional
- ✅ Save/load working flawlessly
- ✅ Zero UI crashes during 30-minute playtest
