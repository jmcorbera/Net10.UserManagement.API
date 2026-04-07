using Net10.UserManagement.Application.Common.Abstractions;

namespace Net10.UserManagement.Infrastructure.Services;

public sealed class ConsoleEmailSender : IEmailSender
{
    public Task SendAsync<T>(string toEmail, string templateName, T data, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[EMAIL] To: {toEmail}");
        Console.WriteLine($"[EMAIL] Template: {templateName}");
        Console.WriteLine($"[EMAIL] Data: {System.Text.Json.JsonSerializer.Serialize(data)}");
        
        return Task.CompletedTask;
    }
}
