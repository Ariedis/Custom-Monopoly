# Custom Monopoly Agents

This directory contains specialized AI agents designed to assist with different aspects of the Custom Monopoly game development project.

## Available Agents

### 1. Software Architect - Games (`software-architect-games.agent.md`)
**Role**: Expert software architect specializing in Windows game development  
**Responsibilities**:
- Creates high-level architecture documents
- Documents design patterns and best practices
- Creates Architecture Decision Records (ADRs)
- Researches industry best practices from successful games

**Key Constraints**:
- ✅ Creates documentation in `/specifications` directory
- ❌ Does NOT write code
- ❌ Does NOT modify existing code

**Use When**: You need architecture documentation, design decisions, or research on game development patterns.

---

### 2. Senior Business Analyst (`senior-business-analyst.agent.md`)
**Role**: Expert business analyst specializing in Unity game projects  
**Responsibilities**:
- Translates requirements into detailed, actionable plans
- Creates comprehensive documentation for developers
- Breaks down features into user stories and tasks
- Focuses on acceptance criteria and functional requirements

**Key Constraints**:
- ✅ Creates documentation in `/planning` directory
- ❌ Does NOT write code
- ❌ Does NOT create technical specifications

**Use When**: You need to translate high-level requirements into detailed development plans.

---

### 3. Senior Software Developer (`senior-software-developer.agent.md`)
**Role**: Expert C# and Unity game developer  
**Responsibilities**:
- Reviews specifications and planning documents
- Writes clean, maintainable C# code
- Follows Unity best practices and coding standards
- Implements features in `/src` directory

**Key Constraints**:
- ✅ Writes code in `/src` directory only
- ✅ Reviews `/specifications` and `/planning` before coding
- ❌ Does NOT write code outside `/src` directory
- ❌ Does NOT modify specifications or planning documents

**Use When**: You need to implement features based on specifications and plans.

---

### 4. Senior Test Engineer (`senior-test-engineer.agent.md`)
**Role**: Expert QA engineer specializing in C# and Unity test automation  
**Responsibilities**:
- Reviews planning and specifications for acceptance criteria
- Writes comprehensive automated tests
- Runs test suites and analyzes results
- Writes tests to `src/Assets/Tests` directory only

**Key Constraints**:
- ✅ Writes tests in `src/Assets/Tests` directory only
- ✅ Follows Unity Test Framework (NUnit)
- ❌ Does NOT modify source code
- ❌ Does NOT remove or disable failing tests

**Use When**: You need automated tests for features or need to run test suites.

---

### 5. Senior Build Master (`senior-build-master.agent.md`) ⭐ NEW
**Role**: Expert build master specializing in Unity C# game builds  
**Responsibilities**:
- Executes clean, reproducible builds
- Runs automated test suites (Edit Mode and Play Mode)
- Provides clear, actionable feedback on build/test status
- Reports issues without modifying code

**Key Constraints**:
- ✅ Builds Unity projects for Windows
- ✅ Runs complete test suites
- ✅ Reports build and test failures clearly
- ❌ Does NOT modify source code
- ❌ Does NOT modify tests
- ❌ Does NOT remove failing tests
- ❌ Does NOT modify build configuration

**Use When**: You need to:
- Run local or development builds
- Execute automated test suites
- Get build status reports
- Validate code before deployment

---

## Agent Workflow

### Typical Development Flow

1. **Architecture Phase**
   - Use `@software-architect-games` to create architecture documents
   - Documents go to `/specifications` directory

2. **Planning Phase**
   - Use `@senior-business-analyst` to create detailed plans
   - Plans go to `/planning` directory

3. **Development Phase**
   - Use `@senior-software-developer` to implement features
   - Code goes to `/src` directory

4. **Testing Phase**
   - Use `@senior-test-engineer` to write and run tests
   - Tests go to `src/Assets/Tests` directory

5. **Build & Validation Phase** ⭐ NEW
   - Use `@senior-build-master` to build and test
   - Reports build status and test results
   - Identifies issues for developers to fix

### Example Usage Flow

```
User: "We need player movement in our Monopoly game"

Step 1: Architecture
→ @software-architect-games: Create architecture for player movement system
   Result: /specifications/architecture/player-movement-architecture.md

Step 2: Planning
→ @senior-business-analyst: Create implementation plan for player movement
   Result: /planning/player-movement-plan.md

Step 3: Development
→ @senior-software-developer: Implement player movement system
   Result: /src/Assets/Scripts/PlayerMovement.cs

Step 4: Testing
→ @senior-test-engineer: Write tests for player movement
   Result: /src/Assets/Tests/PlayerMovementTests.cs

Step 5: Build & Validate ⭐
→ @senior-build-master: Run development build and test suite
   Result: Build report with test results
```

---

## Build Master Quick Start Guide

### Running a Development Build

```
@senior-build-master Please run a development build with Edit Mode tests
```

The build master will:
1. Clean previous build artifacts
2. Run Edit Mode tests
3. Build development version
4. Report results with metrics

### Running Full Test Suite

```
@senior-build-master Please run the complete test suite (Edit Mode and Play Mode)
```

The build master will:
1. Execute Edit Mode tests
2. Execute Play Mode tests
3. Generate comprehensive test report
4. Report any failures with details

### Running Release Build

```
@senior-build-master Please run a release build with all tests
```

The build master will:
1. Verify clean repository state
2. Run complete test suite
3. Build release version (if tests pass)
4. Report build metrics and status

---

## Agent Communication Patterns

### How Agents Work Together

**Scenario: New Feature Development**

1. **Architect** defines system design → Creates spec document
2. **Business Analyst** reads spec → Creates detailed plan
3. **Developer** reads spec + plan → Implements code
4. **Test Engineer** reads spec + plan → Writes tests
5. **Build Master** → Builds + runs tests → Reports status ⭐

**Scenario: Bug Fix**

1. **Build Master** reports test failure ⭐
2. **Developer** investigates and fixes code
3. **Build Master** re-runs build and tests ⭐
4. **Build Master** confirms fix ⭐

**Scenario: Architecture Decision**

1. **Architect** researches options → Creates ADR
2. **Business Analyst** reads ADR → Updates planning
3. **Developer** reads ADR + plan → Implements according to decision
4. **Build Master** validates implementation through builds ⭐

---

## Best Practices

### When to Use Each Agent

| Task | Recommended Agent |
|------|-------------------|
| System design | Software Architect |
| Feature planning | Business Analyst |
| Code implementation | Software Developer |
| Test writing | Test Engineer |
| **Running builds** ⭐ | **Build Master** |
| **Test execution** ⭐ | **Build Master** |
| **Build validation** ⭐ | **Build Master** |

### Agent Collaboration Tips

1. **Always start with architecture** - Let the architect define the system first
2. **Plan before coding** - Use business analyst to break down features
3. **Test as you go** - Write tests alongside code development
4. **Build frequently** ⭐ - Use build master to validate changes early
5. **Never skip steps** - Each agent builds on previous work

### Common Pitfalls to Avoid

❌ **Don't**: Ask code agents to write specifications  
✅ **Do**: Use architect for specs, then developer for code

❌ **Don't**: Ask build master to fix code issues  
✅ **Do**: Build master reports issues, developer fixes them

❌ **Don't**: Skip planning phase  
✅ **Do**: Always have business analyst create detailed plans

❌ **Don't**: Write code without tests  
✅ **Do**: Use test engineer to create comprehensive test coverage

❌ **Don't**: Deploy without building ⭐  
✅ **Do**: Always use build master to validate before deployment

---

## Directory Structure

```
Custom-Monopoly/
├── .github/
│   └── agents/                    # This directory
│       ├── README.md             # This file
│       ├── software-architect-games.agent.md
│       ├── senior-business-analyst.agent.md
│       ├── senior-software-developer.agent.md
│       ├── senior-test-engineer.agent.md
│       └── senior-build-master.agent.md ⭐ NEW
├── specifications/               # Architecture docs (Architect)
│   ├── architecture/
│   ├── decisions/
│   ├── patterns/
│   └── research/
├── planning/                     # Plans and requirements (Business Analyst)
│   └── [planning documents]
└── src/                         # Source code (Developer & Test Engineer)
    └── Assets/
        ├── Scripts/             # Game code (Developer)
        └── Tests/               # Test code (Test Engineer)
```

---

## Build Master Features ⭐

The new Build Master agent includes:

### Core Capabilities
- ✅ Unity command-line builds (Windows Standalone)
- ✅ Edit Mode test execution
- ✅ Play Mode test execution
- ✅ .NET/C# project builds (if applicable)
- ✅ Test result analysis and reporting
- ✅ Build metrics tracking

### Build Types Supported
- **Development Builds**: Fast builds with debug symbols
- **Release Builds**: Optimized production builds
- **Test Builds**: Builds specifically for test execution

### Reporting Features
- Clear success/failure status
- Detailed test results (passed/failed/total)
- Specific error messages with file locations
- Build metrics (time, size, warnings)
- Investigation suggestions (without fixing)

### Best Practices Implemented
- Clean builds from fresh state
- Full test suite execution
- Never modifies code or tests
- Transparent reporting
- Reproducible processes

---

## Questions?

### For Architecture Questions
→ `@software-architect-games`

### For Planning Questions
→ `@senior-business-analyst`

### For Code Implementation
→ `@senior-software-developer`

### For Testing Questions
→ `@senior-test-engineer`

### For Build and Deployment ⭐
→ `@senior-build-master`

---

## Version History

| Date | Version | Changes |
|------|---------|---------|
| 2026-02-17 | 1.1 | Added Senior Build Master agent |
| 2026-02-16 | 1.0 | Initial agent set (Architect, BA, Developer, Test Engineer) |

---

**Last Updated**: 2026-02-17  
**Maintained By**: Development Team
