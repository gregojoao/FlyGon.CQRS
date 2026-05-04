using FluentAssertions;
using FlyGon.CQRS.Handlers.Contracts;
using Xunit;

namespace FlyGon.CQRS.Tests.Handlers;

public class GenericHandlerTests
{
    private record GetUserByIdRequest(Guid UserId);
    
    private record UserResponse(Guid Id, string Name, string Email);

    private class GetUserByIdHandler : IGenericHandler<GetUserByIdRequest, UserResponse?>
    {
        private readonly Dictionary<Guid, UserResponse> _users;

        public GetUserByIdHandler()
        {
            _users = new Dictionary<Guid, UserResponse>
            {
                { Guid.Parse("00000000-0000-0000-0000-000000000001"), 
                    new UserResponse(Guid.Parse("00000000-0000-0000-0000-000000000001"), "John Doe", "john@example.com") },
                { Guid.Parse("00000000-0000-0000-0000-000000000002"), 
                    new UserResponse(Guid.Parse("00000000-0000-0000-0000-000000000002"), "Jane Smith", "jane@example.com") }
            };
        }

        public Task<UserResponse?> Handle(GetUserByIdRequest request, CancellationToken cancellationToken)
        {
            _users.TryGetValue(request.UserId, out var user);
            return Task.FromResult(user);
        }
    }

    [Fact]
    public async Task Handle_WithExistingUserId_ShouldReturnUser()
    {
        // Arrange
        var handler = new GetUserByIdHandler();
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var request = new GetUserByIdRequest(userId);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(userId);
        result.Name.Should().Be("John Doe");
        result.Email.Should().Be("john@example.com");
    }

    [Fact]
    public async Task Handle_WithNonExistingUserId_ShouldReturnNull()
    {
        // Arrange
        var handler = new GetUserByIdHandler();
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000099");
        var request = new GetUserByIdRequest(userId);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithCancellationToken_ShouldAcceptToken()
    {
        // Arrange
        var handler = new GetUserByIdHandler();
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var request = new GetUserByIdRequest(userId);
        var cts = new CancellationTokenSource();

        // Act
        var result = await handler.Handle(request, cts.Token);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithMultipleRequests_ShouldReturnCorrectUsers()
    {
        // Arrange
        var handler = new GetUserByIdHandler();
        var userId1 = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var userId2 = Guid.Parse("00000000-0000-0000-0000-000000000002");

        // Act
        var result1 = await handler.Handle(new GetUserByIdRequest(userId1), CancellationToken.None);
        var result2 = await handler.Handle(new GetUserByIdRequest(userId2), CancellationToken.None);

        // Assert
        result1.Should().NotBeNull();
        result1!.Name.Should().Be("John Doe");
        
        result2.Should().NotBeNull();
        result2!.Name.Should().Be("Jane Smith");
    }

    private class CalculatorRequest
    {
        public int A { get; set; }
        public int B { get; set; }
        public string Operation { get; set; } = string.Empty;
    }

    private class CalculatorResult
    {
        public int Result { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }

    private class CalculatorHandler : IGenericHandler<CalculatorRequest, CalculatorResult>
    {
        public Task<CalculatorResult> Handle(CalculatorRequest request, CancellationToken cancellationToken)
        {
            var result = new CalculatorResult { Success = true };

            try
            {
                result.Result = request.Operation switch
                {
                    "add" => request.A + request.B,
                    "subtract" => request.A - request.B,
                    "multiply" => request.A * request.B,
                    "divide" => request.B != 0 ? request.A / request.B : throw new DivideByZeroException(),
                    _ => throw new InvalidOperationException("Invalid operation")
                };
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            return Task.FromResult(result);
        }
    }

    [Theory]
    [InlineData(10, 5, "add", 15)]
    [InlineData(10, 5, "subtract", 5)]
    [InlineData(10, 5, "multiply", 50)]
    [InlineData(10, 5, "divide", 2)]
    public async Task CalculatorHandler_WithValidOperations_ShouldReturnCorrectResult(
        int a, int b, string operation, int expectedResult)
    {
        // Arrange
        var handler = new CalculatorHandler();
        var request = new CalculatorRequest { A = a, B = b, Operation = operation };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Result.Should().Be(expectedResult);
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task CalculatorHandler_WithDivisionByZero_ShouldReturnError()
    {
        // Arrange
        var handler = new CalculatorHandler();
        var request = new CalculatorRequest { A = 10, B = 0, Operation = "divide" };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CalculatorHandler_WithInvalidOperation_ShouldReturnError()
    {
        // Arrange
        var handler = new CalculatorHandler();
        var request = new CalculatorRequest { A = 10, B = 5, Operation = "invalid" };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Invalid operation");
    }
}
