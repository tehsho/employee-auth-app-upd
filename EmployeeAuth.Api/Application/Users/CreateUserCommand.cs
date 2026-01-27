using MediatR;

namespace EmployeeAuth.Application.Users;

public record CreateUserCommand(string UserName, string Name, string Email) : IRequest<Guid>;

