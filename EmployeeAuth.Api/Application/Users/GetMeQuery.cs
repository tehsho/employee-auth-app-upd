using MediatR;

namespace EmployeeAuth.Application.Users;

public record GetMeQuery(Guid UserId) : IRequest<MeDto>;

public record MeDto(string UserName, string Name, string Email);
