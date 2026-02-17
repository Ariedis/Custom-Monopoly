# Build and CI/CD Documentation

## Quick Start

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- Git (for cloning and version control)

### Build Commands

```bash
# Clone the repository
git clone https://github.com/Ariedis/Custom-Monopoly.git
cd Custom-Monopoly

# Restore dependencies
dotnet restore MonopolyFrenzy.slnx

# Build the solution
dotnet build MonopolyFrenzy.slnx --configuration Release

# Run tests
dotnet test MonopolyFrenzy.slnx --configuration Release --verbosity normal

# Build and run specific test suite
dotnet test MonopolyFrenzy.slnx --filter "FullyQualifiedName~GameStateTests" --verbosity normal
```

## Project Structure

```
Custom-Monopoly/
├── .github/
│   └── workflows/
│       ├── ci-cd.yml          # GitHub Actions CI/CD pipeline
│       └── README.md          # CI/CD documentation
├── src/
│   ├── Assets/                # Original Unity-style organization
│   │   ├── Scripts/           # Source C# files
│   │   └── Tests/             # Test files
│   ├── MonopolyFrenzy/        # .NET class library project
│   │   ├── Core/              # Game state, board, player logic
│   │   ├── Commands/          # Command pattern implementations
│   │   ├── Events/            # Event system
│   │   ├── Rules/             # Game rules engine
│   │   ├── StateMachine/      # State machine for game flow
│   │   └── MonopolyFrenzy.csproj
│   └── MonopolyFrenzy.Tests/  # NUnit test project
│       ├── Core/              # Core logic tests
│       ├── Commands/          # Command tests
│       ├── Events/            # Event system tests
│       ├── Rules/             # Rules engine tests
│       ├── StateMachine/      # State machine tests
│       └── MonopolyFrenzy.Tests.csproj
├── MonopolyFrenzy.slnx        # .NET solution file
├── .gitignore                 # Git ignore rules
└── BUILD.md                   # This file
```

## Solution Architecture

### MonopolyFrenzy (Class Library)
The main game logic library containing:
- **Core:** Game state management, board, players, properties
- **Commands:** Action implementations using Command Pattern
- **Events:** Event bus and game event definitions
- **Rules:** Monopoly game rules and rent calculations
- **StateMachine:** Game flow and turn state management

**Key Features:**
- Pure C# implementation (no Unity dependencies)
- Fully testable in isolation
- JSON serializable for save/load functionality
- Designed for future Unity integration

### MonopolyFrenzy.Tests (NUnit Test Project)
Comprehensive test suite covering:
- 182 unit tests total
- Test-Driven Development (TDD) approach
- Currently 65 passing tests (implementation in progress)
- Targeting 85%+ code coverage

**Test Categories:**
- `GameStateTests` - Game state management (35+ tests)
- `StateMachineTests` - State machine behavior (25+ tests)
- `CommandTests` - Command pattern implementation (50+ tests)
- `RulesEngineTests` - Game rules validation (80+ tests)
- `EventSystemTests` - Event system functionality (40+ tests)

## CI/CD Pipeline

### Automated Workflows

The project uses GitHub Actions for continuous integration and deployment:

**Trigger Events:**
- Push to `main` branch
- Pull requests to `main` branch
- Manual workflow dispatch

### Pipeline Jobs

#### 1. Build and Test (Runs on all events)
```yaml
Steps:
1. Checkout code
2. Setup .NET 8.0
3. Restore NuGet packages
4. Build in Release configuration
5. Run all tests with coverage
6. Upload test results and coverage
7. Publish test report
```

**Artifacts Generated:**
- Test results (.trx format)
- Code coverage reports (Cobertura XML)

#### 2. Package (Runs only on main branch)
```yaml
Steps:
1. Checkout code
2. Setup .NET 8.0
3. Build application
4. Publish binaries
5. Create build info metadata
6. Package as .tar.gz archive
7. Upload artifact (30-day retention)
```

**Artifacts Generated:**
- `monopoly-frenzy-build` - Packaged application binaries

### Viewing Build Results

**In GitHub:**
1. Navigate to the "Actions" tab
2. Select a workflow run
3. View job logs and artifacts
4. Download test results or build packages

**Test Results:**
- Available in the workflow summary
- Downloadable as artifacts
- Includes pass/fail status for each test

## Development Workflow

### Making Changes

```bash
# Create a feature branch
git checkout -b feature/my-feature

# Make your changes
# ... edit files ...

# Build and test locally
dotnet build MonopolyFrenzy.slnx --configuration Debug
dotnet test MonopolyFrenzy.slnx --configuration Debug

# Commit and push
git add .
git commit -m "Add my feature"
git push origin feature/my-feature

# Create pull request
# The CI/CD pipeline will automatically run tests
```

### Running Specific Tests

```bash
# Run tests for a specific class
dotnet test --filter "FullyQualifiedName~GameStateTests"

# Run tests for a specific namespace
dotnet test --filter "FullyQualifiedName~MonopolyFrenzy.Tests.Core"

# Run a specific test method
dotnet test --filter "FullyQualifiedName~Initialize_CreatesDefaultGameState"
```

### Code Coverage

```bash
# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Coverage reports are in:
# src/MonopolyFrenzy.Tests/TestResults/{guid}/coverage.cobertura.xml

# View coverage with ReportGenerator (install separately)
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
```

## Build Configuration

### Release Build
Optimized for performance, no debug symbols:
```bash
dotnet build MonopolyFrenzy.slnx --configuration Release
```

### Debug Build
Includes debug symbols, better for development:
```bash
dotnet build MonopolyFrenzy.slnx --configuration Debug
```

## Dependencies

The project uses the following NuGet packages:

### MonopolyFrenzy
- `Newtonsoft.Json` (13.0.4) - JSON serialization

### MonopolyFrenzy.Tests
- `NUnit` (3.14.0) - Test framework
- `NUnit3TestAdapter` (4.5.0) - Visual Studio test adapter
- `NUnit.Analyzers` (3.9.0) - Code analysis for tests
- `Microsoft.NET.Test.Sdk` (17.8.0) - Test SDK
- `coverlet.collector` (6.0.0) - Code coverage collection

## Troubleshooting

### Build Failures

**Problem:** Build fails with missing dependencies
```bash
# Solution: Restore packages explicitly
dotnet restore MonopolyFrenzy.slnx --force
dotnet nuget locals all --clear
dotnet restore MonopolyFrenzy.slnx
```

**Problem:** Build fails with compilation errors
```bash
# Solution: Clean and rebuild
dotnet clean MonopolyFrenzy.slnx
dotnet build MonopolyFrenzy.slnx --configuration Release
```

### Test Failures

**Problem:** Tests fail unexpectedly
```bash
# Solution: Run tests with detailed output
dotnet test MonopolyFrenzy.slnx --verbosity detailed

# Or run specific failing test
dotnet test --filter "FullyQualifiedName~FailingTestName" --verbosity detailed
```

**Note:** Some test failures are expected during Phase 1 development as this is a TDD project where tests were written before implementation.

### CI/CD Issues

**Problem:** GitHub Actions workflow fails
1. Check the workflow logs in the Actions tab
2. Look for error messages in the failed step
3. Reproduce the issue locally using the same commands
4. Verify all files are committed and pushed

**Problem:** Artifacts not generated
1. Ensure code is merged to `main` branch
2. Verify `build-and-test` job passed
3. Check `package` job logs for errors

## Performance

### Build Times (Typical)
- **Restore:** ~5 seconds
- **Build:** ~5-10 seconds
- **Test:** ~5-10 seconds
- **Total CI Pipeline:** ~30-60 seconds

### Test Execution
- **Edit Mode Tests:** Fast (<1 second per test)
- **Total Test Suite:** ~400ms for 182 tests

## Future Enhancements

### Planned Improvements
- [ ] Unity project integration
- [ ] Unity build pipeline for Windows executable
- [ ] Automated deployment to testing environment
- [ ] Performance benchmarking in CI
- [ ] Security scanning (CodeQL, dependency checks)
- [ ] Docker containerization
- [ ] Semantic versioning automation
- [ ] Windows installer generation
- [ ] Steam integration preparation

### Unity Integration (Phase 2+)
When Unity integration is added, the pipeline will include:
- Unity Editor installation in CI
- Unity-specific build commands
- Unity Play Mode test execution
- Executable packaging for Windows

## Best Practices

### For Contributors

1. **Always run tests locally** before pushing
   ```bash
   dotnet test MonopolyFrenzy.slnx
   ```

2. **Keep builds clean** - Address all warnings
   ```bash
   dotnet build MonopolyFrenzy.slnx --warnaserror
   ```

3. **Write tests first** (TDD approach)
   - Write failing test
   - Implement feature
   - Verify test passes
   - Refactor if needed

4. **Follow C# coding standards**
   - Use XML documentation comments
   - Follow naming conventions
   - Keep methods focused and small

5. **Check code coverage**
   ```bash
   dotnet test --collect:"XPlat Code Coverage"
   ```

### For Build Masters

1. **Monitor CI pipeline health**
   - Check workflow success rates
   - Review failing tests
   - Track build times

2. **Maintain dependencies**
   - Keep NuGet packages updated
   - Test compatibility before upgrading
   - Document breaking changes

3. **Optimize build performance**
   - Use caching effectively
   - Parallelize when possible
   - Profile slow steps

## Additional Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [.NET CLI Reference](https://docs.microsoft.com/dotnet/core/tools/)
- [NUnit Documentation](https://docs.nunit.org/)
- [Coverlet Documentation](https://github.com/coverlet-coverage/coverlet)

## Support

For build or CI/CD issues:
1. Check this documentation
2. Review [.github/workflows/README.md](.github/workflows/README.md)
3. Check GitHub Actions logs
4. Consult [Implementation Plan](planning/IMPLEMENTATION-PLAN.md)
5. Open an issue in the repository

---

**Last Updated:** 2026-02-17  
**Build Status:** ✅ Operational  
**Current Phase:** Phase 1 (85% Complete)
