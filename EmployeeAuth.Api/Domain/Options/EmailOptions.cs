namespace EmployeeAuth.Api.Domain.Options;

public class EmailOptions
{
    public string Provider { get; set; } = "Console";
    public string From { get; set; } = "no-reply@employeeauth.local";
    public SmtpOptions Smtp { get; set; } = new();

    public class SmtpOptions
    {
        public string Host { get; set; } = "";
        public int Port { get; set; } = 587;
        public string User { get; set; } = "";
        public string Password { get; set; } = "";
        public bool UseStartTls { get; set; } = true;
    }
}
