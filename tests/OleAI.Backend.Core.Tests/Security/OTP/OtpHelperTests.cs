using FluentAssertions;
using OleAI.Backend.Core.Security.OTP;
using Xunit;

namespace OleAI.Backend.Core.Tests.Security.OTP;

public class OtpHelperTests
{
    [Fact]
    public void GenerateSixDigitCode_ShouldReturnSixCharacterString()
    {
        // Act
        var code = OtpHelper.GenerateSixDigitCode();

        // Assert
        code.Should().NotBeNullOrEmpty();
        code.Length.Should().Be(6);
    }

    [Fact]
    public void GenerateSixDigitCode_ShouldBeWithinValidRange()
    {
        // Act
        var code = OtpHelper.GenerateSixDigitCode();

        // Assert – must be between 100000 and 999999 (inclusive)
        int parsed = int.Parse(code);
        parsed.Should().BeGreaterThanOrEqualTo(100_000)
              .And.BeLessThanOrEqualTo(999_999);
    }

    [Fact]
    public void GenerateSixDigitCode_ShouldReturnNumericStringOnly()
    {
        // Act
        var code = OtpHelper.GenerateSixDigitCode();

        // Assert
        code.Should().MatchRegex(@"^\d{6}$");
    }

    [Fact]
    public void GenerateSixDigitCode_MultipleCallsShouldProduceVariedResults()
    {
        // Generate a batch of codes — statistically they must not all be identical.
        // RandomNumberGenerator is cryptographically random, so identical values across
        // 50 calls would be astronomically unlikely (~0% probability).
        var codes = Enumerable.Range(0, 50)
            .Select(_ => OtpHelper.GenerateSixDigitCode())
            .ToList();

        // At least two distinct values must exist
        codes.Distinct().Count().Should().BeGreaterThan(1);
    }
}
