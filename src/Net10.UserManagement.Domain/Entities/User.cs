using Net10.UserManagement.Domain.Enums;

namespace Net10.UserManagement.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public Status Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private User(string email, string firstName, string lastName)
    {
        Id = Guid.NewGuid();
        SetEmail(email);
        SetFirstName(firstName);
        SetLastName(lastName);
        CreatedAt = DateTime.UtcNow;
        SetPending();
    }

    public static User CreatePending(string email, string firstName, string lastName)
    {
        return new User(email, firstName, lastName);
    }

    public void UpdateEmail(string newEmail)
    {
        SetEmail(newEmail);
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

}