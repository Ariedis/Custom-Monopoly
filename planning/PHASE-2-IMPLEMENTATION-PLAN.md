# Phase 2: User Interface & Basic Gameplay - Implementation Plan

**Date**: 2026-02-18  
**Author**: Senior Business Analyst  
**Status**: Approved  
**Phase Duration**: Weeks 5-8 (4 weeks)  
**Related Specifications**: 
- [Architecture Summary](../specifications/ARCHITECTURE-SUMMARY.md)
- [System Overview](../specifications/architecture/monopoly-frenzy-system-overview.md)
- [ADR-002: Game State Management](../specifications/decisions/adr-002-game-state-management.md)
- [State Machine Pattern](../specifications/patterns/state-machine-pattern.md)

---

## 1. Overview

### Purpose
Phase 2 transforms the pure C# game logic from Phase 1 into a playable game with a visual interface. This phase creates all UI screens, game board visualization, player controls, and integrates them with the existing game logic to enable complete human-vs-human gameplay.

### Business Value
- Makes the game playable for humans (Phase 1 is logic-only)
- Validates game feel and user experience early
- Provides foundation for AI integration (Phase 3)
- Enables early playtesting and feedback
- De-risks UI implementation before adding complex features

### Scope

**In Scope:**
- Main menu with navigation
- Game setup screen (player configuration)
- Game board visualization (all 40 spaces)
- Player HUD (money, position, properties)
- Turn controls (roll dice, buy property, end turn)
- Property cards and information displays
- Trade dialog (basic version)
- Mortgage/unmortgage dialogs
- House/hotel building dialogs
- Pause menu and settings
- Input handling (mouse, keyboard)
- Window resizing and responsive UI
- Integration with Phase 1 game logic
- Save/load game functionality

**Out of Scope:**
- AI opponents (Phase 3)
- Mod system (Phase 4)
- Animations and polish (Phase 5)
- Sound effects and music (Phase 5)
- Tutorial system (Phase 5)
- Achievements (Phase 5)
- Online multiplayer (post-launch)

### Success Criteria
- [ ] Human players can play complete games from start to finish
- [ ] All Monopoly rules accessible through UI
- [ ] 60 FPS maintained throughout gameplay
- [ ] UI responsive to window resizing (1280x720 to 4K)
- [ ] Keyboard navigation functional for all critical actions
- [ ] Zero UI-related crashes during 30-minute playtest
- [ ] Save/load preserves complete game state
- [ ] All UI elements accessible within 3 clicks from any screen
- [ ] Property information clearly visible and understandable
- [ ] Turn flow clear and intuitive (user never confused about next action)

---

## 2. User Stories

### Epic: Main Menu & Navigation

#### User Story 2.1: Main Menu
**As a** player  
**I want to** access game options from a clear main menu  
**So that** I can start a game, load saved games, or change settings

**Acceptance Criteria:**
- [ ] Main menu displays with clear branding and title
- [ ] "New Game" button launches game setup screen
- [ ] "Load Game" button shows list of saved games
- [ ] "Settings" button opens settings screen
- [ ] "Quit" button exits the application
- [ ] All buttons have hover effects
- [ ] Keyboard navigation works (Tab, Enter, Escape)
- [ ] Menu loads in <2 seconds on target hardware
- [ ] Background is visually appealing (consistent with Monopoly theme)

**Priority**: Critical  
**Estimated Complexity**: Simple

---

#### User Story 2.2: Game Setup
**As a** player  
**I want to** configure players before starting  
**So that** I can customize the game experience

**Acceptance Criteria:**
- [ ] Can add 2-6 players (human only in Phase 2)
- [ ] Each player has: name, token choice (8 classic tokens)
- [ ] Can set player order or randomize
- [ ] Can configure starting money (default $1500, range $500-$5000)
- [ ] Can enable/disable house rules (Free Parking, auction on decline, etc.)
- [ ] "Start Game" button launches game board screen
- [ ] "Back" button returns to main menu
- [ ] All player names must be unique (validation)
- [ ] Token selection prevents duplicates
- [ ] Setup persists if returning from game (can edit before restarting)

**Priority**: Critical  
**Estimated Complexity**: Medium

---

### Epic: Game Board & Visualization

#### User Story 2.3: Board Display
**As a** player  
**I want to** see the complete Monopoly board clearly  
**So that** I understand property positions and game state

**Acceptance Criteria:**
- [ ] All 40 spaces visible and clearly labeled
- [ ] Properties show color group (visual border)
- [ ] Properties show ownership (player color indicator)
- [ ] Properties show house/hotel count (visual icons)
- [ ] Railroads and utilities visually distinct
- [ ] Corner spaces (GO, Jail, Free Parking, Go To Jail) prominent
- [ ] Chance and Community Chest spaces identifiable by icon
- [ ] Tax spaces clearly marked
- [ ] Board scales appropriately with window size
- [ ] Board maintains aspect ratio and legibility
- [ ] Can zoom board or adjust view (optional but recommended)

**Priority**: Critical  
**Estimated Complexity**: Complex

---

#### User Story 2.4: Player Tokens
**As a** player  
**I want to** see player tokens on the board  
**So that** I know where each player is positioned

**Acceptance Criteria:**
- [ ] Each player has distinct token icon (8 classic choices: car, hat, ship, etc.)
- [ ] Tokens positioned on correct space
- [ ] Multiple tokens on same space stack or offset (all visible)
- [ ] Current player's token highlighted or larger
- [ ] Tokens update position immediately when player moves
- [ ] Token movement path visible (brief highlight of spaces traveled)
- [ ] Tokens in jail positioned in "Just Visiting" area

**Priority**: Critical  
**Estimated Complexity**: Medium

---

### Epic: Player HUD & Information

#### User Story 2.5: Player Status Display
**As a** player  
**I want to** see my money, properties, and status  
**So that** I can make informed decisions

**Acceptance Criteria:**
- [ ] Current player's money displayed prominently (large, clear font)
- [ ] Current player's name and token shown
- [ ] Current player's properties listed (name, color group)
- [ ] Property count displayed
- [ ] "Get Out of Jail Free" cards shown if owned
- [ ] Jail status visible if applicable (turns remaining)
- [ ] Net worth displayed (money + property values)
- [ ] Bankruptcy status clear if applicable
- [ ] HUD updates immediately when values change
- [ ] HUD visible at all times during gameplay

**Priority**: Critical  
**Estimated Complexity**: Medium

---

#### User Story 2.6: Other Players Summary
**As a** player  
**I want to** see other players' status at a glance  
**So that** I can assess competition

**Acceptance Criteria:**
- [ ] List of all players with names and tokens
- [ ] Each player shows: money amount, property count, status (active/bankrupt)
- [ ] Current player highlighted in list
- [ ] Bankrupt players grayed out or marked
- [ ] Players in jail shown with jail icon
- [ ] Click player to see detailed properties (modal or panel)
- [ ] Turn order visible (numbered or visual indicator)
- [ ] List updates immediately when player status changes

**Priority**: High  
**Estimated Complexity**: Simple

---

### Epic: Turn Controls & Actions

#### User Story 2.7: Roll Dice
**As a** player  
**I want to** roll dice to move my token  
**So that** I can take my turn

**Acceptance Criteria:**
- [ ] "Roll Dice" button visible at start of turn
- [ ] Button disabled if not player's turn
- [ ] Dice result displayed clearly (two dice values, total)
- [ ] Doubles detection shown ("You rolled doubles!")
- [ ] Token moves automatically after dice roll
- [ ] Movement path briefly highlighted
- [ ] If doubles, player can roll again (up to 3 times)
- [ ] Third doubles sends player to jail with message
- [ ] Pass GO detected and $200 added with notification
- [ ] Keyboard shortcut: Space bar to roll dice

**Priority**: Critical  
**Estimated Complexity**: Medium

---

#### User Story 2.8: Property Purchase
**As a** player  
**I want to** buy properties I land on  
**So that** I can build my portfolio

**Acceptance Criteria:**
- [ ] Property card modal appears when landing on unowned property
- [ ] Modal shows: property name, color group, price, rent structure
- [ ] "Buy" button purchases property (deducts money, adds to player's properties)
- [ ] "Decline" button skips purchase (closes modal)
- [ ] "Buy" button disabled if insufficient funds
- [ ] Property ownership updates immediately on board
- [ ] Property added to player's HUD property list
- [ ] Confirmation message: "You purchased [Property Name] for $[Price]"
- [ ] Modal dismisses after action
- [ ] Keyboard shortcuts: Enter for Buy, Escape for Decline

**Priority**: Critical  
**Estimated Complexity**: Medium

---

#### User Story 2.9: Pay Rent
**As a** player  
**I want to** pay rent when landing on others' properties  
**So that** game rules are enforced

**Acceptance Criteria:**
- [ ] Notification appears: "You owe $[Amount] rent to [Owner]"
- [ ] Rent calculated correctly based on property state (houses, hotels, monopoly)
- [ ] Money automatically deducted from player
- [ ] Money automatically added to owner
- [ ] Transaction log entry created
- [ ] If insufficient funds, bankruptcy process starts (see User Story 2.18)
- [ ] Mortgage status handled (no rent on mortgaged properties)
- [ ] Railroads: rent based on number owned by owner (1-4)
- [ ] Utilities: rent based on dice roll (4x or 10x depending on ownership)

**Priority**: Critical  
**Estimated Complexity**: Medium

---

#### User Story 2.10: End Turn
**As a** player  
**I want to** end my turn when finished  
**So that** the next player can play

**Acceptance Criteria:**
- [ ] "End Turn" button visible after all mandatory actions complete
- [ ] Button disabled during mandatory actions (paying rent, etc.)
- [ ] Clicking ends current player's turn
- [ ] Next player becomes active (highlighted in player list)
- [ ] New player's HUD displayed
- [ ] Turn counter incremented
- [ ] Notification: "[Next Player Name]'s turn"
- [ ] If player rolled doubles, "End Turn" not available (must roll again)
- [ ] Keyboard shortcut: Enter key to end turn

**Priority**: Critical  
**Estimated Complexity**: Simple

---

### Epic: Property Management

#### User Story 2.11: Property Cards
**As a** player  
**I want to** view detailed property information  
**So that** I can make strategic decisions

**Acceptance Criteria:**
- [ ] Click any property on board to view property card
- [ ] Property card shows: name, owner, color group, price
- [ ] Shows full rent structure: base, 1-4 houses, hotel
- [ ] Shows house/hotel costs
- [ ] Shows mortgage value
- [ ] Shows current state (unmortgaged, mortgaged, houses, hotel)
- [ ] Shows if property is part of complete color group (monopoly)
- [ ] For railroads: shows rent based on ownership count
- [ ] For utilities: shows rent calculation (4x or 10x dice)
- [ ] Card dismisses with X button or clicking outside

**Priority**: High  
**Estimated Complexity**: Medium

---

#### User Story 2.12: Build Houses/Hotels
**As a** player  
**I want to** build houses and hotels on my monopolies  
**So that** I can increase rent income

**Acceptance Criteria:**
- [ ] "Build" button in property card if player owns monopoly
- [ ] Shows available houses (32 total in game) and hotels (12 total)
- [ ] Can select property and number of houses to build (1-4)
- [ ] Can upgrade 4 houses to hotel
- [ ] Even building rule enforced (must build evenly across color group)
- [ ] House cost deducted from player money
- [ ] "Build" button disabled if insufficient funds
- [ ] Houses visually appear on board immediately
- [ ] Cannot build on mortgaged properties
- [ ] Cannot build if any property in group is mortgaged
- [ ] Confirmation dialog: "Build [X] house(s) on [Property] for $[Cost]?"

**Priority**: High  
**Estimated Complexity**: Complex

---

#### User Story 2.13: Mortgage Properties
**As a** player  
**I want to** mortgage properties for cash  
**So that** I can stay solvent

**Acceptance Criteria:**
- [ ] "Mortgage" button in property card for owned properties
- [ ] Mortgage value shown (50% of property price)
- [ ] Clicking mortgages property and adds cash to player
- [ ] Mortgaged property grayed out on board
- [ ] "MORTGAGED" label on property
- [ ] Cannot collect rent on mortgaged property
- [ ] Cannot build houses on color group with mortgaged property
- [ ] Confirmation dialog: "Mortgage [Property] for $[Amount]?"

**Priority**: High  
**Estimated Complexity**: Simple

---

#### User Story 2.14: Unmortgage Properties
**As a** player  
**I want to** unmortgage properties  
**So that** I can collect rent again

**Acceptance Criteria:**
- [ ] "Unmortgage" button shown for mortgaged properties
- [ ] Unmortgage cost shown (110% of mortgage value)
- [ ] Button disabled if insufficient funds
- [ ] Clicking unmortgages property and deducts cost
- [ ] Property returns to normal state on board
- [ ] Can collect rent again immediately
- [ ] Can build houses if complete monopoly
- [ ] Confirmation dialog: "Unmortgage [Property] for $[Amount]?"

**Priority**: High  
**Estimated Complexity**: Simple

---

### Epic: Trading System

#### User Story 2.15: Initiate Trade
**As a** player  
**I want to** propose trades with other players  
**So that** I can acquire properties strategically

**Acceptance Criteria:**
- [ ] "Trade" button accessible during player's turn
- [ ] Opens trade dialog with two panels: offering and receiving
- [ ] Can select trade partner from active players
- [ ] Can add properties to offer (from player's properties)
- [ ] Can add money to offer ($0-$999999)
- [ ] Can add properties to request (from partner's properties)
- [ ] Can add money to request
- [ ] "Get Out of Jail Free" cards can be traded
- [ ] Cannot trade mortgaged properties
- [ ] Cannot trade properties with houses/hotels (must sell first)
- [ ] "Propose Trade" button sends trade to partner
- [ ] "Cancel" button closes dialog

**Priority**: High  
**Estimated Complexity**: Complex

---

#### User Story 2.16: Accept/Decline Trade
**As a** player  
**I want to** review and respond to trade offers  
**So that** I can control my assets

**Acceptance Criteria:**
- [ ] Trade proposal appears as modal for receiving player
- [ ] Shows clear summary: "You give: [items], You receive: [items]"
- [ ] "Accept" button executes trade (transfers all items)
- [ ] "Decline" button rejects trade
- [ ] "Counter Offer" button opens trade dialog with pre-filled items (optional)
- [ ] Trade cannot be forced (must be accepted)
- [ ] After acceptance, both players' property lists update
- [ ] Notification to both players: "Trade complete" or "Trade declined"
- [ ] Cannot respond to trade if not your turn (queued for your turn)

**Priority**: High  
**Estimated Complexity**: Medium

---

### Epic: Special Situations

#### User Story 2.17: Jail Mechanics
**As a** player  
**I want to** manage jail time  
**So that** I can minimize lost turns

**Acceptance Criteria:**
- [ ] When sent to jail, token moves to jail space
- [ ] "In Jail" status shown in HUD
- [ ] On turn in jail, three options shown:
  - [ ] "Pay $50" - immediately released, can roll and move
  - [ ] "Use Get Out of Jail Free Card" - if owned, released immediately
  - [ ] "Try to Roll Doubles" - if successful, released and moves; if fail, turn ends
- [ ] After 3 failed rolls, must pay $50 and released automatically
- [ ] Turn counter for jail (1/3, 2/3, 3/3)
- [ ] If doubles rolled in jail, player moves but doesn't roll again
- [ ] "Just Visiting" status clear if landed on jail normally (not sent)

**Priority**: High  
**Estimated Complexity**: Medium

---

#### User Story 2.18: Bankruptcy
**As a** player  
**I want to** be declared bankrupt when I cannot pay  
**So that** game rules are enforced

**Acceptance Criteria:**
- [ ] When player cannot pay debt, bankruptcy process starts
- [ ] Modal shows: "You owe $[Amount] but only have $[Money]"
- [ ] "Sell Houses" option available if player owns houses
- [ ] "Mortgage Properties" option available if player owns unmortgaged properties
- [ ] If player can raise enough money, debt is paid and play continues
- [ ] If player cannot raise funds, "Declare Bankruptcy" button shown
- [ ] Declaring bankruptcy:
  - [ ] All player's properties unmortgaged and transferred to creditor
  - [ ] If creditor is Bank, properties become unowned
  - [ ] All player's cash transferred to creditor
  - [ ] Player marked as "Bankrupt" and removed from turn order
  - [ ] Player's token removed from board
- [ ] If only one player remains, game over (that player wins)

**Priority**: High  
**Estimated Complexity**: Complex

---

#### User Story 2.19: Chance and Community Chest
**As a** player  
**I want to** draw cards when landing on Chance/Community Chest  
**So that** game variety and rules are enforced

**Acceptance Criteria:**
- [ ] Landing on Chance or Community Chest space triggers card draw
- [ ] Card appears as modal with clear text and visual
- [ ] Card effect executes automatically after reading
- [ ] Types of effects:
  - [ ] Money gain/loss (e.g., "Collect $200")
  - [ ] Movement (e.g., "Advance to GO", "Go Back 3 Spaces")
  - [ ] Go to Jail
  - [ ] Get Out of Jail Free (added to player's inventory)
  - [ ] Pay per property (e.g., "$25 per house, $100 per hotel")
- [ ] Card dismissed with "OK" button or auto-dismiss after 3 seconds
- [ ] Get Out of Jail Free cards retained until used
- [ ] All 16 Chance and 16 Community Chest cards implemented

**Priority**: High  
**Estimated Complexity**: Medium

---

### Epic: Game Flow & State Management

#### User Story 2.20: Save Game
**As a** player  
**I want to** save my game progress  
**So that** I can continue later

**Acceptance Criteria:**
- [ ] "Save Game" option in pause menu
- [ ] Prompts for save game name
- [ ] Saves complete game state:
  - [ ] All player data (money, position, properties, status)
  - [ ] Board state (property ownership, houses, mortgages)
  - [ ] Turn order and current player
  - [ ] Card deck states (remaining cards)
  - [ ] Game settings (rules enabled)
- [ ] Save file written to disk (/Saves/ folder)
- [ ] Confirmation message: "Game saved as [Name]"
- [ ] Can save at any point during game (even mid-turn)
- [ ] Multiple save slots supported (at least 10)

**Priority**: High  
**Estimated Complexity**: Medium

---

#### User Story 2.21: Load Game
**As a** player  
**I want to** load saved games  
**So that** I can continue previous sessions

**Acceptance Criteria:**
- [ ] "Load Game" from main menu shows list of saved games
- [ ] Each save shows: name, date saved, number of players, turn count
- [ ] Selecting save loads complete game state
- [ ] Game board appears with all state restored correctly
- [ ] Current player's turn resumes
- [ ] "Delete Save" option for each save (with confirmation)
- [ ] If save file corrupted, shows error: "Save file invalid"
- [ ] Can load save from within game (via pause menu, abandons current game)

**Priority**: High  
**Estimated Complexity**: Simple

---

#### User Story 2.22: Pause Menu
**As a** player  
**I want to** pause the game and access options  
**So that** I can take breaks or change settings

**Acceptance Criteria:**
- [ ] Escape key or Pause button opens pause menu
- [ ] Pause menu overlays game (semi-transparent background)
- [ ] Options:
  - [ ] "Resume" - returns to game
  - [ ] "Save Game" - saves current state
  - [ ] "Settings" - opens settings screen
  - [ ] "Main Menu" - confirms and returns to main menu (abandons game)
  - [ ] "Quit" - confirms and exits application
- [ ] Game state frozen while paused (turn timer stops if applicable)
- [ ] Keyboard navigation works
- [ ] Can pause at any time during game

**Priority**: Medium  
**Estimated Complexity**: Simple

---

### Epic: Settings & Preferences

#### User Story 2.23: Game Settings
**As a** player  
**I want to** configure game preferences  
**So that** I can customize my experience

**Acceptance Criteria:**
- [ ] Settings accessible from main menu and pause menu
- [ ] Categories:
  - [ ] **Display**: Resolution, fullscreen/windowed, VSync
  - [ ] **Audio**: Master volume, music volume, SFX volume (sliders 0-100%)
  - [ ] **Gameplay**: Animation speed (for Phase 5), confirm actions (on/off)
  - [ ] **Controls**: Keyboard shortcuts customization (optional)
- [ ] Settings persist between sessions (saved to PlayerPrefs or config file)
- [ ] "Defaults" button resets all to default values
- [ ] "Apply" button saves changes
- [ ] "Cancel" button discards changes
- [ ] Changes take effect immediately (except resolution)

**Priority**: Medium  
**Estimated Complexity**: Simple

---

### Epic: Responsive Design & Accessibility

#### User Story 2.24: Window Resizing
**As a** player  
**I want to** resize the game window  
**So that** I can adjust to my screen and preferences

**Acceptance Criteria:**
- [ ] Game supports resolutions from 1280x720 to 3840x2160 (4K)
- [ ] UI scales appropriately with window size
- [ ] Text remains legible at all supported resolutions
- [ ] Board scales to fit window while maintaining aspect ratio
- [ ] No UI elements cut off or overlapping
- [ ] 16:9 aspect ratio primary target
- [ ] Ultra-wide (21:9) supported with letterboxing or adapted layout
- [ ] Fullscreen and windowed modes work correctly
- [ ] Can change resolution in settings

**Priority**: High  
**Estimated Complexity**: Medium

---

#### User Story 2.25: Keyboard Navigation
**As a** player  
**I want to** navigate UI with keyboard  
**So that** I can play without mouse

**Acceptance Criteria:**
- [ ] Tab key cycles through interactive elements
- [ ] Shift+Tab cycles backwards
- [ ] Enter key activates selected button
- [ ] Escape key closes modals or opens pause menu
- [ ] Arrow keys navigate lists or options
- [ ] Space bar rolls dice (during turn)
- [ ] Keyboard shortcuts shown in tooltips
- [ ] Current focus visually indicated (highlight or outline)
- [ ] Keyboard navigation works in all screens (menu, game, settings)

**Priority**: Medium  
**Estimated Complexity**: Medium

---

## 3. Functional Requirements

### FR2.1: Unity Canvas Setup
**Description**: Responsive UI using Unity's Canvas system with proper scaling.

**Rationale**: Ensures UI works across different resolutions and aspect ratios.

**Dependencies**: Unity UI package (built-in)

**Behavior Specification:**
- Given game runs on any resolution between 1280x720 and 4K
- When UI is displayed
- Then Canvas Scaler uses "Scale with Screen Size" mode
- And reference resolution is 1920x1080
- And match is set to 0.5 (width/height blend)
- And all UI elements anchor correctly to parents

**Edge Cases:**
- Ultra-wide monitors (21:9) - use letterboxing or adaptive layout
- Very small windows (<1280x720) - show minimum resolution warning
- 4:3 aspect ratio - pillarbox with centered content

**Testing:**
- Test at 1280x720, 1920x1080, 2560x1440, 3840x2160
- Test fullscreen and windowed modes
- Test window resizing dynamically
- Verify text legibility at all resolutions

---

### FR2.2: Scene Management
**Description**: Manage transitions between scenes (MainMenu, GameSetup, GameBoard).

**Rationale**: Clean scene organization and smooth transitions.

**Dependencies**: Unity SceneManager, Phase 1 GameStateMachine

**Behavior Specification:**
- Given player navigates between screens
- When scene transition triggered
- Then current scene unloads (except persistent objects)
- And new scene loads asynchronously
- And loading screen optional (if load time >1 second)
- And game state persists via singleton or DontDestroyOnLoad

**Edge Cases:**
- Scene load fails - show error and return to main menu
- Memory leak between scenes - ensure proper cleanup
- State not persisting - verify singleton pattern

**Testing:**
- Navigate through all scenes multiple times
- Check memory usage for leaks
- Verify game state persists correctly
- Test rapid scene switching (stress test)

---

### FR2.3: Event-Driven UI Updates
**Description**: UI listens to EventBus from Phase 1 and updates accordingly.

**Rationale**: Decouples UI from game logic, enables easy testing and modifications.

**Dependencies**: Phase 1 EventBus, Phase 1 GameEvents

**Behavior Specification:**
- Given UI component needs to display game data
- When subscribed event is published
- Then UI component receives event
- And updates display immediately
- And unsubscribes when destroyed to prevent memory leaks

**Event Subscriptions by UI Component:**
- PlayerStatusPanel: PlayerMovedEvent, MoneyTransferredEvent, PropertyPurchasedEvent
- BoardView: PropertyPurchasedEvent, HousePurchasedEvent, PropertyMortgagedEvent
- TurnControlPanel: TurnStartedEvent, TurnEndedEvent, DiceRolledEvent
- GameOverScreen: GameOverEvent

**Edge Cases:**
- Multiple events in quick succession - queue and process
- UI destroyed before unsubscribing - memory leak prevention
- Event published before UI initialized - use initial state query

**Testing:**
- Unit test each UI component's event handlers
- Integration test full game flow with UI
- Test rapid events (stress test)
- Verify no memory leaks after destroying UI

---

### FR2.4: Input Management
**Description**: Unified input handling for mouse, keyboard, and future gamepad.

**Rationale**: Supports multiple input methods and enables future expansions.

**Dependencies**: Unity Input System (new) or Legacy Input System

**Behavior Specification:**
- Given player uses input device (mouse, keyboard)
- When input action triggered (click, keypress)
- Then InputManager detects and routes to appropriate handler
- And input method (mouse/keyboard) remembered for UI hints

**Input Mappings:**
- Mouse: All UI clicking, drag-to-scroll board (optional)
- Keyboard:
  - Space: Roll dice
  - Enter: Confirm/End turn
  - Escape: Pause menu
  - Tab/Shift+Tab: Navigate UI
  - 1-9: Quick actions (optional)

**Edge Cases:**
- Simultaneous mouse and keyboard input - mouse takes priority
- Input during transitions - ignore or queue
- Rapid input (double-click prevention) - debounce critical actions

**Testing:**
- Test all input methods independently
- Test mixed input usage
- Test rapid inputs (button mashing)
- Test input during scene transitions

---

### FR2.5: Property Visualization
**Description**: Visual representation of board spaces with property information.

**Rationale**: Clear understanding of board state and ownership.

**Dependencies**: Phase 1 Board, Property, and Space classes

**Behavior Specification:**
- Given 40 spaces on Monopoly board
- When rendered in UI
- Then each space shows:
  - Name (readable at default zoom)
  - Color group (for properties)
  - Ownership indicator (player color)
  - House/hotel count (icons)
  - Mortgage status (grayed out or "MORTGAGED" label)
- And spaces arranged in square layout (10 per side)
- And corner spaces larger and more prominent

**Visual Design Guidelines:**
- Properties: Rectangle with color bar at top, name below
- Railroads: Railroad icon, black color scheme
- Utilities: Light bulb or water drop icon
- Chance: Question mark icon, orange/yellow color
- Community Chest: Treasure chest icon, blue color
- Taxes: Dollar sign, red color
- GO: Large green arrow, "Collect $200"
- Jail: Bars icon, "Just Visiting" vs "In Jail" clear
- Free Parking: Car icon or "FREE" text
- Go To Jail: Police icon or arrow pointing to jail

**Edge Cases:**
- Property name too long - truncate with ellipsis
- Multiple tokens on space - stack or fan out
- Zoomed out - scale text appropriately

**Testing:**
- Verify all 40 spaces render correctly
- Test ownership changes update immediately
- Test house/hotel placement and removal
- Test mortgage visual state
- Test at different zoom levels (if zoom implemented)

---

### FR2.6: Modal Dialog System
**Description**: Reusable modal system for property cards, confirmations, trades.

**Rationale**: Consistent UI patterns and code reuse.

**Dependencies**: Unity UI

**Behavior Specification:**
- Given need to display modal information or get user input
- When ShowModal(type, data) called
- Then modal appears with semi-transparent overlay
- And clicks outside modal are blocked
- And Escape key closes modal (if dismissible)
- And modal animates in/out (fade or scale, optional for Phase 5)

**Modal Types:**
- PropertyCard: Display property details and actions
- Confirmation: Yes/No dialog
- Trade: Trade proposal interface
- CardDraw: Chance/Community Chest card
- Notification: Information only (auto-dismiss or OK button)
- Bankruptcy: Asset liquidation interface

**Edge Cases:**
- Multiple modals requested - queue or stack
- Modal data invalid - show error modal
- Closing modal during action - cancel action safely

**Testing:**
- Test each modal type independently
- Test modal queue/stack
- Test clicking outside to dismiss
- Test keyboard shortcuts (Escape, Enter)
- Test rapid modal open/close

---

### FR2.7: Game State Synchronization
**Description**: UI always reflects current game state from Phase 1 logic.

**Rationale**: Single source of truth prevents desynchronization bugs.

**Dependencies**: Phase 1 GameState

**Behavior Specification:**
- Given UI component displays game data
- When game state changes (via Command execution)
- Then event published from EventBus
- And UI subscribes and updates display
- And UI never directly modifies game state (always via Commands)

**Data Flow:**
```
User Input → UI → Command → GameState → Event → UI Update
```

**UI Components and Data Sources:**
- PlayerStatusPanel: GameState.CurrentPlayer
- PlayerListPanel: GameState.Players
- BoardView: GameState.Board, GameState.Players
- TurnControlPanel: GameState.CurrentPlayer, GameState.TurnPhase
- PropertyCardModal: Selected Property from GameState.Board

**Edge Cases:**
- UI requests data before GameState initialized - show loading
- Command fails - show error, UI remains unchanged
- Rapid state changes - batch UI updates (max 60fps)

**Testing:**
- Integration test: verify UI matches game state after every command
- Test command failure handling
- Test rapid commands (stress test)
- Test loading game from save (state synchronization)

---

### FR2.8: Animation Placeholders
**Description**: Minimal animations for Phase 2 (full animations in Phase 5).

**Rationale**: Phase 2 focuses on functionality; polish comes later.

**Dependencies**: None (Unity's basic transform and UI)

**Behavior Specification:**
- Given animations are planned for Phase 5
- When Phase 2 implements UI
- Then use instant updates with brief highlights:
  - Token movement: Instant teleport with 0.3s highlight on destination
  - Money change: Instant update with 0.2s flash (green for gain, red for loss)
  - Property purchase: Instant ownership change with brief pulse
  - Dice roll: Show result instantly (no rolling animation)
- And all animation code abstracted in AnimationController for Phase 5 upgrade

**Phase 5 Upgrade Path:**
- AnimationController.MovePiece() - currently instant, Phase 5 adds smooth movement
- AnimationController.ShowMoneyChange() - currently flash, Phase 5 adds flying numbers
- AnimationController.RollDice() - currently instant, Phase 5 adds 3D dice roll

**Edge Cases:**
- Animation disabled in settings (Phase 5) - already instant in Phase 2
- Rapid animations - instant updates handle this fine

**Testing:**
- Verify all updates are instant and responsive
- Verify brief highlights work correctly
- Verify AnimationController interface ready for Phase 5

---

## 4. Non-Functional Requirements

### NFR2.1: Performance
- **Frame Rate**: Maintain 60 FPS throughout gameplay on target hardware (Intel i5-8400, GTX 1050 Ti, 8GB RAM)
- **UI Response Time**: Button clicks register within 50ms
- **Scene Load Time**: <3 seconds for any scene transition
- **Memory Usage**: <400 MB total (game logic + UI)
- **Draw Calls**: <100 draw calls per frame (use sprite atlasing and batching)

**Testing:**
- Profile with Unity Profiler during 30-minute gameplay session
- Monitor frame time (should stay <16.67ms for 60 FPS)
- Test on minimum spec hardware

---

### NFR2.2: Usability
- **First-Time User**: Can start and complete a game without tutorial (tutorial in Phase 5)
- **Clarity**: All UI text legible without squinting at 1920x1080
- **Feedback**: All actions provide immediate visual or text feedback
- **Error Messages**: Clear and actionable (e.g., "Insufficient funds. You need $X more.")
- **Consistency**: UI patterns consistent across all screens (button styles, modal layouts)
- **Tooltip Help**: Hover tooltips on all non-obvious elements (explain property rent, etc.)

**Testing:**
- Usability test with 3-5 playtesters unfamiliar with project
- Track confusion points and time to complete first game
- Gather feedback on clarity and intuitiveness

---

### NFR2.3: Reliability
- **Crash Rate**: Zero UI-related crashes during 1-hour stress test
- **Save/Load**: 100% successful save and load with no data corruption
- **Error Handling**: All errors caught and logged, no unhandled exceptions
- **Graceful Degradation**: If asset missing, use placeholder instead of crashing

**Testing:**
- Stress test: Rapid clicking, window resizing, scene switching for 1 hour
- Test save/load 50 times consecutively
- Test with intentionally corrupted save files
- Test with missing or invalid assets

---

### NFR2.4: Maintainability
- **Code Organization**: Clear folder structure (UI/, Controllers/, Views/, Models/)
- **Separation of Concerns**: UI code separate from game logic (Views don't reference Commands directly, use Controllers)
- **Naming Conventions**: Follow Unity C# conventions (PascalCase for public, camelCase for private)
- **Documentation**: XML comments on all public classes and methods
- **Code Reuse**: DRY principle - reusable components (ModalDialog, PlayerPanel, etc.)

**Structure:**
```
Assets/Scripts/
├── UI/
│   ├── Screens/
│   │   ├── MainMenuScreen.cs
│   │   ├── GameSetupScreen.cs
│   │   └── GameBoardScreen.cs
│   ├── Panels/
│   │   ├── PlayerStatusPanel.cs
│   │   ├── PlayerListPanel.cs
│   │   └── TurnControlPanel.cs
│   ├── Modals/
│   │   ├── ModalDialog.cs
│   │   ├── PropertyCardModal.cs
│   │   ├── TradeModal.cs
│   │   └── ConfirmationModal.cs
│   ├── Components/
│   │   ├── PropertySpace.cs
│   │   ├── PlayerToken.cs
│   │   └── DiceDisplay.cs
│   └── Controllers/
│       ├── UIController.cs
│       ├── BoardController.cs
│       └── InputController.cs
└── Tests/
    └── UITests/
        ├── MainMenuTests.cs
        ├── GameSetupTests.cs
        └── GameBoardTests.cs
```

---

### NFR2.5: Accessibility (Basic)
- **Contrast**: Minimum 4.5:1 contrast ratio for all text (WCAG AA standard)
- **Font Size**: Minimum 16pt for body text, 20pt for important info
- **Color Independence**: Don't rely on color alone (use text labels too)
- **Keyboard Navigation**: All critical actions accessible via keyboard
- **Focus Indicators**: Clear visual indicator for keyboard-focused elements

**Note**: Full accessibility (colorblind mode, screen reader support, etc.) planned for Phase 5.

**Testing:**
- Use contrast checker tool on all text
- Navigate entire game using keyboard only
- Review with accessibility guidelines checklist

---

## 5. Unity Implementation Guidance

### Recommended Unity Components

#### 1. **Canvas and UI System**
- Use Unity UI (uGUI) for all UI elements
- Canvas Scaler: "Scale with Screen Size" mode, reference 1920x1080
- GraphicRaycaster on Canvas for mouse interactions
- EventSystem in scene for input handling

#### 2. **Layout Components**
- VerticalLayoutGroup / HorizontalLayoutGroup: For player lists, property lists
- GridLayoutGroup: For property grid (if using grid layout)
- ContentSizeFitter: For dynamic sizing based on content
- AspectRatioFitter: For maintaining proportions (player tokens, board)

#### 3. **UI Components**
- Image: For backgrounds, borders, property spaces
- TextMeshPro (TMP): For all text (better rendering than legacy Text)
- Button: For all clickable elements (with Navigation settings)
- ScrollRect: For scrollable lists (saved games, properties)
- Slider: For settings (volume controls)
- Toggle: For settings checkboxes
- Dropdown: For player count selection, token selection

#### 4. **Prefab Structure**
```
Assets/Prefabs/
├── UI/
│   ├── Screens/
│   │   ├── MainMenuScreen.prefab
│   │   ├── GameSetupScreen.prefab
│   │   └── GameBoardScreen.prefab
│   ├── Modals/
│   │   ├── PropertyCardModal.prefab
│   │   ├── TradeModal.prefab
│   │   └── ConfirmationModal.prefab
│   ├── Panels/
│   │   ├── PlayerStatusPanel.prefab
│   │   ├── PlayerListPanel.prefab
│   │   └── TurnControlPanel.prefab
│   └── Components/
│       ├── PropertySpace.prefab
│       ├── PlayerToken.prefab
│       └── DiceDisplay.prefab
└── Board/
    └── MonopolyBoard.prefab (with all 40 spaces)
```

#### 5. **ScriptableObjects**
```
Assets/ScriptableObjects/
├── UI/
│   ├── UITheme.asset (colors, fonts, styles)
│   └── UISettings.asset (default layout settings)
└── Data/
    └── TokenSprites.asset (8 classic token sprites)
```

#### 6. **Animations (Minimal for Phase 2)**
- Use Unity Animator for basic UI transitions (optional)
- Or use Unity's UI animation system (fade, scale)
- DOTween (Phase 5) for smooth animations

---

### Scene Structure

#### Scene: MainMenu
```
Canvas (Screen Space - Overlay)
├── Background (Image)
├── Title (TextMeshProUGUI)
├── ButtonPanel (VerticalLayoutGroup)
│   ├── NewGameButton
│   ├── LoadGameButton
│   ├── SettingsButton
│   └── QuitButton
└── Version (TextMeshProUGUI, bottom-right)

EventSystem
GameManager (DontDestroyOnLoad singleton)
```

#### Scene: GameSetup
```
Canvas
├── Background
├── TitlePanel
│   ├── BackButton (top-left)
│   └── Title (TextMeshProUGUI)
├── PlayerSetupPanel
│   ├── PlayerList (ScrollRect)
│   │   └── PlayerItem (prefab instantiated 2-6 times)
│   │       ├── NameInputField
│   │       ├── TokenDropdown
│   │       └── RemoveButton
│   ├── AddPlayerButton
│   └── RandomizeOrderButton
├── GameOptionsPanel
│   ├── StartingMoneySlider
│   ├── HouseRulesToggles (list)
│   └── DifficultyDropdown (for Phase 3)
└── StartGameButton

EventSystem
GameSetupController
```

#### Scene: GameBoard
```
Canvas
├── Background
├── BoardPanel (center)
│   ├── Board (prefab with 40 PropertySpace children)
│   │   ├── Space_00_GO
│   │   ├── Space_01_MediterraneanAve
│   │   ├── ...
│   │   └── Space_39_Boardwalk
│   └── TokensContainer (for PlayerToken prefabs)
├── PlayerStatusPanel (left or bottom)
│   ├── CurrentPlayerInfo
│   │   ├── NameAndToken
│   │   ├── MoneyDisplay (large)
│   │   ├── PropertiesList (ScrollRect)
│   │   └── StatusIndicators (jail, bankruptcy)
│   └── OtherPlayersList (ScrollRect)
├── TurnControlPanel (right or bottom)
│   ├── TurnTitle ("Your Turn" / "Player X's Turn")
│   ├── DiceDisplay (shows last roll)
│   ├── ActionButtons
│   │   ├── RollDiceButton
│   │   ├── BuyPropertyButton (conditional)
│   │   ├── TradeButton
│   │   ├── BuildButton
│   │   ├── ManagePropertiesButton
│   │   └── EndTurnButton
│   └── MessageLog (ScrollRect, recent actions)
├── PauseButton (top-right)
└── ModalContainer (for dynamically spawned modals)

EventSystem
GameBoardController
BoardController
UIController
```

---

### Integration with Phase 1 Logic

#### Command Execution Pattern
```
UI User Action:
1. Player clicks "Roll Dice" button
2. UI calls UIController.OnRollDiceClicked()
3. UIController creates RollDiceCommand
4. UIController executes command via CommandInvoker
5. Command executes, modifies GameState
6. Command publishes DiceRolledEvent via EventBus
7. UI components subscribed to event update displays
```

**Example Code Flow (conceptual):**

UI Button Click Handler:
```
When "Roll Dice" button clicked
→ Call UIController.RollDice()
```

UIController Method:
```
RollDice():
  Create RollDiceCommand(currentPlayer)
  Execute command via CommandInvoker
  (Command will publish events internally)
```

UI Component Event Subscription:
```
DiceDisplay subscribes to DiceRolledEvent
When DiceRolledEvent received:
  Update dice visuals with roll result
  
PlayerStatusPanel subscribes to PlayerMovedEvent
When PlayerMovedEvent received:
  Update player position display
  
PlayerStatusPanel subscribes to MoneyTransferredEvent
When MoneyTransferredEvent received:
  Update money display
```

#### State Machine Integration
```
Current State determines available UI actions:
- MenuState: Show main menu UI
- SetupState: Show game setup UI
- PlayingState: Show game board UI
  - RollDiceState: Enable "Roll Dice" button
  - MovePieceState: Disable actions, show movement
  - TakeTurnActionState: Enable action buttons (buy, trade, build)
  - EndTurnState: Enable "End Turn" button
- GameOverState: Show game over UI with winner

UI subscribes to StateChangedEvent
When state changes:
  Update available actions and button states
```

---

### Asset Organization

```
Assets/
├── Scenes/
│   ├── MainMenu.unity
│   ├── GameSetup.unity
│   └── GameBoard.unity
├── Scripts/
│   ├── UI/
│   │   ├── Screens/
│   │   ├── Panels/
│   │   ├── Modals/
│   │   ├── Components/
│   │   └── Controllers/
│   └── (Phase 1 scripts already here)
├── Prefabs/
│   └── UI/
├── Sprites/
│   ├── UI/
│   │   ├── Buttons/
│   │   ├── Backgrounds/
│   │   ├── Icons/
│   │   └── PropertySpaces/
│   └── Tokens/ (8 classic token sprites)
├── Fonts/
│   └── (TextMeshPro fonts)
├── Materials/
│   └── UI/ (if needed for effects)
└── ScriptableObjects/
    └── UI/
```

---

### Sprite Atlasing
- Create sprite atlases for UI elements to reduce draw calls
- Atlases:
  - UIElements.spriteatlas (buttons, borders, backgrounds)
  - PropertySpaces.spriteatlas (40 property space graphics)
  - Tokens.spriteatlas (8 player tokens)
  - Icons.spriteatlas (dice, house, hotel, jail, etc.)

---

### Third-Party Assets (Optional but Recommended)
- **TextMesh Pro**: For high-quality text rendering (free, from Unity)
- **UI Extension Package**: For advanced UI components (optional)
- **Simple Sprite Packer**: For creating sprite atlases (optional, Unity has built-in)

**Do NOT use in Phase 2** (save for Phase 5):
- DOTween (animations)
- Audio packages

---

## 6. Testing Requirements

### Unit Test Scenarios

#### UI Component Tests

**Test: MainMenuScreen Initialization**
- Setup: Load MainMenu scene
- Steps: Instantiate MainMenuScreen
- Expected: All buttons present and clickable
- Pass/Fail: All buttons exist and enabled

**Test: GameSetupScreen Player Addition**
- Setup: Load GameSetup scene
- Steps: Click "Add Player" button 6 times
- Expected: 6 player items created
- Pass/Fail: Exactly 6 player items in list

**Test: PropertySpace Display**
- Setup: Create PropertySpace with test property data
- Steps: Set property owned by player 1 with 2 houses
- Expected: Space shows player 1 color and 2 house icons
- Pass/Fail: Visual state matches data

**Test: DiceDisplay Update**
- Setup: Create DiceDisplay component
- Steps: Publish DiceRolledEvent with result (3, 5)
- Expected: Display shows "3 + 5 = 8"
- Pass/Fail: Text matches roll result

**Test: PlayerStatusPanel Money Update**
- Setup: Create PlayerStatusPanel, subscribe to events
- Steps: Publish MoneyTransferredEvent (player gains $200)
- Expected: Money display updates to new amount
- Pass/Fail: Displayed money matches GameState

---

### Integration Test Scenarios

**Integration Test 1: Complete Turn Flow**
- Setup: Start new game with 2 players
- Steps:
  1. Load game board scene
  2. Click "Roll Dice" button
  3. Verify dice roll displayed
  4. Verify token moved to correct space
  5. Verify "End Turn" button enabled
  6. Click "End Turn"
  7. Verify next player's turn starts
- Expected: Turn flow works smoothly, UI updates correctly
- Pass/Fail: All steps complete without errors, UI matches game state

**Integration Test 2: Property Purchase Flow**
- Setup: Start game, position player on unowned property
- Steps:
  1. Trigger landing on property (roll dice or manual)
  2. Verify property card modal appears
  3. Click "Buy" button
  4. Verify modal closes
  5. Verify property ownership updates on board
  6. Verify property added to player's property list
  7. Verify money deducted from player
- Expected: Property purchase completes correctly
- Pass/Fail: All state changes reflected in UI immediately

**Integration Test 3: Save and Load Game**
- Setup: Start game, play for 5 turns
- Steps:
  1. Open pause menu
  2. Click "Save Game"
  3. Enter save name "TestSave"
  4. Confirm save
  5. Return to main menu
  6. Click "Load Game"
  7. Select "TestSave"
  8. Verify game state restored correctly (players, money, properties, turn)
- Expected: Game state identical after load
- Pass/Fail: All game data matches pre-save state

**Integration Test 4: Trade Flow**
- Setup: Start game with 2 players, give each properties
- Steps:
  1. Current player clicks "Trade" button
  2. Select other player as trade partner
  3. Add property to offer
  4. Add $100 to request
  5. Click "Propose Trade"
  6. Switch to other player's turn (or simulate)
  7. Review trade proposal
  8. Click "Accept"
  9. Verify property and money transferred correctly
- Expected: Trade completes successfully
- Pass/Fail: Both players' properties and money updated correctly

**Integration Test 5: Bankruptcy Flow**
- Setup: Player with $100 lands on property with $500 rent
- Steps:
  1. Land on high-rent property
  2. Verify bankruptcy modal appears
  3. Try to mortgage properties (if owned)
  4. If still insufficient, click "Declare Bankruptcy"
  5. Verify player marked bankrupt
  6. Verify player removed from turn order
  7. Verify properties transferred to creditor
- Expected: Bankruptcy handled correctly
- Pass/Fail: Player removed, assets transferred, game continues

---

### Manual Test Cases

**Manual Test 1: Complete Game Playthrough**
- Setup: New game, 4 players
- Steps: Play complete game until one winner
- Duration: 30-60 minutes
- Expected: Game completes without crashes or errors
- Pass/Fail: Game finishes, winner declared, no bugs

**Manual Test 2: UI Responsiveness Test**
- Setup: New game
- Steps:
  1. Resize window multiple times during gameplay
  2. Switch fullscreen/windowed
  3. Change resolution in settings
- Expected: UI adapts smoothly, no visual glitches
- Pass/Fail: UI remains functional and legible

**Manual Test 3: Keyboard Navigation Test**
- Setup: Main menu
- Steps: Navigate entire game using keyboard only (no mouse)
- Expected: All critical actions accessible via keyboard
- Pass/Fail: Can complete a game using keyboard only

**Manual Test 4: Rapid Input Test**
- Setup: Game board
- Steps: Click buttons rapidly, roll dice multiple times quickly
- Expected: Game handles rapid input gracefully (debouncing)
- Pass/Fail: No duplicate actions, no crashes

**Manual Test 5: All UI Screens Test**
- Setup: Launch game
- Steps: Visit every UI screen (menu, setup, board, settings, pause, load)
- Expected: All screens functional, transitions smooth
- Pass/Fail: All screens load, no errors

---

### Performance Testing

**Performance Test 1: Frame Rate Stability**
- Setup: New game, 6 players
- Tools: Unity Profiler
- Duration: 30 minutes
- Metrics: Track FPS, frame time
- Pass Criteria: FPS stays above 60, frame time <16.67ms
- Actions: Normal gameplay, frequent UI updates

**Performance Test 2: Memory Usage**
- Setup: New game
- Tools: Unity Profiler (Memory)
- Duration: 60 minutes
- Metrics: Track total memory, GC allocations
- Pass Criteria: Memory stays <400 MB, no memory leaks
- Actions: Play, save, load, repeat

**Performance Test 3: Scene Load Time**
- Setup: Time scene transitions
- Tools: Stopwatch or profiler
- Scenarios: MainMenu → GameSetup, GameSetup → GameBoard, GameBoard → MainMenu
- Pass Criteria: Each transition <3 seconds
- Repeat: 10 times each

**Performance Test 4: Draw Calls**
- Setup: Game board scene
- Tools: Unity Stats window
- Metrics: Draw calls, batches
- Pass Criteria: <100 draw calls per frame
- Actions: Normal gameplay with all UI visible

---

### Edge Cases to Test

1. **Invalid Save File**
   - Manually corrupt save file
   - Try to load
   - Expected: Error message, game doesn't crash

2. **Insufficient Funds for Action**
   - Try to buy property with $0
   - Try to build house without enough money
   - Expected: Action disabled or error message

3. **Bankruptcy with No Assets**
   - Player with $0 and no properties must pay rent
   - Expected: Immediate bankruptcy, no errors

4. **All Players Except One Bankrupt**
   - Play until 5 of 6 players bankrupt
   - Expected: Game over, winner declared

5. **Maximum Players (6)**
   - Start game with 6 players
   - Expected: All players fit on board and in UI

6. **Minimum Players (2)**
   - Start game with 2 players
   - Expected: Game works correctly

7. **Long Player Names**
   - Enter 50-character player name
   - Expected: Name truncated or wraps appropriately

8. **Special Characters in Player Names**
   - Use emoji, unicode, special chars
   - Expected: Handled gracefully or rejected with message

9. **Window Minimized During Game**
   - Minimize window for 5 minutes
   - Restore
   - Expected: Game state preserved, no errors

10. **Alt+Tab During Modal**
    - Open property card modal
    - Alt+Tab away
    - Return
    - Expected: Modal still present and functional

---

## 7. Examples and References

### Similar Implementations

#### Tabletop Simulator (Unity Board Game)
**UI Approach:**
- Clean, minimal UI overlays on 3D board
- Context menus on right-click
- Hotkeys for common actions
- Smooth camera controls

**Lessons:**
- Keep UI non-intrusive (semi-transparent)
- Provide multiple ways to do same action (click, hotkey, menu)
- Clear visual feedback for all actions

#### Catan Universe (Unity Board Game)
**UI Approach:**
- Card-based UI for resources and actions
- Clear turn indicator (glowing border on active player)
- Property details on hover
- Trade interface with drag-and-drop

**Lessons:**
- Visual hierarchy (most important info largest)
- Color coding for player differentiation
- Smooth transitions between turn phases
- Tutorial overlays to guide new players (Phase 5 for us)

#### Monopoly Plus (Official Monopoly Game)
**UI Approach:**
- 3D board with 2D UI overlays
- Property cards as floating panels
- Dice rolling animation (3D physics)
- Player avatars with animations

**Lessons:**
- Balance realism with clarity (our Phase 2 is clarity-focused)
- Audio feedback for every action
- Clear visual distinction between turn phases
- Celebrate wins and milestones (sound, visuals)

---

### Unity UI Best Practices

#### From Unity Documentation
1. **Canvas Scaler**: Use "Scale with Screen Size" for multi-resolution support
2. **Sprite Atlasing**: Batch UI sprites to reduce draw calls
3. **Anchors and Pivots**: Proper anchor setup for responsive design
4. **Layout Groups**: Use for dynamic layouts (player lists)
5. **Object Pooling**: For frequently created/destroyed UI elements (notifications)

#### From Successful Unity Games
1. **Hierarchy**: Keep UI hierarchy shallow (performance)
2. **Prefabs**: Modular UI prefabs for reusability
3. **EventSystem**: Single EventSystem per scene
4. **Raycasting**: Disable raycast on non-interactive elements
5. **Rich Text**: Use TextMeshPro for better text rendering

---

### Board Game UI Patterns

#### Property Visualization Patterns
- **Classic Board Style** (our choice for Phase 2):
  - 2D top-down view
  - Square board with spaces around perimeter
  - Properties as rectangles with color bars
  - Clear, legible text
  - Example: Monopoly Plus, Monopoly Game by Marmalade

- **Isometric View** (future consideration):
  - 3D-looking board with isometric perspective
  - Properties as 3D buildings
  - More visually interesting but potentially less clear
  - Example: Monopoly Streets

- **Simplified List View**:
  - Board as schematic or list
  - Focus on information over visuals
  - Good for accessibility
  - Example: Some mobile Monopoly apps

#### Turn Indicator Patterns
- **Glowing Border**: Active player panel has glowing border (our choice)
- **Spotlight**: Light shines on current player's position
- **Banner**: "Your Turn!" banner across top
- **Color Shift**: Entire UI tints to current player's color

---

### UI Mockup References (Conceptual Descriptions)

#### Main Menu Mockup
```
┌─────────────────────────────────────┐
│                                     │
│         MONOPOLY FRENZY            │
│         ───────────────            │
│                                     │
│         [   New Game    ]          │
│         [   Load Game   ]          │
│         [   Settings    ]          │
│         [     Quit      ]          │
│                                     │
│                         v1.0.0      │
└─────────────────────────────────────┘
```

#### Game Board Layout
```
┌────────────────────────────────────────────────────────────┐
│ [Pause]                                  [Current Turn: P1]│
├────────┬───────────────────────────────────────┬───────────┤
│        │ ┌───┬───┬───┬───┬───┬───┬───┬───┬───┐│           │
│ Player │ │GO │   │   │   │   │   │   │   │JL ││  Dice:    │
│ List:  │ ├───┤                           ├───┤│  ┌───┬───┐│
│        │ │   │       MONOPOLY BOARD      │   ││  │ 4 │ 3 ││
│ 1. P1  │ │   │                           │   ││  └───┴───┘│
│ 2. P2  │ │   │     (Properties with      │   ││   Total: 7│
│ 3. P3  │ │   │      tokens displayed)    │   ││           │
│ 4. P4  │ │   │                           │   ││ [Roll]    │
│        │ ├───┤                           ├───┤│ [Buy]     │
│ Money: │ │FP │   │   │   │   │   │   │   │GJ ││ [Trade]   │
│ $1,245 │ └───┴───┴───┴───┴───┴───┴───┴───┴───┘│ [Build]   │
│        │                                       │ [End Turn]│
│ Props: │  Message Log:                        │           │
│ • Med  │  - You rolled 7                      │           │
│ • Balt │  - Moved to Baltic Ave                         │
│        │  - You purchased Baltic Ave          │           │
└────────┴───────────────────────────────────────┴───────────┘
```

#### Property Card Modal
```
┌──────────────────────────┐
│  Mediterranean Avenue     │
│  ─────────────────────   │
│  Color: Brown             │
│  Price: $60               │
│                           │
│  Rent: $2                 │
│  1 House: $10             │
│  2 Houses: $30            │
│  3 Houses: $90            │
│  4 Houses: $160           │
│  Hotel: $250              │
│                           │
│  House Cost: $50          │
│  Mortgage Value: $30      │
│                           │
│  [  Buy $60  ] [ Decline ]│
└──────────────────────────┘
```

---

## 8. Dependencies and Prerequisites

### Must Be Complete First

#### Phase 1 Components (85% complete)
- [x] GameState management
- [x] Board with all 40 spaces
- [x] Player management
- [x] Property system
- [x] Command Pattern (7/10 commands)
- [x] State Machine
- [x] Event System
- [ ] **BankruptcyHandler** - Required for Phase 2 bankruptcy UI
- [ ] **JailRules** - Required for Phase 2 jail UI
- [ ] **Card System** - Required for Phase 2 Chance/Community Chest
- [ ] **TradeCommand** - Required for Phase 2 trade UI

**Action**: Complete Phase 1 remaining items (estimated 10-12 hours) before starting Phase 2 UI.

---

### Required Assets

#### Graphics Assets
- **Property Space Sprites**: 40 individual property graphics (or template with color variations)
- **Player Token Sprites**: 8 classic Monopoly tokens (car, hat, ship, dog, thimble, iron, shoe, wheelbarrow)
- **UI Elements**: Buttons (normal, hover, pressed), borders, backgrounds, panels
- **Icons**: Dice, house, hotel, jail bars, money, chance, community chest, railroad, utility
- **Board Background**: Classic Monopoly board aesthetic or modern variant

**Sources:**
- Create custom assets (recommended for unique look)
- Use public domain Monopoly-style graphics
- Purchase from Unity Asset Store (UI packs)

#### Fonts
- **Title Font**: Bold, clear font for headings (e.g., Roboto Bold, Arial Bold)
- **Body Font**: Readable font for all text (e.g., Roboto Regular, Arial)
- Use TextMeshPro for better rendering

#### Audio (Phase 5)
- Not required for Phase 2, but prepare hooks in code

---

### Technical Prerequisites

#### Unity Version
- Unity 2022 LTS (2022.3.x) - as per ADR-001
- TextMesh Pro package (comes with Unity)

#### Packages
- Universal RP (optional, for better rendering)
- Unity UI (built-in)
- Input System (New Input System recommended, or Legacy Input)

#### Development Environment
- Visual Studio 2022 or Rider
- Git for version control
- Unity Profiler for performance testing

#### Hardware (Target Minimum Spec)
- Intel Core i5-8400 or equivalent
- NVIDIA GTX 1050 Ti or equivalent
- 8 GB RAM
- Windows 10/11

---

## 9. Implementation Phases

### Week 5: Foundation & Main Screens

**Goal**: Set up UI infrastructure and main menu/setup screens.

**Deliverables:**
- Unity scenes created (MainMenu, GameSetup, GameBoard)
- Main menu functional with navigation
- Game setup screen functional (player configuration)
- Scene management working
- Basic styling and layout

**Tasks:**
1. Create Unity project structure (scenes, folders)
2. Set up Canvas with Canvas Scaler
3. Import or create basic UI assets (buttons, backgrounds)
4. Implement MainMenuScreen
5. Implement GameSetupScreen with player configuration
6. Implement scene transitions (LoadScene async)
7. Create UIController singleton for global UI management
8. Test navigation flow (menu → setup → back)

**Estimated Effort**: 40 hours (1 full-time developer for 1 week)

**Acceptance Criteria:**
- [ ] Can navigate from main menu to game setup and back
- [ ] Can configure 2-6 players with names and tokens
- [ ] No duplicate tokens or names
- [ ] "Start Game" button transitions to game board (even if empty)

---

### Week 6: Game Board & Core Gameplay

**Goal**: Implement game board visualization and basic turn controls.

**Deliverables:**
- Complete game board with all 40 spaces
- Player tokens on board
- Turn controls (roll dice, end turn)
- Player HUD (money, properties)
- Basic property purchase flow

**Tasks:**
1. Create MonopolyBoard prefab with 40 PropertySpace prefabs
2. Implement PropertySpace component (display name, color, ownership)
3. Implement PlayerToken component (positioning, highlighting)
4. Create BoardController (manages board state, token positions)
5. Implement TurnControlPanel (roll dice, end turn buttons)
6. Implement PlayerStatusPanel (money, properties, status)
7. Integrate with Phase 1 Commands (RollDiceCommand, BuyPropertyCommand)
8. Subscribe UI components to EventBus events
9. Test basic turn flow (roll → move → buy → end turn)

**Estimated Effort**: 40 hours

**Acceptance Criteria:**
- [ ] All 40 spaces visible and labeled
- [ ] Can roll dice and token moves correctly
- [ ] Can buy property when landing on it
- [ ] Money and properties update in UI
- [ ] Can end turn and next player's turn starts

---

### Week 7: Property Management & Advanced Actions

**Goal**: Implement property details, building, mortgaging, and trading.

**Deliverables:**
- Property card modal with full details
- Build houses/hotels UI
- Mortgage/unmortgage UI
- Trade dialog (basic version)
- Jail mechanics UI
- Chance/Community Chest card display

**Tasks:**
1. Implement PropertyCardModal (detailed property info)
2. Implement BuildHousesModal (select property, number of houses)
3. Integrate with BuyHouseCommand from Phase 1
4. Implement Mortgage/Unmortgage buttons in PropertyCardModal
5. Integrate with MortgageCommand from Phase 1
6. Implement TradeModal (two-panel interface)
7. Integrate with TradeCommand from Phase 1 (complete if not done)
8. Implement Jail UI (options: pay, use card, roll doubles)
9. Integrate with JailRules from Phase 1 (complete if not done)
10. Implement CardDrawModal for Chance/Community Chest
11. Integrate with DrawCardCommand from Phase 1 (complete if not done)
12. Test all property management actions

**Estimated Effort**: 40 hours

**Acceptance Criteria:**
- [ ] Can view any property's full details
- [ ] Can build houses evenly on monopolies
- [ ] Can mortgage and unmortgage properties
- [ ] Can propose and accept trades
- [ ] Jail mechanics work (pay, card, roll doubles)
- [ ] Chance/Community Chest cards display and execute

---

### Week 8: Polish, Save/Load, & Testing

**Goal**: Complete remaining UI features, implement save/load, and comprehensive testing.

**Deliverables:**
- Save/load game functionality
- Settings screen
- Pause menu
- Bankruptcy UI
- Game over screen
- Final polish and bug fixes
- Comprehensive testing

**Tasks:**
1. Implement SaveGameModal (prompt for name, save to disk)
2. Implement LoadGameScreen (list saves, load selected)
3. Integrate with Phase 1 GameState serialization
4. Implement SettingsScreen (audio, display, gameplay settings)
5. Implement PauseMenu (resume, save, settings, quit)
6. Implement BankruptcyModal (asset liquidation interface)
7. Integrate with BankruptcyHandler from Phase 1 (complete if not done)
8. Implement GameOverScreen (winner display, stats, return to menu)
9. Implement keyboard navigation for all screens
10. Implement responsive design (test multiple resolutions)
11. Unit test all UI components
12. Integration test complete game flows
13. Performance test (profiling, optimization)
14. Playtest with 3-5 users
15. Fix all critical bugs
16. Polish UI visuals (consistent styling, colors, fonts)

**Estimated Effort**: 40 hours

**Acceptance Criteria:**
- [ ] Can save and load games with full state preservation
- [ ] Settings functional and persist between sessions
- [ ] Pause menu accessible and works
- [ ] Bankruptcy process completes correctly
- [ ] Game over declared when one player remains
- [ ] Keyboard navigation works throughout
- [ ] UI responsive from 1280x720 to 4K
- [ ] Zero crashes during 30-minute playtest
- [ ] All Phase 2 user stories complete

---

## 10. Open Questions

- [ ] **Asset Style**: Should we use realistic Monopoly visuals or a more stylized/modern aesthetic?
- [ ] **Color Palette**: Classic Monopoly colors or custom theme?
- [ ] **Zoom Feature**: Should players be able to zoom in/out on the board, or fixed view?
- [ ] **Message Log**: How many recent messages to display? Scrollable or fixed?
- [ ] **Notification Style**: Toasts (auto-dismiss) or modals (manual dismiss)?
- [ ] **Property Card Trigger**: Click property space, or dedicated button, or both?
- [ ] **Trade Counter-Offers**: In scope for Phase 2 or defer to later?
- [ ] **Hotkeys Customization**: Allow players to customize keyboard shortcuts in settings?
- [ ] **Multi-Monitor Support**: Should we support multi-monitor setups?
- [ ] **Aspect Ratio Edge Cases**: How to handle very unusual aspect ratios (e.g., vertical monitors)?

---

## 11. Assumptions

### Assumptions for Phase 2

1. **Phase 1 Completion**: Assume Phase 1 will be 100% complete before starting Phase 2 (currently 85%).
2. **Single Unity Developer**: Plan assumes 1 full-time Unity developer for UI implementation.
3. **Assets Available**: Basic UI assets (buttons, icons, property graphics) will be available or created by Week 5.
4. **No 3D Graphics**: Phase 2 uses 2D UI only; 3D board is out of scope.
5. **No Animations**: Minimal or no animations in Phase 2; full animations in Phase 5.
6. **No Audio**: No sound effects or music in Phase 2; audio in Phase 5.
7. **No Tutorial**: Players expected to know Monopoly rules; tutorial in Phase 5.
8. **PC Only**: Target Windows PC; no mobile or console considerations yet.
9. **Local Multiplayer Only**: Single device, pass-and-play style; no online multiplayer yet.
10. **Unity 2022 LTS**: Using Unity 2022 LTS as per ADR-001.
11. **TextMesh Pro**: Using TMP for all text rendering.
12. **New Input System**: Using Unity's new Input System (or Legacy if team prefers).
13. **No Third-Party UI Frameworks**: Using Unity's built-in UI system, not external frameworks (e.g., NGUI, FairyGUI).
14. **English Only**: UI text in English; localization in future phases or post-launch.
15. **60 FPS Target**: Maintaining 60 FPS on target hardware (i5-8400, GTX 1050 Ti, 8GB RAM).

---

## 12. Risks and Mitigation

| Risk | Impact | Likelihood | Mitigation Strategy |
|------|--------|------------|---------------------|
| **Phase 1 not complete in time** | High | Medium | Prioritize Phase 1 completion before starting Phase 2 UI. Run Phase 1 and 2 in parallel if necessary (UI mocks while logic finishes). |
| **UI performance issues** (low FPS) | High | Medium | Profile early (Week 6). Use sprite atlasing, reduce draw calls, optimize layouts. Test on min spec hardware. |
| **Responsive design complexity** | Medium | High | Use Canvas Scaler and proper anchors from start. Test multiple resolutions weekly. |
| **UI/Logic integration bugs** | High | Medium | Strict separation: UI never modifies GameState directly. All actions via Commands. Comprehensive integration tests. |
| **Scope creep** (too much polish in Phase 2) | Medium | High | Defer animations, audio, and tutorial to Phase 5. Focus on functionality only. |
| **Asset creation delays** | Medium | Medium | Use placeholder assets (solid colors, text labels) until final assets ready. |
| **Trade UI complexity** | Medium | Medium | Start with basic trade dialog. Advanced features (counter-offers, history) can be added later. |
| **Keyboard navigation not working** | Low | Low | Test keyboard nav early (Week 6) and fix issues immediately. |
| **Save/load data corruption** | High | Low | Robust validation, versioning, and error handling. Test extensively. Backups for saves. |
| **Usability issues** (confusing UI) | Medium | Medium | Playtest early and often (starting Week 7). Iterate based on feedback. |

---

## 13. Acceptance Criteria Summary

**Phase 2 is complete when:**

### Core Functionality
- [ ] All user stories 2.1 - 2.25 acceptance criteria met
- [ ] Can play complete Monopoly game from start to finish with 2-6 human players
- [ ] All Monopoly rules accessible and functional through UI
- [ ] Save and load preserves complete game state accurately

### Performance
- [ ] 60 FPS maintained throughout gameplay on target hardware
- [ ] Scene transitions complete in <3 seconds
- [ ] Memory usage stays <400 MB
- [ ] Zero UI-related crashes during 30-minute stress test

### Usability
- [ ] UI responsive to window resizing (1280x720 to 4K)
- [ ] Keyboard navigation functional for all critical actions
- [ ] All UI elements accessible within 3 clicks from any screen
- [ ] Clear feedback for all user actions (visual or text)
- [ ] Error messages clear and actionable

### Integration
- [ ] All UI components properly subscribed to EventBus events
- [ ] All user actions execute via Command Pattern
- [ ] GameState is single source of truth (UI never modifies directly)
- [ ] No desynchronization between UI and game logic

### Testing
- [ ] All unit tests pass (UI components)
- [ ] All integration tests pass (turn flow, property purchase, trade, save/load, bankruptcy)
- [ ] All manual test cases completed successfully
- [ ] Performance tests meet criteria (FPS, memory, load times)
- [ ] 30-minute playtest completed with 4 players, zero crashes, zero confusion

### Code Quality
- [ ] All public methods have XML documentation
- [ ] Code follows Unity C# conventions
- [ ] No warnings or errors in console
- [ ] Code reviewed and approved
- [ ] Separation of concerns maintained (UI, Controllers, Game Logic)

### Documentation
- [ ] UI architecture documented
- [ ] Integration with Phase 1 logic documented
- [ ] Known issues and workarounds documented (if any)
- [ ] Future enhancements list created (for Phase 5 polish)

---

## References

### Internal Documents
- [Phase 1 Implementation Plan](./IMPLEMENTATION-PLAN.md)
- [Master Roadmap](./MASTER-ROADMAP.md)
- [Architecture Summary](../specifications/ARCHITECTURE-SUMMARY.md)
- [ADR-002: Game State Management](../specifications/decisions/adr-002-game-state-management.md)
- [State Machine Pattern](../specifications/patterns/state-machine-pattern.md)
- [Command Pattern](../specifications/patterns/command-pattern.md)

### Unity Documentation
- [Unity UI User Guide](https://docs.unity3d.com/Manual/UISystem.html)
- [Canvas Scaler](https://docs.unity3d.com/Manual/script-CanvasScaler.html)
- [TextMesh Pro](https://docs.unity3d.com/Manual/com.unity.textmeshpro.html)
- [Unity Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/index.html)

### External Resources
- [Unity UI Best Practices](https://unity.com/how-to/unity-ui-optimization-tips)
- [Board Game UI Design](https://www.gamedeveloper.com/design/designing-board-game-uis-for-digital-platforms)

---

**Document Version**: 1.0  
**Last Updated**: 2026-02-18  
**Next Review**: Weekly during Phase 2 implementation (Weeks 5-8)  
**Status**: ✅ Approved - Ready for Implementation

---

## Revision History

| Date | Version | Changes | Author |
|------|---------|---------|--------|
| 2026-02-18 | 1.0 | Initial Phase 2 implementation plan created | Senior Business Analyst Agent |
