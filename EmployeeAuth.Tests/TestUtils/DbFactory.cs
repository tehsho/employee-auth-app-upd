using EmployeeAuth.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAuth.Tests.TestUtils;

public static class DbFactory
{
    public static (AppDbContext db, SqliteConnection conn) CreateSqliteInMemoryDb()
    {
        var conn = new SqliteConnection("DataSource=:memory:");
        conn.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(conn)
            .Options;

        var db = new AppDbContext(options);
        db.Database.EnsureCreated(); // creates Users table, etc.

        return (db, conn);
    }
}
