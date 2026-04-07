namespace Net10.UserManagement.Application.Common.Abstractions;

public interface IEmailSender
{
    Task SendAsync<T>(string toEmail, string templateName, T data, CancellationToken cancellationToken = default);
}