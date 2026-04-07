using Net10.UserManagement.Domain.Entities;
using Net10.UserManagement.Domain.Enums;

namespace Net10.UserManagement.Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void CreatePending_Should_Create_User_With_Pending_Status()
    {
        var user = User.CreatePending("12345678", "jane.doe@example.com", "Jane", "Doe");

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("12345678", user.Identification);
        Assert.Equal("jane.doe@example.com", user.Email);
        Assert.Equal("Jane", user.FirstName);
        Assert.Equal("Doe", user.LastName);
        Assert.Equal(Status.Pending, user.Status);
        Assert.True(user.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void CreatePending_Should_Throw_When_Identification_Is_Empty_or_Null()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            User.CreatePending("", "test@example.com", "John", "Doe"));

        Assert.Throws<ArgumentException>(() =>
            User.CreatePending(null!, "test@example.com", "John", "Doe"));
    }
    
    [Fact]
    public void CreatePending_Should_Throw_When_Identification_Is_Invalid()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            User.CreatePending("1234567890123456789012345678901234567890","invalid-email", "John", "Doe"));
    }

    [Fact]
    public void CreatePending_Should_Throw_When_Email_Is_Invalid()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            User.CreatePending("12345678","invalid-email", "John", "Doe"));
    }

    [Fact]
    public void CreatePending_Should_Throw_When_FirstName_Is_Empty_or_Null()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            User.CreatePending("12345678", "test@example.com", "", "Doe"));

        Assert.Throws<ArgumentException>(() =>
            User.CreatePending("12345678", "test@example.com", null!, "Doe"));
    }
    
    [Fact]
    public void CreatePending_Should_Throw_When_LastName_Is_Empty_or_Null()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            User.CreatePending("12345678", "test@example.com", "John", ""));

        Assert.Throws<ArgumentException>(() =>
            User.CreatePending("12345678", "test@example.com", "John", null!));
    }

    [Fact]
    public void Activate_Should_Change_Status_To_Active()
    {
        var user = User.CreatePending("12345678", "test@example.com", "John", "Doe");
        
        user.Activate();
        
        Assert.Equal(Status.Active, user.Status);
    }

    [Fact]
    public void Deactivate_Should_Change_Status_To_Inactive()
    {
        var user = User.CreatePending("12345678", "test@example.com", "John", "Doe");
        
        user.Deactivate();
        
        Assert.Equal(Status.Inactive, user.Status);
    }

    [Fact]
    public void UpdateEmail_Should_Update_Email_Successfully()
    {
        var user = User.CreatePending("12345678", "old@example.com", "John", "Doe");
        
        user.UpdateEmail("new@example.com");
        
        Assert.Equal("new@example.com", user.Email);
    }

    [Fact]
    public void UpdateEmail_Should_Throw_When_Email_Is_Invalid()
    {
        var user = User.CreatePending("12345678", "old@example.com", "John", "Doe");
        
        Assert.Throws<ArgumentException>(() => user.UpdateEmail("invalid-email"));
    }
}
