using Microsoft.Extensions.Options;
using Net10.UserManagement.Application.Common.Abstractions;
using Net10.UserManagement.Infrastructure.Options;

namespace Net10.UserManagement.Infrastructure.Services;

public class OtpSettingsProvider (IOptions<OtpGeneratorOptions> options) : IOtpSettingsProvider
{
    public int ExpirationMinutes => options.Value.ValidForMinutes;
    public int Length => options.Value.Length;
}