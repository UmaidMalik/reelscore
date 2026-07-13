using Moq;
using ReelScore.Api.DataTransferObjects;
using ReelScore.Api.Models;
using ReelScore.Api.Repositories;
using ReelScore.Api.Services;

namespace ReelScore.Api.Tests.Services;

public sealed class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepository = new();

    [Fact]
    public async Task CreateUserAsync_WhenEmailExists_ReturnsConflictStatus()
    {
        _userRepository
            .Setup(repository => repository.EmailExistsAsync(
                "umaid@example.com",
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = new UserService(_userRepository.Object);
        var request = new CreateUserRequest
        {
            Username = "Umaid",
            Email = "Umaid@Example.com"
        };

        var result = await service.CreateUserAsync(request);

        Assert.Equal(CreateUserStatus.EmailAlreadyExists, result.Status);
        Assert.Null(result.User);

        _userRepository.Verify(
            repository => repository.AddAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateUserAsync_NormalizesEmailAndCreatesUser()
    {
        _userRepository
            .Setup(repository => repository.EmailExistsAsync(
                It.IsAny<string>(),
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _userRepository
            .Setup(repository => repository.UsernameExistsAsync(
                It.IsAny<string>(),
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        User? capturedUser = null;

        _userRepository
            .Setup(repository => repository.AddAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((user, _) =>
            {
                user.UserId = 7;
                capturedUser = user;
            })
            .ReturnsAsync((User user, CancellationToken _) => user);

        var service = new UserService(_userRepository.Object);
        var request = new CreateUserRequest
        {
            Username = "  Umaid  ",
            Email = "  UMAID@EXAMPLE.COM  "
        };

        var result = await service.CreateUserAsync(request);

        Assert.Equal(CreateUserStatus.Created, result.Status);
        Assert.NotNull(result.User);
        Assert.Equal(7, result.User.UserId);
        Assert.Equal("Umaid", result.User.Username);
        Assert.Equal("umaid@example.com", result.User.Email);
        Assert.NotNull(capturedUser);
        Assert.Equal("umaid@example.com", capturedUser.Email);
    }

    [Fact]
    public async Task UpdateUserAsync_WhenUserDoesNotExist_ReturnsNotFound()
    {
        _userRepository
            .Setup(repository => repository.GetByIdAsync(
                404,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var service = new UserService(_userRepository.Object);
        var request = new UpdateUserRequest
        {
            Username = "Missing",
            Email = "missing@example.com"
        };

        var result = await service.UpdateUserAsync(404, request);

        Assert.Equal(UpdateUserStatus.NotFound, result.Status);
        Assert.Null(result.User);
    }
}
