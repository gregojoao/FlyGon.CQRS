using FluentAssertions;
using FlyGon.CQRS.Commands;
using FlyGon.CQRS.Commands.Contracts;
using FlyGon.CQRS.Handlers.Contracts;
using Xunit;

namespace FlyGon.CQRS.Tests.Handlers;

public class CommandHandlerTests
{
    private class CreateUserCommand : Command
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public override void Validate()
        {
            if (string.IsNullOrWhiteSpace(Username))
                AddNotification("Username", "Username is required");

            if (string.IsNullOrWhiteSpace(Email))
                AddNotification("Email", "Email is required");
        }
    }

    private class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, CommandResult>
    {
        public Task<CommandResult> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            command.Validate();

            if (command.IsInvalid)
            {
                return Task.FromResult(new CommandResult(
                    false,
                    "Validation failed",
                    command.Notifications
                ));
            }

            var user = new { Id = Guid.NewGuid(), command.Username, command.Email };
            return Task.FromResult(new CommandResult(true, "User created successfully", user));
        }
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldReturnSuccess()
    {
        // Arrange
        var handler = new CreateUserCommandHandler();
        var command = new CreateUserCommand
        {
            Username = "johndoe",
            Email = "john@example.com"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Sucess.Should().BeTrue();
        result.Message.Should().Be("User created successfully");
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithInvalidCommand_ShouldReturnFailure()
    {
        // Arrange
        var handler = new CreateUserCommandHandler();
        var command = new CreateUserCommand
        {
            Username = "",
            Email = ""
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Sucess.Should().BeFalse();
        result.Message.Should().Be("Validation failed");
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithCancellationToken_ShouldAcceptToken()
    {
        // Arrange
        var handler = new CreateUserCommandHandler();
        var command = new CreateUserCommand
        {
            Username = "johndoe",
            Email = "john@example.com"
        };
        var cts = new CancellationTokenSource();

        // Act
        var result = await handler.Handle(command, cts.Token);

        // Assert
        result.Should().NotBeNull();
        result.Sucess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithPartiallyInvalidCommand_ShouldReturnFailure()
    {
        // Arrange
        var handler = new CreateUserCommandHandler();
        var command = new CreateUserCommand
        {
            Username = "johndoe",
            Email = "" // Missing email
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Sucess.Should().BeFalse();
    }
}
