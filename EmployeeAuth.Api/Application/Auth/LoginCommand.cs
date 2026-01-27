using MediatR;

namespace EmployeeAuth.Application.Auth;

public record LoginCommand(string Login, string Password) : IRequest<string>; // returns JWT
