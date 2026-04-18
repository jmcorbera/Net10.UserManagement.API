using System.Text.RegularExpressions;
using Net10.UserManagement.Domain.Enums;

namespace Net10.UserManagement.Domain.Entities;

public partial class User
{
    public Guid Id { get; private set; }

    public string Identification { get; private set; } = null!;
    public int IdentificationTypeId { get; private set; }
    public string? PasswordHash { get; private set; }
    public string Email { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public Status Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private User(string identification, int identificationTypeId, string email, string firstName, string lastName)
    {
        Id = Guid.NewGuid();
        SetIdentification(identification);
        IdentificationTypeId = identificationTypeId;
        SetEmail(email);
        SetFirstName(firstName);
        SetLastName(lastName);
        CreatedAt = DateTime.UtcNow;
        SetPending();
    }

    public static User CreatePending(string identification, int identificationTypeId, string email, string firstName, string lastName)
    {
        return new User(identification, identificationTypeId, email, firstName, lastName);
    }

    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    public void UpdateEmail(string newEmail)
    {
        SetEmail(newEmail);
    }

    public void SetIdentificationTypeId(int identificationTypeId)
    {
        if(identificationTypeId != (int)IdentificationType.DNI && identificationTypeId != (int)IdentificationType.Passport)
            throw new ArgumentException($"Identification type must be {(int)IdentificationType.DNI} or {(int)IdentificationType.Passport}", nameof(identificationTypeId));
        IdentificationTypeId = identificationTypeId;
    }

    public void SetIdentification(string identification)
    {
        if (string.IsNullOrWhiteSpace(identification))
            throw new ArgumentException("Identification cannot be empty", nameof(identification));

        if(!IdentificationRegex().IsMatch(identification))
            throw new ArgumentException("Identification must be 7 or 8 digits", nameof(identification));

        Identification = identification;
    }

    private void SetFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
        FirstName = firstName;
    }

    private void SetLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));
        LastName = lastName;
    }


    private void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        try
        {
            var mailAddress = new System.Net.Mail.MailAddress(email);
            Email = mailAddress.Address;
        }
        catch (FormatException)
        {
            throw new ArgumentException("Invalid email format", nameof(email));
        }
    }

    public void Activate() => Status = Status.Active;
    public void Deactivate() => Status = Status.Inactive;
    public void SetPending() => Status = Status.Pending;
    
    [GeneratedRegex(@"^\d{7,8}$")]
    private static partial Regex IdentificationRegex();
}