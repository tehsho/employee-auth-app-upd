using MediatR;

namespace EmployeeAuth.Application.Users;

public record UpdateProfileCommand(Guid UserId, string Name, string? NewPassword) : IRequest;
