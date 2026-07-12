using Microsoft.AspNetCore.Mvc;
using ReelScore.Api.DataTransferObjects;
using ReelScore.Api.Services;

namespace ReelScore.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<UserResponse>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<UserResponse>>> GetUsers(
        CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllUsersAsync(cancellationToken);

        return Ok(users);
    }

    [HttpGet("{userId:long}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> GetUser(
        long userId,
        CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserByIdAsync(
            userId,
            cancellationToken);

        if (user is null)
        {
            return NotFound(new
            {
                message = $"User with ID {userId} was not found."
            });
        }

        return Ok(user);
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponse>> PostUser(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _userService.CreateUserAsync(
            request,
            cancellationToken);

        if (result.Status == CreateUserStatus.EmailAlreadyExists)
        {
            return Conflict(new
            {
                message = "A user with this email already exists."
            });
        }

        if (result.Status == CreateUserStatus.UsernameAlreadyExists)
        {
            return Conflict(new
            {
                message = "A user with this username already exists."
            });
        }

        var user = result.User!;

        return CreatedAtAction(
            nameof(GetUser),
            new
            {
                userId = user.UserId
            },
            user);
    }

    [HttpPut("{userId:long}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponse>> PutUser(
        long userId,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateUserAsync(
            userId,
            request,
            cancellationToken);

        if (result.Status == UpdateUserStatus.NotFound)
        {
            return NotFound(new
            {
                message = $"User with ID {userId} was not found."
            });
        }

        if (result.Status == UpdateUserStatus.EmailAlreadyExists)
        {
            return Conflict(new
            {
                message = "A user with this email already exists."
            });
        }

        if (result.Status == UpdateUserStatus.UsernameAlreadyExists)
        {
            return Conflict(new
            {
                message = "A user with this username already exists."
            });
        }

        return Ok(result.User);
    }

    [HttpDelete("{userId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(
        long userId,
        CancellationToken cancellationToken)
    {
        var deleted = await _userService.DeleteUserAsync(
            userId,
            cancellationToken);

        if (!deleted)
        {
            return NotFound(new
            {
                message = $"User with ID {userId} was not found."
            });
        }

        return NoContent();
    }
}
