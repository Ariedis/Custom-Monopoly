# Digital Board Game Architecture Research

**Date**: 2026-02-16  
**Status**: Approved  
**Related Documents**: 
- [System Overview](../architecture/monopoly-frenzy-system-overview.md)
- [ADR-001: Technology Stack Selection](../decisions/adr-001-technology-stack-selection.md)

## Overview

This document provides research findings on successful digital board game implementations, their architectural patterns, and lessons learned. This research informs the architecture of Monopoly Frenzy.

## Successful Digital Board Game Examples

### 1. Tabletop Simulator (Berserk Games)

**Platform**: Windows, macOS, Linux  
**Technology**: Unity Engine (C#)  
**Architecture Highlights**:
- Flexible mod system using Lua scripting
- Physics-based interaction model
- State synchronization for multiplayer
- Asset streaming and dynamic loading

**Key Lessons**:
- ✅ Robust mod support increases longevity and community engagement
- ✅ Physics simulation adds immersion but needs performance optimization
- ✅ Clear separation between game rules and presentation layer
- ⚠️ Complex physics can introduce bugs and edge cases

### 2. Ticket to Ride (Days of Wonder)

**Platform**: Windows, iOS, Android, Steam  
**Technology**: Custom C++ engine  
**Architecture Highlights**:
- Turn-based state machine
- Cross-platform shared core logic
- Platform-specific UI layers
- Robust save/load system

**Key Lessons**:
- ✅ Shared core logic enables multi-platform deployment
- ✅ Clear separation of game rules from UI
- ✅ Deterministic game state for reliable save/load
- ✅ AI implemented as separate, pluggable components

### 3. Monopoly Plus (Ubisoft)

**Platform**: Windows, PlayStation, Xbox  
**Technology**: UbiArt Framework  
**Architecture Highlights**:
- 3D animated board with camera system
- Rule variants system
- Local and online multiplayer
- Telemetry and analytics integration

**Key Lessons**:
- ✅ Animation system separated from game logic
- ✅ Rule engine supports variants and house rules
- ✅ Comprehensive tutorial and onboarding system
- ⚠️ Complex visuals can overshadow gameplay
- ⚠️ Online features require significant infrastructure

### 4. Catan Universe (USM)

**Platform**: Windows, Browser, Mobile  
**Technology**: Unity (C#)  
**Architecture Highlights**:
- Event-driven architecture
- AI using minimax and Monte Carlo algorithms
- Cross-platform multiplayer with matchmaking
- Extensive rule validation system

**Key Lessons**:
- ✅ Event-driven design enables replay and undo features
- ✅ Separate AI difficulty levels by adjusting evaluation depth
- ✅ Client-side validation with server authority for multiplayer
- ✅ Progressive disclosure UI for complex rules

### 5. Armello (League of Geeks)

**Platform**: Windows, PlayStation, Xbox, Mobile  
**Technology**: Unity (C#)  
**Architecture Highlights**:
- Card-based action system
- Turn-based with asynchronous multiplayer
- Rich animation and VFX system
- Comprehensive mod support

**Key Lessons**:
- ✅ Command pattern for all player actions enables undo/replay
- ✅ Asynchronous turn-based allows play-by-email style gaming
- ✅ Animation queue system prevents blocking gameplay
- ✅ Telemetry tracks player behavior for balancing

## Common Architectural Patterns

### 1. State Machine Pattern

**Usage**: Game flow control (Menu → Setup → Playing → End Game)

**Benefits**:
- Clear state transitions
- Easy to debug and visualize
- Supports save/load at any state
- Prevents invalid state combinations

**Implementation Notes**:
- Each state is a separate class/module
- State context maintains shared game data
- Transitions triggered by events or commands

**Examples**:
- Ticket to Ride uses state machine for turn phases
- Catan Universe uses hierarchical state machine for game → turn → action states

### 2. Command Pattern

**Usage**: Player actions, undo/redo, network synchronization

**Benefits**:
- All actions are first-class objects
- Easy to implement undo/redo
- Network replay by sending command sequence
- Enables AI to evaluate potential actions

**Implementation Notes**:
- Each action (move, buy, trade) is a command
- Commands encapsulate all parameters
- Command history enables undo/redo

**Examples**:
- Armello uses commands for all player actions
- Tabletop Simulator uses commands for object manipulation

### 3. Observer/Event Pattern

**Usage**: UI updates, game event notifications, achievement tracking

**Benefits**:
- Loose coupling between game logic and presentation
- Easy to add new observers (UI, sound, effects)
- Centralized event bus for debugging

**Implementation Notes**:
- Game logic publishes events
- UI/audio/effects subscribe to relevant events
- Events carry all necessary data for observers

**Examples**:
- Catan Universe uses event bus for all game state changes
- Monopoly Plus uses observers for animation triggers

### 4. Model-View-Controller (MVC)

**Usage**: Overall application structure

**Benefits**:
- Clear separation of concerns
- UI can be swapped/restyled independently
- Game logic is testable without UI
- Multiple views (2D/3D) from same model

**Implementation Notes**:
- Model: Game state and rules
- View: Rendering and presentation
- Controller: Input handling and user actions

**Examples**:
- Ticket to Ride uses MVC for cross-platform support
- Tabletop Simulator separates physics model from rendering

### 5. Entity-Component System (ECS)

**Usage**: Game objects with flexible properties

**Benefits**:
- Flexible composition over inheritance
- Performance optimization through data locality
- Easy to add new entity types
- Supports dynamic property modification

**When NOT to Use**:
- Board games with fixed entity types
- Turn-based games without performance concerns
- Small-scale projects with simple entities

**Examples**:
- Some Unity-based board games use ECS
- Better suited for real-time strategy games

## Technology Stack Recommendations

### Programming Languages

#### C# with .NET
**Best For**: Windows desktop applications, Unity games

**Pros**:
- Excellent Windows integration
- Rich ecosystem of libraries
- Strong typing with modern features
- Garbage collection simplifies memory management
- Great debugging tools (Visual Studio)
- Unity support for easy 2D/3D graphics

**Cons**:
- .NET runtime dependency
- Slightly larger memory footprint
- Less control than C++

**Games Using This**: Tabletop Simulator, Catan Universe, most Unity games

#### C++
**Best For**: Performance-critical, AAA-style games

**Pros**:
- Maximum performance and control
- DirectX integration
- No runtime dependency
- Memory control

**Cons**:
- Higher complexity
- Longer development time
- Manual memory management
- More potential for bugs

**Games Using This**: Ticket to Ride, Age of Empires, many Ubisoft games

#### Python
**Best For**: Prototyping, simple games, tools

**Pros**:
- Rapid development
- Easy to learn
- Great for AI algorithms
- Excellent for scripting

**Cons**:
- Performance limitations
- Not ideal for production games
- Distribution challenges on Windows

**Games Using This**: Prototypes, game tools, AI testing

### Game Frameworks/Engines

#### Unity (Recommended for Monopoly Frenzy)
**Best For**: 2D/3D games with mod support

**Pros**:
- Excellent 2D tools for board game graphics
- Built-in animation system
- Asset pipeline for mods
- Large community and resources
- Cross-platform by default
- Visual editor for non-programmers

**Cons**:
- Engine overhead for simple games
- Learning curve for engine-specific workflows
- Licensing costs for revenue sharing

**Why Recommended**: Perfect balance of features, ease of development, and mod support

#### MonoGame
**Best For**: 2D games with full control

**Pros**:
- Lightweight framework
- Full control over architecture
- No engine overhead
- Free and open source
- C# based

**Cons**:
- Need to build more yourself
- Less tooling than Unity
- Steeper learning curve for beginners

#### WPF/WinForms with Custom Rendering
**Best For**: Simple 2D board games

**Pros**:
- Native Windows integration
- Simple for 2D graphics
- Good for UI-heavy applications
- .NET ecosystem

**Cons**:
- Limited graphics capabilities
- Not designed for games
- Performance limitations
- No built-in game features

#### Unreal Engine
**Best For**: 3D, graphically intensive games

**Pros**:
- Stunning graphics capabilities
- Blueprint visual scripting
- Comprehensive toolset

**Cons**:
- Overkill for 2D board games
- Large engine size
- Higher system requirements
- Steeper learning curve

## Performance Considerations

### Target Specifications for Board Games

**Minimum Requirements**:
- CPU: Dual-core 2.0 GHz
- RAM: 2 GB
- GPU: Integrated graphics with DirectX 11
- Storage: 500 MB

**Recommended Requirements**:
- CPU: Quad-core 2.5 GHz
- RAM: 4 GB
- GPU: Dedicated graphics with DirectX 11
- Storage: 1 GB

**Performance Targets**:
- 60 FPS during gameplay
- <100ms input latency
- <2 second load times between screens
- <5 second game load from disk

### Optimization Strategies

1. **Asset Loading**:
   - Load assets asynchronously
   - Use sprite atlases for 2D graphics
   - Compress images appropriately
   - Cache loaded assets

2. **Rendering**:
   - Batch draw calls
   - Use dirty rectangles for 2D updates
   - Limit animations to visible elements
   - Use level of detail for 3D elements

3. **Game Logic**:
   - Separate update rates (logic vs rendering)
   - Cache computed values
   - Use efficient data structures
   - Minimize allocations in hot paths

4. **Memory Management**:
   - Object pooling for frequent allocations
   - Unload unused assets
   - Profile memory usage regularly
   - Consider memory budgets per subsystem

## Mod Support Best Practices

### Asset-Based Mods
**Approach**: Allow replacement of art and data files

**Implementation**:
- Define standard folder structure
- Support common formats (PNG, JPG, JSON)
- Hot-reload during development
- Validation of mod files

**Examples**:
- Tabletop Simulator's asset bundles
- Catan Universe's custom boards

### Script-Based Mods
**Approach**: Allow custom game logic

**Implementation**:
- Embedded scripting language (Lua, Python)
- Sandboxed execution environment
- API documentation
- Mod conflict resolution

**Examples**:
- Tabletop Simulator's Lua scripting
- Garry's Mod's Lua system

### For Monopoly Frenzy (Recommended)
**Hybrid Approach**:
- Asset-based for visual customization
- Data-driven for game rules
- Preset system for combinations
- No scripting initially (simplicity)

**Benefits**:
- Easier to implement
- More stable (no arbitrary code)
- Sufficient for card/property customization
- Can add scripting later if needed

## AI Implementation Strategies

### Rule-Based AI (Easy)
**Approach**: Simple heuristics and rules

**Implementation**:
- Priority-based decision making
- Fixed thresholds for actions
- Deterministic behavior

**Performance**: Very fast, minimal CPU

### Minimax with Alpha-Beta Pruning (Medium)
**Approach**: Look ahead several turns

**Implementation**:
- Evaluate future game states
- Limited search depth
- Position evaluation function

**Performance**: Moderate CPU, scales with depth

### Monte Carlo Tree Search (Hard)
**Approach**: Simulate many random games

**Implementation**:
- Play out random games from current state
- Track win rates for actions
- Best action is highest win rate

**Performance**: CPU intensive, adjustable iterations

### Hybrid Approach (Recommended)
**Implementation**:
- Easy: Pure rule-based
- Medium: Rule-based with limited lookahead
- Hard: MCTS with time limits

**Benefits**:
- Scalable difficulty
- Predictable performance
- Each difficulty builds on previous

## Multiplayer Architecture

### Hot-Seat (Local Multiplayer)
**Implementation**: Single device, players take turns

**Considerations**:
- Hide opponent information between turns
- Quick player switching UI
- Prevent accidental reveals
- Save/resume support

**Best For**: Monopoly Frenzy initial release

### Network Multiplayer
**Architectures**:

1. **Peer-to-Peer**:
   - No server costs
   - Direct connections
   - Harder synchronization
   - Trust issues (cheating)

2. **Client-Server**:
   - Authoritative server
   - Easier to prevent cheating
   - Server hosting costs
   - More complex infrastructure

3. **Relay Server**:
   - Server relays commands
   - Minimal server logic
   - Lower costs than full server
   - Some trust issues remain

**Recommendation**: Start with hot-seat, add network later if needed

## Save/Load System Architecture

### Full State Serialization
**Approach**: Save entire game state

**Pros**:
- Simple to implement
- Guaranteed consistent state
- Works with any game state

**Cons**:
- Large save files
- Can break with game updates
- Slow for large states

### Event Sourcing
**Approach**: Save sequence of events/commands

**Pros**:
- Small save files
- Built-in replay functionality
- Time-travel debugging
- Can validate game progression

**Cons**:
- Must replay all events to restore state
- Events must be deterministic
- Version management complexity

### Incremental Snapshots
**Approach**: Periodic full state + recent events

**Pros**:
- Fast load times
- Supports undo/redo
- Reasonable file sizes

**Cons**:
- More complex implementation
- Need to manage snapshot frequency

**Recommendation for Monopoly Frenzy**: Full state serialization (simplicity) with JSON format

## User Interface Design Patterns

### Screen Navigation
**Pattern**: State-based navigation with stack

**Implementation**:
- Each screen is a state
- Navigation stack for back button
- Transition animations
- Persistent data between screens

### Game Board Representation
**Options**:

1. **Fixed Camera 2D**: Top-down view, no camera movement
   - Simplest implementation
   - Traditional board game feel
   - Limited visual flair

2. **Scrollable/Zoomable 2D**: Pan and zoom support
   - More flexibility
   - Better for large boards
   - Needs camera controls

3. **3D with Camera**: Animated 3D board
   - Most visually impressive
   - Complex implementation
   - Higher system requirements

**Recommendation**: Fixed or scrollable 2D for simplicity and performance

### Animation Strategy
**Approach**: Animation queue system

**Implementation**:
- Queue animations for sequential play
- Allow skip/speed-up options
- Non-blocking for UI updates
- Separate animation from game logic

**Examples**:
- Monopoly Plus queues piece movement
- Catan Universe allows animation skip

## Testing Strategies

### Unit Testing
**Focus**: Game rule validation

**Tools**: NUnit (C#), Google Test (C++)

**Coverage**:
- Rule enforcement
- State transitions
- Command execution
- AI decision making

### Integration Testing
**Focus**: Component interactions

**Coverage**:
- UI to game logic
- Save/load functionality
- Mod loading
- State management

### Playtesting
**Focus**: User experience and balance

**Methods**:
- AI vs AI automated testing
- Human playtesting sessions
- Telemetry data analysis

## Windows-Specific Considerations

### DirectX Integration
- Use DirectX 11 for wide compatibility
- DirectX 12 if targeting modern hardware
- Consider DirectX fallback chains

### Input Handling
- Support keyboard, mouse, and gamepad
- Configurable controls
- Accessibility options (high contrast, etc.)

### Distribution
**Options**:
1. **Steam**: Largest PC gaming platform
2. **Microsoft Store**: Windows integration
3. **Itch.io**: Indie-friendly platform
4. **Direct Download**: Full control

**Considerations**:
- DRM requirements
- Update mechanisms
- Analytics integration

## Common Pitfalls and Anti-Patterns

### ❌ Tight Coupling Between UI and Logic
**Problem**: UI directly manipulates game state

**Solution**: Use MVC/MVVM pattern with clear boundaries

### ❌ Hardcoded Game Rules
**Problem**: Rules embedded in multiple locations

**Solution**: Centralized rule engine with data-driven configuration

### ❌ Blocking Animations
**Problem**: Game waits for animations to complete

**Solution**: Animation queue with async execution

### ❌ Inadequate Save/Load Testing
**Problem**: Save files corrupt or incompatible

**Solution**: Automated testing of save/load cycle, versioning

### ❌ Poor Mod Validation
**Problem**: Invalid mods crash the game

**Solution**: Robust validation, error handling, fallback to defaults

### ❌ Ignoring Performance Early
**Problem**: Performance issues discovered late

**Solution**: Regular profiling, performance budgets from start

## Recommended Architecture for Monopoly Frenzy

### Technology Stack
- **Language**: C# with .NET 6/7
- **Framework**: Unity 2022 LTS
- **Graphics**: 2D with sprite-based rendering
- **Platform**: Windows 10/11 (64-bit)

### Core Architecture Patterns
1. **State Machine**: Game flow control
2. **Command Pattern**: Player actions and undo/redo
3. **Observer Pattern**: UI updates and events
4. **MVC**: Overall structure
5. **Data-Driven Design**: Mod support

### Key Design Principles
1. **Separation of Concerns**: Game logic independent from presentation
2. **Modularity**: Clear subsystem boundaries
3. **Extensibility**: Support for mods and customization
4. **Performance**: 60 FPS on minimum specs
5. **Testability**: Automated testing for rules and state

## References and Further Reading

### Books
- "Game Programming Patterns" by Robert Nystrom
- "Game Engine Architecture" by Jason Gregory
- "AI for Games" by Ian Millington
- "Multiplayer Game Programming" by Joshua Glazer

### Online Resources
- Unity Learn: Board game tutorials
- Gamasutra Post-Mortems: Digital board games
- Game Developer Conference (GDC) Talks: Board game design
- Microsoft Docs: DirectX and Windows game development

### Case Studies
- Tabletop Simulator: Mod system architecture
- Ticket to Ride: Cross-platform development
- Catan Universe: Multiplayer synchronization
- Armello: Animation and VFX systems

### Technical Articles
- "Building a Turn-Based Game Architecture"
- "Implementing Undo/Redo with Command Pattern"
- "Event-Driven Game Architecture"
- "Optimizing 2D Games in Unity"

## Revision History

| Date | Version | Changes | Author |
|------|---------|---------|--------|
| 2026-02-16 | 1.0 | Initial research document | Software Architect Agent |

