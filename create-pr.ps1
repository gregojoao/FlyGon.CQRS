# Script to create Pull Request for .NET 8.0 Upgrade
# Run this script after authenticating with GitHub CLI

Write-Host "Creating Pull Request for .NET 8.0 Upgrade..." -ForegroundColor Green
Write-Host ""

# Check if gh is authenticated
$authStatus = gh auth status 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ GitHub CLI is not authenticated." -ForegroundColor Red
    Write-Host ""
    Write-Host "Please run: gh auth login" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "After authentication, run this script again." -ForegroundColor Yellow
    exit 1
}

Write-Host "✅ GitHub CLI is authenticated" -ForegroundColor Green
Write-Host ""

# Create the PR
$title = "Upgrade to .NET 8.0 and Add Comprehensive Unit Tests"
$body = @"
## Summary
This PR upgrades the project to .NET 8.0 (LTS) and adds comprehensive unit test coverage.

## Changes Made

### Framework Upgrade
- ✅ Updated **FlyGon.CQRS** from ``netstandard2.1`` to ``net8.0``
- ✅ Updated **FlyGon.CQRS.HowToUse** sample from ``net5.0`` to ``net8.0``
- ✅ Bumped package version from ``1.0.1`` to ``2.0.0``
- ✅ Updated documentation file paths to reflect new target framework

### Dependencies
All dependencies are using their latest compatible versions:
- FlyGon.Notifications: ``1.1.2`` (unchanged, compatible with net8.0)

### Unit Tests (NEW)
Added a comprehensive test project with **33 passing tests** covering:

#### CommandResult Tests (7 tests)
- Constructor behavior with and without parameters
- Property setters
- Complex object data storage
- Various success/failure scenarios

#### Command Validation Tests (8 tests)
- Valid data scenarios
- Empty/null field validation
- Boundary value testing
- Multiple validation errors
- Integration with FlyGon.Notifications

#### CommandHandler Tests (4 tests)
- Valid command processing
- Invalid command handling
- Cancellation token support
- Partial validation failures

#### GenericHandler Tests (14 tests)
- Query handlers (user lookup)
- Calculator operations (add, subtract, multiply, divide)
- Error handling (division by zero, invalid operations)
- Multiple request handling
- Cancellation token support

### Test Dependencies
- xunit: ``2.9.2``
- FluentAssertions: ``6.12.2``
- NSubstitute: ``5.3.0``
- Microsoft.NET.Test.Sdk: ``17.11.1``
- coverlet.collector: ``6.0.2``

## Verification
✅ All projects compile successfully in Release mode  
✅ All 33 unit tests pass  
✅ Code coverage report generated  
✅ Sample project runs correctly with net8.0

## Breaking Changes
⚠️ This is a major version bump (2.0.0) due to the framework upgrade from netstandard2.1 to net8.0. Projects targeting older frameworks may need to upgrade.

## Benefits
- 🚀 Access to latest .NET 8.0 performance improvements
- 🛡️ Long-term support (LTS) until November 2026
- ✅ Comprehensive test coverage for better code quality
- 📊 Code coverage metrics available
- 🔧 Better tooling and IDE support
"@

Write-Host "Creating Pull Request..." -ForegroundColor Cyan
gh pr create --title $title --body $body --base master

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✅ Pull Request created successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "View your PR at: https://github.com/grecojoao/FlyGon.CQRS/pulls" -ForegroundColor Cyan
} else {
    Write-Host ""
    Write-Host "❌ Failed to create Pull Request" -ForegroundColor Red
    Write-Host ""
    Write-Host "You can create it manually at:" -ForegroundColor Yellow
    Write-Host "https://github.com/grecojoao/FlyGon.CQRS/pull/new/upgrade-dotnet8-and-dependencies" -ForegroundColor Cyan
}
