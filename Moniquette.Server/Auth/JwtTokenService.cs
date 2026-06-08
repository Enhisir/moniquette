using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Moniquette.Common.Models;

namespace Moniquette.Server.Auth;

public class JwtTokenService(IOptions<JwtOptions> options) : IJwtTokenService
{
    private readonly JwtOptions _options = options.Value;

    public string CreateToken(Session session)
    {
        ValidateOptions();

        var now = DateTimeOffset.UtcNow;
        var expires = now.AddMinutes(_options.TokenLifetimeMinutes);

        var header = new Dictionary<string, object>
        {
            ["alg"] = "HS256",
            ["typ"] = "JWT"
        };

        var payload = new Dictionary<string, object>
        {
            ["iss"] = _options.Issuer,
            ["aud"] = _options.Audience,
            ["sub"] = session.Id.ToString(),
            ["session_id"] = session.Id.ToString(),
            ["given_name"] = session.FirstName,
            ["family_name"] = session.LastName,
            ["iat"] = now.ToUnixTimeSeconds(),
            ["nbf"] = now.ToUnixTimeSeconds(),
            ["exp"] = expires.ToUnixTimeSeconds(),
            ["jti"] = Guid.NewGuid().ToString()
        };

        var headerPart = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(header));
        var payloadPart = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(payload));
        var signaturePart = Sign($"{headerPart}.{payloadPart}");

        return $"{headerPart}.{payloadPart}.{signaturePart}";
    }

    public JwtValidationResult ValidateToken(string token)
    {
        ValidateOptions();

        var parts = token.Split('.');
        if (parts.Length != 3)
        {
            return JwtValidationResult.Fail("Invalid JWT format.");
        }

        var expectedSignature = Sign($"{parts[0]}.{parts[1]}");
        if (!FixedTimeEquals(parts[2], expectedSignature))
        {
            return JwtValidationResult.Fail("Invalid JWT signature.");
        }

        JsonDocument payload;
        try
        {
            var payloadBytes = Base64UrlDecode(parts[1]);
            payload = JsonDocument.Parse(payloadBytes);
        }
        catch (Exception)
        {
            return JwtValidationResult.Fail("Invalid JWT payload.");
        }

        using (payload)
        {
            var root = payload.RootElement;
            if (!TryGetString(root, "iss", out var issuer) || issuer != _options.Issuer)
            {
                return JwtValidationResult.Fail("Invalid JWT issuer.");
            }

            if (!TryGetString(root, "aud", out var audience) || audience != _options.Audience)
            {
                return JwtValidationResult.Fail("Invalid JWT audience.");
            }

            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (!TryGetInt64(root, "nbf", out var notBefore) || now < notBefore)
            {
                return JwtValidationResult.Fail("JWT is not active yet.");
            }

            if (!TryGetInt64(root, "exp", out var expires) || now >= expires)
            {
                return JwtValidationResult.Fail("JWT is expired.");
            }

            if (!TryGetString(root, "session_id", out var sessionIdValue)
                || !Guid.TryParse(sessionIdValue, out var sessionId))
            {
                return JwtValidationResult.Fail("JWT does not contain a valid session_id.");
            }

            return JwtValidationResult.Success(sessionId);
        }
    }

    private string Sign(string value)
    {
        var secretBytes = Encoding.UTF8.GetBytes(_options.Secret);
        var valueBytes = Encoding.UTF8.GetBytes(value);
        using var hmac = new HMACSHA256(secretBytes);
        return Base64UrlEncode(hmac.ComputeHash(valueBytes));
    }

    private void ValidateOptions()
    {
        if (string.IsNullOrWhiteSpace(_options.Secret) || _options.Secret.Length < 32)
        {
            throw new InvalidOperationException("JWT secret must contain at least 32 characters.");
        }
    }

    private static bool TryGetString(JsonElement element, string propertyName, out string value)
    {
        value = string.Empty;
        return element.TryGetProperty(propertyName, out var property)
               && property.ValueKind == JsonValueKind.String
               && !string.IsNullOrWhiteSpace(value = property.GetString() ?? string.Empty);
    }

    private static bool TryGetInt64(JsonElement element, string propertyName, out long value)
    {
        value = default;
        return element.TryGetProperty(propertyName, out var property)
               && property.TryGetInt64(out value);
    }

    private static string Base64UrlEncode(byte[] bytes)
        => Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');

    private static byte[] Base64UrlDecode(string value)
    {
        var base64 = value
            .Replace('-', '+')
            .Replace('_', '/');

        base64 += (base64.Length % 4) switch
        {
            2 => "==",
            3 => "=",
            0 => string.Empty,
            _ => throw new FormatException("Invalid base64url length.")
        };

        return Convert.FromBase64String(base64);
    }

    private static bool FixedTimeEquals(string left, string right)
    {
        var leftBytes = Encoding.UTF8.GetBytes(left);
        var rightBytes = Encoding.UTF8.GetBytes(right);
        return leftBytes.Length == rightBytes.Length
               && CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
    }
}
