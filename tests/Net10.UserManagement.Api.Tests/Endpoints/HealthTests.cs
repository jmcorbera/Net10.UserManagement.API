using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Net10.UserManagement.Api.Tests.Endpoints;

public class HealthTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public HealthTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Health_Live_Should_Return_Ok()
    {
        var response = await _client.GetAsync("/health/live");
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Health_Ready_Should_Return_Ok()
    {
        var response = await _client.GetAsync("/health/ready");
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Health_Live_Should_Return_Healthy_Status()
    {
        var response = await _client.GetAsync("/health/live");
        
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        
        content.Should().Contain("Healthy");
    }

    [Fact]
    public async Task Health_Ready_Should_Return_Healthy_Status()
    {
        var response = await _client.GetAsync("/health/ready");
        
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        
        content.Should().Contain("Healthy");
    }

    [Fact]
    public async Task Health_Live_Should_Have_ContentType_TextPlain()
    {
        var response = await _client.GetAsync("/health/live");
        
        response.Content.Headers.ContentType?.MediaType.Should().Be("text/plain");
    }

    [Fact]
    public async Task Health_Ready_Should_Have_ContentType_TextPlain()
    {
        var response = await _client.GetAsync("/health/ready");
        
        response.Content.Headers.ContentType?.MediaType.Should().Be("text/plain");
    }
}
