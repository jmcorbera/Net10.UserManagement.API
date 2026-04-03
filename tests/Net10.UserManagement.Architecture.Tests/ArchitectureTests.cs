using NetArchTest.Rules;

namespace Net10.UserManagement.Architecture.Tests;

public class ArchitectureTests
{
    private const string UserManagementApplication = "Net10.UserManagement.Application";
    private const string UserManagementInfrastructure = "Net10.UserManagement.Infrastructure";
    private const string UserManagementApi = "Net10.UserManagement.Api";
    private const string UserManagementDomain = "Net10.UserManagement.Domain";
    
    [Fact]
    public void Domain_Should_Not_Depend_On_AnyOtherProject()
    {
        var domainAssembly = Types.InAssembly(typeof(Net10.UserManagement.Domain.AssemblyReference).Assembly);

        var dependsOnApplication = domainAssembly
            .ShouldNot()
            .HaveDependencyOnAny([UserManagementApplication, UserManagementInfrastructure, UserManagementApi])
            .GetResult();

        Assert.True(dependsOnApplication.IsSuccessful, "Domain layer should not depend on Application, Infrastructure or API");
    }

    [Fact]
    public void Application_Should_Not_Depend_On_API_Or_Infrastructure()
    {
        var applicationAssembly = Types.InAssembly(typeof(Net10.UserManagement.Application.AssemblyReference).Assembly);

        var dependsOnApi = applicationAssembly
            .ShouldNot()
            .HaveDependencyOnAny([UserManagementApi, UserManagementInfrastructure])
            .GetResult();

        Assert.True(dependsOnApi.IsSuccessful, "Application layer should not depend on API or Infrastructure");
    }

    [Fact]
    public void Infrastructure_Should_Not_Depend_On_API()
    {
        var result = Types.InAssembly(typeof(Net10.UserManagement.Infrastructure.AssemblyReference).Assembly)
            .ShouldNot()
            .HaveDependencyOn(UserManagementApi)
            .GetResult();
            
        Assert.True(result.IsSuccessful, "Infrastructure layer should not depend on API");
    }

    [Fact]
    public void API_Should_Not_Depend_On_Infrastructure_Or_Domain()
    {
        var dependsOnInfrastructure = Types.InAssembly(typeof(Net10.UserManagement.Api.Program).Assembly)
            .That()
            .DoNotHaveName("ServiceCollectionExtensions")
            .ShouldNot()
            .HaveDependencyOnAny([UserManagementInfrastructure, UserManagementDomain])
            .GetResult();

        Assert.True(dependsOnInfrastructure.IsSuccessful, "API layer should not depend on Infrastructure or Domain");
    }
}
