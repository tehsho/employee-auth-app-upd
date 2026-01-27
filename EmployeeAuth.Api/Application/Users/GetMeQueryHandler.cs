using EmployeeAuth.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAuth.Application.Users;

public class GetMeQueryHandler : IRequestHandler<GetMeQuery, MeDto>
{
    private readonly AppDbContext _db;

    public GetMeQueryHandler(AppDbContext db) => _db = db;

    public async Task<MeDto> Handle(GetMeQuery request, CancellationToken ct)
    {
        var user = await _db.Users
            .Where(u => u.Id == request.UserId)
            .Select(u => new MeDto(u.UserName, u.Name, u.Email))
            .FirstOrDefaultAsync(ct);

        if (user is null) throw new InvalidOperationException("User not found.");
        return user;
    }
}
