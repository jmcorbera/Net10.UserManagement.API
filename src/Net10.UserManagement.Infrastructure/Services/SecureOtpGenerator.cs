using Net10.UserManagement.Application.Common.Abstractions;
using OtpNet;

namespace Net10.UserManagement.Infrastructure.Services;

public sealed class SecureOtpGenerator(IOtpSettingsProvider otpSettingsProvider) : IOtpGenerator
{
    private readonly IOtpSettingsProvider _otpSettingsProvider = otpSettingsProvider;

    public string Generate()
    {
        var length = _otpSettingsProvider.Length;
        var key = KeyGeneration.GenerateRandomKey(20);
        var totp = new Totp(key, step: 30, mode: OtpHashMode.Sha256, totpSize: length);

        return totp.ComputeTotp();
    }
}