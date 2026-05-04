using FluentAssertions;
using FlyGon.CQRS.Commands;
using FlyGon.Notifications;
using Xunit;

namespace FlyGon.CQRS.Tests.Commands;

public class CommandTests
{
    private class TestCommand : Command
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }

        public override void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                AddNotification("Name", "Name is required");

            if (Age < 0)
                AddNotification("Age", "Age must be positive");

            if (Age > 150)
                AddNotification("Age", "Age must be less than 150");
        }
    }

    [Fact]
    public void Command_ShouldInheritFromNotifiable()
    {
        // Arrange & Act
        var command = new TestCommand();

        // Assert
        command.Should().BeAssignableTo<Notifiable>();
    }

    [Fact]
    public void Validate_WithValidData_ShouldNotHaveNotifications()
    {
        // Arrange
        var command = new TestCommand
        {
            Name = "John Doe",
            Age = 30
        };

        // Act
        command.Validate();

        // Assert
        command.IsValid.Should().BeTrue();
        command.Notifications.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldAddNotification()
    {
        // Arrange
        var command = new TestCommand
        {
            Name = "",
            Age = 30
        };

        // Act
        command.Validate();

        // Assert
        command.IsValid.Should().BeFalse();
        command.Notifications.Should().ContainSingle()
            .Which.Property.Should().Be("Name");
    }

    [Fact]
    public void Validate_WithNegativeAge_ShouldAddNotification()
    {
        // Arrange
        var command = new TestCommand
        {
            Name = "John Doe",
            Age = -5
        };

        // Act
        command.Validate();

        // Assert
        command.IsValid.Should().BeFalse();
        command.Notifications.Should().ContainSingle()
            .Which.Property.Should().Be("Age");
    }

    [Fact]
    public void Validate_WithAgeTooHigh_ShouldAddNotification()
    {
        // Arrange
        var command = new TestCommand
        {
            Name = "John Doe",
            Age = 200
        };

        // Act
        command.Validate();

        // Assert
        command.IsValid.Should().BeFalse();
        command.Notifications.Should().ContainSingle()
            .Which.Property.Should().Be("Age");
    }

    [Fact]
    public void Validate_WithMultipleErrors_ShouldAddMultipleNotifications()
    {
        // Arrange
        var command = new TestCommand
        {
            Name = "",
            Age = -10
        };

        // Act
        command.Validate();

        // Assert
        command.IsValid.Should().BeFalse();
        command.Notifications.Should().HaveCount(2);
    }

    [Theory]
    [InlineData("", 25, false)]
    [InlineData("Valid Name", -1, false)]
    [InlineData("Valid Name", 151, false)]
    [InlineData("Valid Name", 25, true)]
    [InlineData("Valid Name", 0, true)]
    [InlineData("Valid Name", 150, true)]
    public void Validate_WithVariousInputs_ShouldValidateCorrectly(string name, int age, bool expectedValid)
    {
        // Arrange
        var command = new TestCommand
        {
            Name = name,
            Age = age
        };

        // Act
        command.Validate();

        // Assert
        command.IsValid.Should().Be(expectedValid);
    }
}
