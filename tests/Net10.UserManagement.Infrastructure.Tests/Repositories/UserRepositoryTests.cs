using FluentAssertions;
using Net10.UserManagement.Domain.Entities;
using Net10.UserManagement.Infrastructure.Repositories;

namespace Net10.UserManagement.Infrastructure.Tests.Repositories;

public class UserRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_Should_Return_Initialized_Users()
    {
        var repository = new UserRepository();
        
        var users = await repository.GetAllAsync();
        
        users.Should().NotBeNull();
        users.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_User_When_User_Exists()
    {
        var repository = new UserRepository();
        var allUsers = await repository.GetAllAsync();
        var existingUser = allUsers.First();
        
        var user = await repository.GetByIdAsync(existingUser.Id);
        
        user.Should().NotBeNull();
        user!.Id.Should().Be(existingUser.Id);
        user.Email.Should().Be(existingUser.Email);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_User_Does_Not_Exist()
    {
        var repository = new UserRepository();
        var nonExistentId = Guid.NewGuid();
        
        var user = await repository.GetByIdAsync(nonExistentId);
        
        user.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_Should_Return_User_When_Email_Exists()
    {
        var repository = new UserRepository();
        var email = "john.doe@example.com";
        
        var user = await repository.GetByEmailAsync(email);
        
        user.Should().NotBeNull();
        user!.Email.Should().Be(email);
    }

    [Fact]
    public async Task GetByEmailAsync_Should_Return_Null_When_Email_Does_Not_Exist()
    {
        var repository = new UserRepository();
        var nonExistentEmail = "nonexistent@example.com";
        
        var user = await repository.GetByEmailAsync(nonExistentEmail);
        
        user.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_Should_Add_User_To_Repository()
    {
        var repository = new UserRepository();
        var newUser = User.CreatePending("new.user@example.com", "New", "User");
        
        var createdUser = await repository.CreateAsync(newUser);
        
        createdUser.Should().NotBeNull();
        createdUser.Should().Be(newUser);
        
        var allUsers = await repository.GetAllAsync();
        allUsers.Should().Contain(newUser);
    }

    [Fact]
    public async Task CreateAsync_Should_Increase_User_Count()
    {
        var repository = new UserRepository();
        var initialCount = (await repository.GetAllAsync()).Count();
        var newUser = User.CreatePending("another.user@example.com", "Another", "User");
        
        await repository.CreateAsync(newUser);
        
        var finalCount = (await repository.GetAllAsync()).Count();
        finalCount.Should().Be(initialCount + 1);
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_Updated_User()
    {
        var repository = new UserRepository();
        var user = User.CreatePending("update.test@example.com", "Update", "Test");
        await repository.CreateAsync(user);
        
        user.UpdateEmail("updated.email@example.com");
        var updatedUser = await repository.UpdateAsync(user);
        
        updatedUser.Should().NotBeNull();
        updatedUser!.Email.Should().Be("updated.email@example.com");
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_User_From_Repository()
    {
        var repository = new UserRepository();
        var user = User.CreatePending("delete.test@example.com", "Delete", "Test");
        await repository.CreateAsync(user);
        
        await repository.DeleteAsync(user.Id);
        
        var deletedUser = await repository.GetByIdAsync(user.Id);
        deletedUser.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_Should_Decrease_User_Count()
    {
        var repository = new UserRepository();
        var user = User.CreatePending("count.test@example.com", "Count", "Test");
        await repository.CreateAsync(user);
        var countBeforeDelete = (await repository.GetAllAsync()).Count();
        
        await repository.DeleteAsync(user.Id);
        
        var countAfterDelete = (await repository.GetAllAsync()).Count();
        countAfterDelete.Should().Be(countBeforeDelete - 1);
    }

    [Fact]
    public async Task DeleteAsync_Should_Throw_When_User_Does_Not_Exist()
    {
        var repository = new UserRepository();
        var nonExistentId = Guid.NewGuid();
        
        var act = async () => await repository.DeleteAsync(nonExistentId);
        
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("User not found");
    }
}
