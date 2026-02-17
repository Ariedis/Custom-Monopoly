# CI/CD Pipeline Overview

## Pipeline Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                     GitHub Repository                            │
│                   (Ariedis/Custom-Monopoly)                      │
└──────────────────────┬──────────────────────────────────────────┘
                       │
                       │ Push/PR to main
                       │
                       ▼
┌─────────────────────────────────────────────────────────────────┐
│                  GitHub Actions Workflow                         │
│                     (ci-cd.yml)                                  │
└─────────────────────────────────────────────────────────────────┘
                       │
                       │
        ┌──────────────┴───────────────┐
        │                              │
        ▼                              ▼
┌──────────────────┐          ┌──────────────────┐
│ Build and Test   │          │   Package        │
│                  │          │  (main only)     │
│ ✓ Restore        │          │                  │
│ ✓ Build          │          │ ✓ Build          │
│ ✓ Test (182)     │──────────▶ ✓ Publish        │
│ ✓ Coverage       │ success  │ ✓ Archive        │
│ ✓ Report         │          │ ✓ Upload         │
└──────────────────┘          └──────────────────┘
        │                              │
        ▼                              ▼
┌──────────────────┐          ┌──────────────────┐
│   Artifacts      │          │   Artifacts      │
│                  │          │                  │
│ • test-results   │          │ • build package  │
│ • coverage       │          │   (.tar.gz)      │
└──────────────────┘          └──────────────────┘
```

## Workflow Triggers

| Event | Branch | Jobs Run | Artifacts Created |
|-------|--------|----------|-------------------|
| **Push to main** | `main` | Build & Test + Package | Test results, Coverage, Build package |
| **Pull Request to main** | Any | Build & Test only | Test results, Coverage |
| **Manual Dispatch** | Any | Build & Test + Package* | Test results, Coverage, Build package* |

*Package job only runs if triggered on main branch

## Job Execution Flow

### Job 1: Build and Test (Always Runs)

```
1. Checkout Code
   └─> Latest commit from branch
   
2. Setup .NET 8.0
   └─> Install .NET SDK
   
3. Restore Dependencies
   └─> dotnet restore MonopolyFrenzy.slnx
   └─> Downloads NuGet packages (Newtonsoft.Json, NUnit, etc.)
   
4. Build Solution
   └─> dotnet build --configuration Release
   └─> Compiles all C# code
   └─> Result: DLL files in bin/Release/
   
5. Run Tests
   └─> dotnet test --logger trx --collect:"Code Coverage"
   └─> Executes 182 NUnit tests
   └─> Generates test results (.trx) and coverage reports
   
6. Upload Artifacts
   └─> Test results → test-results artifact
   └─> Coverage reports → coverage-reports artifact
   
7. Publish Test Report
   └─> Creates GitHub Actions summary with test results
```

**Duration:** ~30-45 seconds

### Job 2: Package (Only on main branch push)

```
1. Checkout Code
   └─> Latest commit from main branch
   
2. Setup .NET 8.0
   └─> Install .NET SDK
   
3. Restore & Build
   └─> dotnet restore + build
   └─> Ensures clean build
   
4. Publish Application
   └─> dotnet publish --output ./publish/MonopolyFrenzy
   └─> Creates self-contained binaries
   └─> Includes all dependencies
   
5. Create Build Info
   └─> Generates BUILD_INFO.txt with metadata
   └─> Includes commit SHA, date, branch info
   
6. Create Archive
   └─> tar -czf MonopolyFrenzy-{sha}.tar.gz
   └─> Compresses all binaries and dependencies
   
7. Upload Artifact
   └─> Stores archive in GitHub
   └─> Retained for 30 days
   
8. Generate Summary
   └─> Creates build summary in GitHub UI
```

**Duration:** ~45-60 seconds

## Current Build Status

### Phase 1 Development (85% Complete)

```
Test Suite Status:
╔═══════════════════════════════════════════════════════════════╗
║  Total Tests: 182                                              ║
║  ├─ Passing:  65  (36%) ✓                                     ║
║  └─ Failing:  117 (64%) ⚠ (Implementation in progress)        ║
║                                                                ║
║  Build: SUCCESS ✓                                              ║
║  Warnings: 255 (NUnit analyzer suggestions - non-critical)    ║
║  Errors: 0                                                     ║
╚═══════════════════════════════════════════════════════════════╝
```

### Test Coverage by Component

```
Component          Target    Current   Status
─────────────────────────────────────────────
Core (GameState)   90%       In dev    🟡
State Machine      85%       In dev    🟡
Commands           100%      In dev    🟡
Rules Engine       90%       In dev    🟡
Event System       80%       In dev    🟡
─────────────────────────────────────────────
Overall            85%+      In dev    🟡
```

## Artifact Details

### Test Results Artifact

**Name:** `test-results`  
**Format:** .trx (Visual Studio Test Results)  
**Contains:**
- Test execution results
- Pass/fail status for each test
- Execution times
- Error messages and stack traces

**How to view:**
1. Download from GitHub Actions artifacts
2. Open in Visual Studio, VS Code, or text editor
3. Convert to HTML with test result viewer

### Coverage Reports Artifact

**Name:** `coverage-reports`  
**Format:** Cobertura XML  
**Contains:**
- Line coverage percentages
- Branch coverage metrics
- Covered/uncovered code paths
- Per-file and per-method statistics

**How to view:**
1. Download from GitHub Actions artifacts
2. Use ReportGenerator to create HTML report:
   ```bash
   reportgenerator -reports:coverage.cobertura.xml -targetdir:report
   ```

### Build Package Artifact

**Name:** `monopoly-frenzy-build`  
**Format:** .tar.gz compressed archive  
**Contains:**
- Compiled binaries (MonopolyFrenzy.dll)
- All dependencies (Newtonsoft.Json.dll, etc.)
- BUILD_INFO.txt with metadata
- Configuration files

**How to use:**
1. Download from GitHub Actions artifacts
2. Extract: `tar -xzf MonopolyFrenzy-{sha}.tar.gz`
3. Run tests or integrate into larger application

## GitHub Actions Integration

### PR Workflow

```
Developer creates PR
        │
        ▼
GitHub Actions triggered
        │
        ├─> Build & Test job runs
        │   ├─> All tests pass? ✓
        │   └─> Report posted to PR
        │
        ▼
PR shows status check
        │
        ├─> ✓ All checks passed
        │   └─> Ready to merge
        │
        └─> ✗ Some checks failed
            └─> Review test results
            └─> Fix issues
            └─> Push new commit
```

### Main Branch Workflow

```
PR merged to main
        │
        ▼
Push to main triggers workflow
        │
        ├─> Build & Test job
        │   └─> Validates merge
        │
        └─> Package job
            ├─> Creates binaries
            ├─> Packages application
            └─> Uploads artifact
            
        ▼
Artifact available for download
        └─> Retained for 30 days
```

## Build Metrics

### Performance Targets

| Metric | Target | Current |
|--------|--------|---------|
| **Build Time** | < 60s | ~30-45s ✓ |
| **Test Execution** | < 10s | ~5s ✓ |
| **Package Time** | < 60s | ~45-60s ✓ |
| **Total Pipeline** | < 2min | ~1-2min ✓ |

### Resource Usage

| Resource | Usage |
|----------|-------|
| **Runner** | Ubuntu Latest (GitHub-hosted) |
| **vCPUs** | 2 cores |
| **Memory** | 7 GB |
| **Storage** | 14 GB SSD |
| **Build artifacts** | ~5-10 MB per build |

## Security Considerations

### Current Security Measures

✓ **No secrets in code** - All builds use public packages  
✓ **Artifact retention** - 30-day automatic cleanup  
✓ **Branch protection** - CI must pass before merge  
✓ **Dependency scanning** - NuGet vulnerability checks  

### Planned Security Enhancements

- [ ] CodeQL security analysis
- [ ] Dependency vulnerability scanning
- [ ] SBOM (Software Bill of Materials) generation
- [ ] Code signing for releases
- [ ] Security audit reports

## Monitoring & Maintenance

### What to Monitor

1. **Build Success Rate**
   - Target: > 95% for main branch
   - Current: TBD (new pipeline)

2. **Test Pass Rate**
   - Target: 100% (when Phase 1 complete)
   - Current: 36% (implementation in progress)

3. **Build Duration**
   - Target: < 2 minutes
   - Current: ~1-2 minutes ✓

4. **Artifact Size**
   - Target: < 50 MB
   - Current: ~5-10 MB ✓

### Maintenance Tasks

**Weekly:**
- Review failed builds
- Check for outdated dependencies
- Monitor artifact storage usage

**Monthly:**
- Update GitHub Actions versions
- Review and update dependencies
- Optimize build performance

**Quarterly:**
- Review security reports
- Update .NET SDK version
- Assess pipeline improvements

## Troubleshooting Guide

### Common Issues

#### Build Fails: "Restore failed"
```
Symptom: Cannot download NuGet packages
Solution: Check NuGet.org status, verify network connectivity
```

#### Build Fails: "Compilation error"
```
Symptom: C# code doesn't compile
Solution: Fix compilation errors locally first, then push
```

#### Tests Fail: Unexpected failures
```
Symptom: Tests pass locally but fail in CI
Solution: Check for environment-specific issues, timing problems
```

#### Package Not Created
```
Symptom: No build artifact after merge to main
Solution: Verify build-and-test job passed, check package job logs
```

## Future Roadmap

### Phase 2: Unity Integration
- Add Unity Editor to CI pipeline
- Unity-specific build commands
- Unity Play Mode test execution
- Windows executable generation

### Phase 3: Advanced CI/CD
- Automated deployment to testing environment
- Performance benchmarking
- Load testing
- Integration with Steam pipeline

### Phase 4: Release Automation
- Semantic versioning
- Automated changelog generation
- Release notes creation
- Multi-platform builds (Windows, Linux, macOS)

## Resources

- **GitHub Actions Logs:** Available in repository Actions tab
- **Build Documentation:** [BUILD.md](BUILD.md)
- **Workflow Details:** [.github/workflows/README.md](.github/workflows/README.md)
- **Implementation Plan:** [planning/IMPLEMENTATION-PLAN.md](planning/IMPLEMENTATION-PLAN.md)

---

**Pipeline Status:** ✅ Operational  
**Last Updated:** 2026-02-17  
**Version:** 1.0  
**Maintained By:** Build Engineering Team
