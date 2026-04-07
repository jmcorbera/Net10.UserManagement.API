using FluentAssertions;
using Moq;
using Net10.UserManagement.Application.Common.Abstractions;
using Net10.UserManagement.Infrastructure.Services;

namespace Net10.UserManagement.Infrastructure.Tests.Services;

public class SecureOtpGeneratorTests
{
    [Fact]
    public void Generate_Should_Return_6_Digit_Code_By_Default()
    {
        var optionsMock = new Mock<IOtpSettingsProvider>();
        optionsMock.Setup(o => o.Length).Returns(6);
        
        var generator = new SecureOtpGenerator(optionsMock.Object);

        var code = generator.Generate();

        code.Should().NotBeNullOrEmpty();
        code.Length.Should().Be(6);
    }

    [Fact]
    public void Generate_Should_Return_Only_Numeric_Characters()
    {
        var optionsMock = new Mock<IOtpSettingsProvider>();
        optionsMock.Setup(o => o.Length).Returns(6);
        
        var generator = new SecureOtpGenerator(optionsMock.Object);

        var code = generator.Generate();

        code.Should().MatchRegex(@"^\d+$");
    }

    [Fact]
    public void Generate_Should_Return_Different_Codes_On_Multiple_Calls()
    {
        var optionsMock = new Mock<IOtpSettingsProvider>();
        optionsMock.Setup(o => o.Length).Returns(6);
        
        var generator = new SecureOtpGenerator(optionsMock.Object);

        var code1 = generator.Generate();
        var code2 = generator.Generate();
        var code3 = generator.Generate();

        code1.Should().NotBe(code2);
        code2.Should().NotBe(code3);
        code1.Should().NotBe(code3);
    }

    [Fact]
    public void Generate_Should_Respect_Custom_Length()
    {
        var optionsMock = new Mock<IOtpSettingsProvider>();
        optionsMock.Setup(o => o.Length).Returns(8);
        
        var generator = new SecureOtpGenerator(optionsMock.Object);

        var code = generator.Generate();

        code.Length.Should().Be(8);
    }

    [Fact]
    public void Generate_Should_Always_Return_Valid_Numeric_String()
    {
        var optionsMock = new Mock<IOtpSettingsProvider>();
        optionsMock.Setup(o => o.Length).Returns(6);
        
        var generator = new SecureOtpGenerator(optionsMock.Object);

        for (int i = 0; i < 10; i++)
        {
            var code = generator.Generate();
            code.Should().MatchRegex(@"^\d+$");
            code.Length.Should().Be(6);
        }
    }
}
