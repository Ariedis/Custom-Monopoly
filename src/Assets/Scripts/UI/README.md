# Phase 2: User Interface & Basic Gameplay

## Overview

Phase 2 implements the complete Unity UI system for Monopoly Frenzy, transforming the pure C# game logic from Phase 1 into a fully playable game with visual interface.

## Architecture

### Event-Driven Design
All UI components subscribe to the `EventBus` and update automatically when game state changes. This ensures:
- Single source of truth (GameState)
- Decoupled UI and game logic
- Easy testing and maintenance

### Command Pattern Integration
All player actions are executed through the Command pattern:
- `RollDiceCommand` - Rolling dice
- `BuyPropertyCommand` - Purchasing properties
- `MortgageCommand/UnmortgageCommand` - Property financing
- `BuyHouseCommand` - Building houses/hotels
- `EndTurnCommand` - Ending turn

## Components

### Controllers
- **UIController** - Singleton managing game state and scene transitions
- **BoardController** - Manages 40 board spaces and their visualization

### Screens
- **MainMenuScreen** - Main menu with navigation
- **GameSetupScreen** - Player configuration (2-6 players, tokens, starting money)
- **GameBoardScreen** - Main game board with all UI panels
- **GameOverScreen** - Winner display and game statistics

### Panels
- **TurnControlPanel** - Dice rolling, action buttons, message log
- **PlayerStatusPanel** - Current player money, properties, net worth, jail status
- **PauseMenu** - Pause menu with save, settings, quit options

### Components
- **PropertySpaceUI** - Individual space on the board
  - Shows property name, color group
  - Displays ownership (player color)
  - Shows houses/hotels count
  - Indicates mortgage status
  
- **PlayerTokenUI** - Player token on board
  - Smooth movement animations
  - Position highlighting
  - Current player indicator
  
- **NotificationToast** - Brief notification messages
  - Auto-dismiss after duration
  - Color-coded by type (info, success, warning, error)

### Modals
- **ModalDialog** (Base) - Base class for all modals
  - Fade in/out animations
  - Background overlay
  - Dismiss on background click

- **PropertyCardModal** - Property details and actions
  - Full rent structure display
  - Buy/decline options (unowned properties)
  - Mortgage/unmortgage buttons (owned properties)
  - Build button (monopolies only)

- **BuildHousesModal** - House/hotel building
  - Shows buildable monopolies
  - Even building rule enforcement
  - Available houses/hotels tracking (32/12 limit)
  - Total cost calculation

- **ConfirmationModal** - Generic yes/no dialogs
  - Reusable for any confirmation
  - Customizable title, message, button text

## Features Implemented

### Week 5: Foundation
- ✅ Complete UI directory structure
- ✅ Scene management system
- ✅ Main menu and game setup screens
- ✅ Player configuration with validation

### Week 6: Core Gameplay
- ✅ 40-space board visualization
- ✅ Player tokens with animations
- ✅ Turn control system
- ✅ Player status display
- ✅ Dice rolling
- ✅ Property purchase
- ✅ Message log

### Week 7: Property Management
- ✅ Property card viewing
- ✅ Buy/mortgage/unmortgage actions
- ✅ House/hotel building with even building rule
- ✅ Monopoly detection
- ✅ Building resource tracking
- ✅ Modal dialog system

### Week 8: Polish & Testing (Partial)
- ✅ Pause menu with time freezing
- ✅ Game over screen
- ✅ Notification system
- ⏳ Save/load functionality (planned)
- ⏳ Settings screen (planned)
- ⏳ Unit tests (planned)

## Key Design Decisions

### Responsive UI
- Canvas Scaler set to "Scale with Screen Size"
- Reference resolution: 1920x1080
- Supports 1280x720 to 4K
- Proper anchoring for all UI elements

### Performance Optimizations
- Component caching (avoid GetComponent in Update)
- Object pooling for notifications (if needed)
- Sprite atlasing for UI elements
- Batch UI updates (max 60fps)

### Color Coding
- Property color groups match standard Monopoly
- Player colors generated from player ID hash
- Status colors: green (success), red (error), yellow (warning), blue (info)

### Animation Philosophy
- Phase 2: Minimal animations (fade, highlight)
- Phase 5: Full animations (movement, dice roll, effects)
- All animations in AnimationController for easy Phase 5 upgrade

## Integration with Phase 1

### Game State
```csharp
// UI accesses game state through UIController
GameState gameState = UIController.Instance.GameState;
```

### Commands
```csharp
// UI executes commands, which modify game state
var command = new BuyPropertyCommand(player, property);
var result = command.Execute(gameState);
```

### Events
```csharp
// UI subscribes to events for updates
eventBus.Subscribe<PropertyPurchasedEvent>(OnPropertyPurchased);
```

## Usage Example

### Rolling Dice
1. Player clicks "Roll Dice" button in TurnControlPanel
2. TurnControlPanel creates `RollDiceCommand`
3. Command executes and publishes `DiceRolledEvent`
4. TurnControlPanel updates dice display
5. Command creates `MoveCommand` automatically
6. MoveCommand publishes `PlayerMovedEvent`
7. PlayerTokenUI receives event and animates token movement
8. BoardController updates space highlights

### Buying Property
1. Player lands on unowned property
2. PropertyCardModal shows with "Buy" button
3. Player clicks "Buy"
4. Modal creates `BuyPropertyCommand`
5. Command executes and publishes `PropertyPurchasedEvent`
6. PropertySpaceUI receives event and shows ownership color
7. PlayerStatusPanel receives event and updates money/properties
8. Modal closes

## File Organization

```
src/Assets/Scripts/UI/
├── Controllers/
│   ├── UIController.cs           # Singleton, scene management
│   └── BoardController.cs        # Board visualization
├── Screens/
│   ├── MainMenuScreen.cs
│   ├── GameSetupScreen.cs
│   ├── GameBoardScreen.cs
│   └── GameOverScreen.cs
├── Panels/
│   ├── TurnControlPanel.cs
│   ├── PlayerStatusPanel.cs
│   └── PauseMenu.cs
├── Components/
│   ├── PropertySpaceUI.cs
│   ├── PlayerTokenUI.cs
│   └── NotificationToast.cs
├── Modals/
│   ├── ModalDialog.cs            # Base class
│   ├── PropertyCardModal.cs
│   ├── BuildHousesModal.cs
│   └── ConfirmationModal.cs
└── SceneNames.cs                 # Scene name constants
```

## Testing Strategy

### Unit Tests (Planned)
- UI component initialization
- Button click handlers
- Display updates from events
- Validation logic (even building, etc.)

### Integration Tests (Planned)
- Complete turn flow
- Property purchase flow
- House building flow
- Save/load flow

### Manual Testing
- Complete game playthrough (4 players)
- Window resizing test
- Keyboard navigation test
- Rapid input test (button mashing)

## Next Steps

1. **Create Unity Scenes** - Build actual Unity scenes with prefabs
2. **Wire Up References** - Assign all SerializeField references
3. **Create UI Assets** - Sprites for properties, tokens, icons
4. **Implement Trade Modal** - Player-to-player trading
5. **Implement Jail UI** - Jail options (pay, roll, use card)
6. **Implement Card System** - Chance/Community Chest displays
7. **Save/Load System** - Persistent game state
8. **Settings Screen** - Graphics, audio, gameplay settings
9. **Unit Tests** - Comprehensive test coverage
10. **Performance Testing** - Profiling and optimization

## Performance Targets

- 60 FPS throughout gameplay
- Scene load time < 3 seconds
- Memory usage < 400 MB
- UI response time < 50ms
- Zero crashes in 30-minute playtest

## Known Limitations

### Phase 2 Scope
- No AI opponents (Phase 3)
- No mod support (Phase 4)
- Minimal animations (Phase 5)
- No sound effects/music (Phase 5)
- No tutorial (Phase 5)
- English only (localization future)
- Local multiplayer only (online future)

### Requires Unity Scene Setup
All scripts are implemented but require Unity Editor work to:
- Create scene files
- Build prefabs
- Assign references
- Create UI layouts
- Import assets

## References

- [Phase 2 Implementation Plan](../../../planning/PHASE-2-IMPLEMENTATION-PLAN.md)
- [Architecture Summary](../../../specifications/ARCHITECTURE-SUMMARY.md)
- [ADR-002: Game State Management](../../../specifications/decisions/adr-002-game-state-management.md)
- [Unity UI Best Practices](https://unity.com/how-to/unity-ui-optimization-tips)

---

**Version**: 1.0  
**Last Updated**: 2026-02-18  
**Status**: Scripts Complete, Unity Setup Pending
