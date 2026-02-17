# CI/CD Pipeline Documentation

## Overview

This directory contains GitHub Actions workflows for automated building, testing, and packaging of the Monopoly Frenzy application.

## Workflows

### CI/CD Pipeline (`ci-cd.yml`)

**Triggers:**
- Push to `main` branch
- Pull requests to `main` branch
- Manual workflow dispatch

**Jobs:**

#### 1. Build and Test
Runs on every push and pull request to validate code quality.

**Steps:**
1. **Checkout repository** - Gets the latest code
2. **Setup .NET** - Installs .NET 8.0 SDK
3. **Restore dependencies** - Downloads NuGet packages
4. **Build solution** - Compiles the C# code in Release configuration
5. **Run tests** - Executes all NUnit tests with code coverage
6. **Upload artifacts** - Stores test results and coverage reports

**Artifacts Generated:**
- `test-results` - NUnit test result files (.trx)
- `coverage-reports` - Code coverage reports (Cobertura format)

**Status Reporting:**
- Test results are published to the GitHub Actions summary
- Failures are clearly reported with details

#### 2. Package Application
Runs only when code is merged to `main` branch.

**Steps:**
1. **Checkout repository** - Gets the latest code
2. **Setup .NET** - Installs .NET 8.0 SDK
3. **Restore & Build** - Compiles the application
4. **Publish application** - Creates distributable binaries
5. **Create build info** - Generates build metadata file
6. **Create archive** - Packages build as .tar.gz
7. **Upload artifact** - Stores package for 30 days
8. **Generate summary** - Creates build summary in GitHub UI

**Artifacts Generated:**
- `monopoly-frenzy-build` - Compressed archive with application binaries and build info

## Current Build Status

The project is currently in **Phase 1** development (85% complete). The CI/CD pipeline is operational and will:

✅ **Build:** Compile all C# source code  
✅ **Test:** Run 182 unit tests (65 passing, 117 in development)  
✅ **Package:** Create distributable artifacts on main branch merges  
✅ **Report:** Provide detailed test and coverage reports  

### Expected Test Results

The project follows a Test-Driven Development (TDD) approach. Tests were written first, and implementation is ongoing:

- **Total Tests:** 182
- **Currently Passing:** 65 (36%)
- **In Development:** 117 (64%)
- **Target Coverage:** 85%+

As Phase 1 implementation progresses, the passing test percentage will increase.

## Running Locally

### Prerequisites
- .NET 8.0 SDK or later
- Git

### Build Commands

```bash
# Restore dependencies
dotnet restore MonopolyFrenzy.slnx

# Build solution
dotnet build MonopolyFrenzy.slnx --configuration Release

# Run tests
dotnet test MonopolyFrenzy.slnx --configuration Release

# Run tests with coverage
dotnet test MonopolyFrenzy.slnx --configuration Release --collect:"XPlat Code Coverage"

# Publish application
dotnet publish src/MonopolyFrenzy/MonopolyFrenzy.csproj --configuration Release --output ./publish
```

## Understanding Build Results

### Test Results

Test results are available in the GitHub Actions UI:
1. Click on the workflow run
2. Navigate to the "Build and Test" job
3. View the "Test Results" section
4. Download artifacts for detailed reports

### Code Coverage

Code coverage reports are generated using Coverlet and stored as artifacts. Download the `coverage-reports` artifact to view detailed coverage analysis.

### Build Artifacts

When code is merged to `main`, the packaged application is available:
1. Go to the workflow run
2. Scroll to "Artifacts" section
3. Download `monopoly-frenzy-build`
4. Extract the .tar.gz file
5. Application binaries are in the `MonopolyFrenzy/` directory

## Configuration

### Environment Variables

The workflow uses these environment variables (defined in `.github/workflows/ci-cd.yml`):

- `DOTNET_VERSION`: .NET SDK version (currently 8.0.x)
- `SOLUTION_FILE`: Solution file name (MonopolyFrenzy.slnx)
- `BUILD_CONFIGURATION`: Build configuration (Release)

### Customization

To customize the CI/CD pipeline:

1. **Change .NET version:** Update `DOTNET_VERSION` in the workflow file
2. **Add deployment:** Add a new job after `package` job
3. **Adjust test settings:** Modify the test command in `build-and-test` job
4. **Change artifact retention:** Update `retention-days` in upload-artifact steps

## Troubleshooting

### Build Failures

**Symptom:** Build fails with compilation errors  
**Solution:** 
- Check error messages in the build log
- Ensure code compiles locally first
- Verify all dependencies are restored

### Test Failures

**Symptom:** Tests fail or produce unexpected results  
**Solution:**
- Review test output in GitHub Actions summary
- Download test result artifacts for detailed analysis
- Run tests locally to reproduce
- Note: Some test failures are expected during Phase 1 development

### Package Not Generated

**Symptom:** No build artifact after merge to main  
**Solution:**
- Verify the commit was pushed to `main` branch
- Check that `build-and-test` job passed
- Review `package` job logs for errors

## Future Enhancements

Planned improvements for the CI/CD pipeline:

- [ ] Add Unity build support when Unity integration is complete
- [ ] Implement automated deployment to staging environment
- [ ] Add performance benchmarking
- [ ] Integrate security scanning (CodeQL)
- [ ] Add Docker containerization
- [ ] Implement semantic versioning
- [ ] Add release notes generation
- [ ] Create installers for Windows distribution

## Related Documentation

- [Implementation Plan](../../planning/IMPLEMENTATION-PLAN.md)
- [Architecture Summary](../../specifications/ARCHITECTURE-SUMMARY.md)
- [Test Suite Documentation](../../src/Assets/Tests/README.md)

## Support

For issues with the CI/CD pipeline:
1. Check the GitHub Actions logs
2. Review this documentation
3. Consult the Implementation Plan
4. Open an issue in the repository

---

**Last Updated:** 2026-02-17  
**Status:** Active and Operational ✅
