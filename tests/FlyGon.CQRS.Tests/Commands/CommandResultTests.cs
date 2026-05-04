using FluentAssertions;
using FlyGon.CQRS.Commands;
using Xunit;

namespace FlyGon.CQRS.Tests.Commands;

public class CommandResultTests
{
    [Fact]
    public void Constructor_WithoutParameters_ShouldCreateInstance()
    {
        // Act
        var result = new CommandResult();

        // Assert
        result.Should().NotBeNull();
        result.Sucess.Should().BeFalse();
        result.Message.Should().BeNull();
        result.Data.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithParameters_ShouldSetProperties()
    {
        // Arrange
        var success = true;
        var message = "Operation completed successfully";
        var data = new { Id = 1, Name = "Test" };

        // Act
        var result = new CommandResult(success, message, data);

        // Assert
        result.Sucess.Should().BeTrue();
        result.Message.Should().Be(message);
        result.Data.Should().Be(data);
    }

    [Fact]
    public void Constructor_WithoutData_ShouldSetSuccessAndMessage()
    {
        // Arrange
        var success = false;
        var message = "Operation failed";

        // Act
        var result = new CommandResult(success, message);

        // Assert
        result.Sucess.Should().BeFalse();
        result.Message.Should().Be(message);
        result.Data.Should().BeNull();
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var result = new CommandResult();
        var newData = new { Value = 42 };

        // Act
        result.Sucess = true;
        result.Message = "Updated message";
        result.Data = newData;

        // Assert
        result.Sucess.Should().BeTrue();
        result.Message.Should().Be("Updated message");
        result.Data.Should().Be(newData);
    }

    [Theory]
    [InlineData(true, "Success message")]
    [InlineData(false, "Error message")]
    public void Constructor_WithDifferentSuccessValues_ShouldSetCorrectly(bool success, string message)
    {
        // Act
        var result = new CommandResult(success, message);

        // Assert
        result.Sucess.Should().Be(success);
        result.Message.Should().Be(message);
    }

    [Fact]
    public void Data_WithComplexObject_ShouldStoreCorrectly()
    {
        // Arrange
        var complexData = new
        {
            Id = 123,
            Name = "Test Entity",
            CreatedAt = DateTime.UtcNow,
            Tags = new[] { "tag1", "tag2" }
        };

        // Act
        var result = new CommandResult(true, "Created", complexData);

        // Assert
        result.Data.Should().Be(complexData);
    }
}
