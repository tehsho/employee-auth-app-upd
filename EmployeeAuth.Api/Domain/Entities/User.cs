using System.ComponentModel.DataAnnotations;

namespace EmployeeAuth.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    [MaxLength(50)]
    public string UserName { get; set; } = default!;

    [MaxLength(100)]
    public string Name { get; set; } = default!;

    [MaxLength(255)]
    public string Email { get; set; } = default!;

    [MaxLength(500)]
    public string PasswordHash { get; set; } = default!;

    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
