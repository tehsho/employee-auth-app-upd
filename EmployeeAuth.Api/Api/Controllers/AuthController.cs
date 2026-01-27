using EmployeeAuth.Application.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAuth.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    public record LoginRequest(string Login, string Password);

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var token = await _mediator.Send(new LoginCommand(req.Login, req.Password));
        return Ok(new { token });
    }
}

