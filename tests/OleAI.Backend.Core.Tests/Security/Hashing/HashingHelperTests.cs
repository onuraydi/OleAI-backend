using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using OleAI.Backend.Core.Security.Hashing;
using Xunit;

namespace OleAI.Backend.Core.Tests.Security.Hashing;

public class HashingHelperTests
{
    [Fact]
    public void CreatePasswordHash_ShouldGenerateNonNullHashAndSalt()
    {
        // Act
        HashingHelper.CreatePasswordHash("TestPassword123!", out byte[] hash, out byte[] salt);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        salt.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void CreatePasswordHash_ShouldGenerateDifferentSaltEachCall()
    {
        // Act – two separate hashes for the same password
        HashingHelper.CreatePasswordHash("SamePassword", out byte[] hash1, out byte[] salt1);
        HashingHelper.CreatePasswordHash("SamePassword", out byte[] hash2, out byte[] salt2);

        // Assert – HMACSHA512 uses a new random key each time, so salts must differ
        salt1.Should().NotEqual(salt2);
        hash1.Should().NotEqual(hash2);
    }

    [Fact]
    public void VerifyPasswordHash_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        const string password = "MySecurePassword@42";
        HashingHelper.CreatePasswordHash(password, out byte[] hash, out byte[] salt);

        // Act
        var result = HashingHelper.VerifyPasswordHash(password, hash, salt);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPasswordHash_WithWrongPassword_ShouldReturnFalse()
    {
        // Arrange
        HashingHelper.CreatePasswordHash("CorrectPassword", out byte[] hash, out byte[] salt);

        // Act
        var result = HashingHelper.VerifyPasswordHash("WrongPassword", hash, salt);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPasswordHash_WithTamperedHash_ShouldReturnFalse()
    {
        // Arrange
        const string password = "TamperTest";
        HashingHelper.CreatePasswordHash(password, out byte[] hash, out byte[] salt);

        // Tamper: flip a single byte in the hash
        hash[0] ^= 0xFF;

        // Act
        var result = HashingHelper.VerifyPasswordHash(password, hash, salt);

        // Assert – timing-safe comparison must still return false
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPasswordHash_HashIsLengthOf64Bytes_ShouldBeExpectedHmacSha512Size()
    {
        // HMAC-SHA512 output is always 64 bytes
        HashingHelper.CreatePasswordHash("AnyPassword", out byte[] hash, out _);

        hash.Length.Should().Be(64);
    }

    [Fact]
    public void VerifyPasswordHash_WithLegacySaltLength128_ShouldUseLegacyVerificationPath()
    {
        // Arrange
        const string password = "LegacyPassword";
        var legacySalt = RandomNumberGenerator.GetBytes(128);
        using var hmac = new HMACSHA512(legacySalt);
        var legacyHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        // Act
        var result = HashingHelper.VerifyPasswordHash(password, legacyHash, legacySalt);

        // Assert
        result.Should().BeTrue();
    }
}
