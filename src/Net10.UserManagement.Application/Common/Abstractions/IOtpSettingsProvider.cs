namespace Net10.UserManagement.Application.Common.Abstractions;

public interface IOtpSettingsProvider
{
    int ExpirationMinutes { get; }
    int Length { get; }
}