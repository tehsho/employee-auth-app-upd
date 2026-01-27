namespace EmployeeAuth.Infrastructure.Email;

public class ConsoleEmailSender : IEmailSender
{
    private readonly ILogger<ConsoleEmailSender> _logger;

    public ConsoleEmailSender(ILogger<ConsoleEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string to, string subject, string body)
    {
        _logger.LogInformation("Sending email via ConsoleEmailSender to {To} (Subject: {Subject})", to, subject);

        Console.WriteLine("----- EMAIL -----");
        Console.WriteLine($"To: {to}");
        Console.WriteLine(subject);
        Console.WriteLine(body);
        Console.WriteLine("-----------------");

        return Task.CompletedTask;
    }
}
