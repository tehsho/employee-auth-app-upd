using EmployeeAuth.Infrastructure.Persistence;
using EmployeeAuth.Infrastructure.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EmployeeAuth.Tests.Integration;

public class TestWebAppFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace AppDbContext (SQL Server) with SQLite in-memory
            services.RemoveAll<DbContextOptions<AppDbContext>>();

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(_connection));

            // Override password generator so we know the password in tests
            services.RemoveAll<IPasswordGenerator>();
            services.AddSingleton<IPasswordGenerator>(new FixedPasswordGenerator("Test#123!"));

            // Ensure schema exists
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection?.Dispose();
            _connection = null;
        }
    }

    private sealed class FixedPasswordGenerator : IPasswordGenerator
    {
        private readonly string _password;
        public FixedPasswordGenerator(string password) => _password = password;
        public string Generate() => _password;
    }
}
