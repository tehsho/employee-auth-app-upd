using System.Reflection;
using System.Text;
using EmployeeAuth.Api.Domain.Options;
using EmployeeAuth.Domain.Options;
using EmployeeAuth.Infrastructure.Email;
using EmployeeAuth.Infrastructure.Persistence;
using EmployeeAuth.Infrastructure.Security;
using EmployeeAuth.Infrastructure.Strategies;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core SQL Server
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Options
builder.Services.Configure<PasswordPolicyOptions>(builder.Configuration.GetSection("PasswordPolicy"));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Security services
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<IPasswordGenerator, PasswordGenerator>();
builder.Services.AddScoped<IPasswordPolicyValidator, PasswordPolicyValidator>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Strategy services
builder.Services.AddScoped<UsernameLookupStrategy>();
builder.Services.AddScoped<EmailLookupStrategy>();
builder.Services.AddScoped<ILoginLookupStrategyResolver, LoginLookupStrategyResolver>();

// Email service (simple)
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Email"));
builder.Services.AddScoped<IEmailSender>(sp =>
{
    var opt = sp.GetRequiredService<IOptions<EmailOptions>>().Value;

    return opt.Provider.Equals("Smtp", StringComparison.OrdinalIgnoreCase)
        ? ActivatorUtilities.CreateInstance<SmtpEmailSender>(sp)
        : ActivatorUtilities.CreateInstance<ConsoleEmailSender>(sp);
});

// JWT Auth
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = key
        };
    });

builder.Services.AddAuthorization();

// Basic CORS for frontend dev
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("dev", p =>
        p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

var app = builder.Build();

app.UseCors("dev");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Simple error handler (keeps responses readable)
app.Use(async (ctx, next) =>
{
    try { await next(); }
    catch (UnauthorizedAccessException)
    {
        ctx.Response.StatusCode = 401;
        await ctx.Response.WriteAsJsonAsync(new { error = "Invalid credentials." });
    }
    catch (Exception ex)
    {
        ctx.Response.StatusCode = 400;
        await ctx.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
public partial class Program { }
public partial class Program { }
