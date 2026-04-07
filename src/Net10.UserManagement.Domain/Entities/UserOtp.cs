namespace Net10.UserManagement.Domain.Entities;

public class UserOtp
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Code { get; private set; } = null!;
    public bool IsUsed { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }

    private UserOtp(Guid userId, string code, int expirationMinutes)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        SetCode(code);
        ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);
        IsUsed = false;
        CreatedAt = DateTime.UtcNow;
    }

    public static UserOtp Create(Guid userId, string code, int expirationMinutes = 10)
    {
        return new UserOtp(userId, code, expirationMinutes);
    }

    private void SetCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("OTP code cannot be empty", nameof(code));
        
        if (code.Length != 6 || !code.All(char.IsDigit))
            throw new ArgumentException("OTP code must be 6 digits", nameof(code));
        
        Code = code;
    }

    public void MarkAsUsed()
    {
        if (IsUsed)
            throw new InvalidOperationException("OTP has already been used");
        
        if (DateTime.UtcNow > ExpiresAt)
            throw new InvalidOperationException("OTP has expired");
        
        IsUsed = true;
    }

    public bool IsValid()
    {
        return !IsUsed && DateTime.UtcNow <= ExpiresAt;
    }

    public bool ValidateCode(string code)
    {
        return Code == code && IsValid();
    }
}