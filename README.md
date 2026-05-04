# FlyGon.CQRS

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![Build and Test](https://github.com/gregojoao/FlyGon.CQRS/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/gregojoao/FlyGon.CQRS/actions/workflows/build-and-test.yml)
[![NuGet](https://img.shields.io/nuget/v/FlyGon.CQRS.svg)](https://www.nuget.org/packages/FlyGon.CQRS)
[![NuGet Downloads](https://img.shields.io/nuget/dt/FlyGon.CQRS.svg)](https://www.nuget.org/packages/FlyGon.CQRS)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Tests](https://img.shields.io/badge/tests-33%20passing-success)](tests/)

Easily use the CQRS (Command Query Responsibility Segregation) design pattern in your .NET applications.

## 🚀 What's New in v2.0.0

- ✅ **Upgraded to .NET 10** (LTS until November 2028)
- ✅ **33 comprehensive unit tests** with 100% pass rate
- ✅ **Full test coverage** with code coverage reports
- ✅ **Latest dependencies** (FluentAssertions 8.9.0, Test SDK 18.5.1)
- ✅ **Improved documentation** and examples

## 📦 Installation

```bash
dotnet add package FlyGon.CQRS
```

Or via NuGet Package Manager:

```
Install-Package FlyGon.CQRS
```

## 🎯 Features

- **Command Pattern**: Implement commands with built-in validation
- **Handler Pattern**: Process commands with dedicated handlers
- **Generic Handlers**: Create flexible query and operation handlers
- **Notification System**: Integrated with FlyGon.Notifications for validation feedback
- **Async Support**: Full async/await support with CancellationToken
- **Type Safety**: Strongly-typed commands and results

## 📖 Quick Start

### 1. Create a Command

```csharp
using FlyGon.CQRS.Commands;
using FlyGon.Notifications.Validations;

public class CreateUserCommand : Command
{
    public string Username { get; set; }
    public string Email { get; set; }

    public override void Validate()
    {
        AddNotifications(
            new ValidationContract()
                .IsNotNullOrEmpty(Username, "Username", "Username is required")
                .IsEmail(Email, "Email", "Invalid email format")
        );
    }
}
```

### 2. Create a Handler

```csharp
using FlyGon.CQRS.Commands;
using FlyGon.CQRS.Handlers.Contracts;

public class CreateUserHandler : ICommandHandler<CreateUserCommand, CommandResult>
{
    public async Task<CommandResult> Handle(
        CreateUserCommand command, 
        CancellationToken cancellationToken)
    {
        command.Validate();
        
        if (command.IsInvalid)
            return new CommandResult(false, "Validation failed", command.Notifications);

        // Your business logic here
        var user = await _userRepository.CreateAsync(command);
        
        return new CommandResult(true, "User created successfully", user);
    }
}
```

### 3. Use the Handler

```csharp
var handler = new CreateUserHandler();
var command = new CreateUserCommand 
{ 
    Username = "johndoe", 
    Email = "john@example.com" 
};

var result = await handler.Handle(command, CancellationToken.None);

if (result.Sucess)
{
    Console.WriteLine($"Success: {result.Message}");
    var user = result.Data;
}
else
{
    Console.WriteLine($"Error: {result.Message}");
}
```

## 🔧 Generic Handlers

For queries and operations that don't fit the command pattern:

```csharp
using FlyGon.CQRS.Handlers.Contracts;

public record GetUserByIdQuery(Guid UserId);
public record UserDto(Guid Id, string Name, string Email);

public class GetUserByIdHandler : IGenericHandler<GetUserByIdQuery, UserDto>
{
    public async Task<UserDto> Handle(
        GetUserByIdQuery request, 
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        return new UserDto(user.Id, user.Name, user.Email);
    }
}
```

## 📚 Documentation

- **[SUMMARY.md](SUMMARY.md)** - Complete project summary
- **[UPGRADE_NOTES.md](UPGRADE_NOTES.md)** - Upgrade notes and migration guide
- **[LEIA-ME-UPGRADE.md](LEIA-ME-UPGRADE.md)** - Guia de atualização (Português)
- **[samples/](samples/)** - Working examples

## 🧪 Testing

The project includes comprehensive unit tests:

```bash
# Run all tests
dotnet test

# Run with code coverage
dotnet test --collect:"XPlat Code Coverage"

# Run in Release mode
dotnet test --configuration Release
```

### Test Coverage

| Category | Tests | Coverage |
|----------|-------|----------|
| CommandResult | 7 | ✅ 100% |
| Command Validation | 8 | ✅ 100% |
| CommandHandler | 4 | ✅ 100% |
| GenericHandler | 14 | ✅ 100% |
| **Total** | **33** | **✅ 100%** |

## 🏗️ Project Structure

```
FlyGon.CQRS/
├── src/
│   └── FlyGon.CQRS/
│       ├── Commands/
│       │   ├── Command.cs
│       │   ├── CommandResult.cs
│       │   └── Contracts/
│       └── Handlers/
│           └── Contracts/
├── samples/
│   └── FlyGon.CQRS.HowToUse/
└── tests/
    └── FlyGon.CQRS.Tests/
```

## 🔄 Migration from v1.x to v2.0

### Breaking Changes

- **Target Framework**: Changed from `netstandard2.1` to `net10.0`
- Projects targeting older frameworks need to upgrade to .NET 10 or later

### Benefits of Upgrading

- 🚀 **Performance**: Significant performance improvements from .NET 10
- 🛡️ **Support**: LTS support until November 2028
- ✅ **Quality**: Comprehensive test coverage
- 🔧 **Tooling**: Better IDE and tooling support

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📦 Publishing a New Version

To publish a new version to NuGet:

1. Update the version in `src/FlyGon.CQRS/FlyGon.CQRS.csproj`
2. Commit and push your changes
3. Create a new release on GitHub with a tag (e.g., `v2.0.0`)
4. GitHub Actions will automatically build, test, and publish to NuGet

For detailed instructions, see [.github/RELEASE.md](.github/RELEASE.md)

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👤 Author

**João Greco** ([@gregojoao](https://github.com/gregojoao))

## 🔗 Links

- [NuGet Package](https://www.nuget.org/packages/FlyGon.CQRS)
- [GitHub Repository](https://github.com/gregojoao/FlyGon.CQRS)
- [FlyGon.Notifications](https://www.nuget.org/packages/FlyGon.Notifications)

## ⭐ Show Your Support

Give a ⭐️ if this project helped you!

---

**Made with ❤️ using .NET 10**
