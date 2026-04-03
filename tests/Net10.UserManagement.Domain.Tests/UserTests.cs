using Net10.UserManagement.Domain.Entities;

namespace Net10.UserManagement.Domain.Tests;

public class UserTests
{
    [Fact]
    public void User_Should_Initialize_String_Properties_With_Empty_String()
    {
        var user = new User();

        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(string.Empty, user.FirstName);
        Assert.Equal(string.Empty, user.LastName);
    }

    [Fact]
    public void User_Should_Preserve_Assigned_Property_Values()
    {
        var id = Guid.NewGuid();
        var createdAt = new DateTime(2026, 4, 1, 12, 0, 0, DateTimeKind.Utc);

        var user = new User
        {
            Id = id,
            Email = "jane.doe@example.com",
            FirstName = "Jane",
            LastName = "Doe",
            CreatedAt = createdAt
        };

        Assert.Equal(id, user.Id);
        Assert.Equal("jane.doe@example.com", user.Email);
        Assert.Equal("Jane", user.FirstName);
        Assert.Equal("Doe", user.LastName);
        Assert.Equal(createdAt, user.CreatedAt);
    }
}
