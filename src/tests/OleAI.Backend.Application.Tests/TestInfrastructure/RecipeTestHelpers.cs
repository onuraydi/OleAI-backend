using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace OleAI.Backend.Application.Tests.TestInfrastructure;

internal static class RecipeTestHelpers
{
    public static DefaultHttpContext CreateHttpContext(Guid userId, params string[] roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString())
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var identity = new ClaimsIdentity(claims, authenticationType: "TestAuth");
        return new DefaultHttpContext
        {
            User = new ClaimsPrincipal(identity)
        };
    }

    public static IFormFile CreateFormFile(string content = "image-content", string fileName = "image.jpg", string name = "file", string contentType = "image/jpeg")
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);
        stream.Position = 0;

        var formFile = new FormFile(stream, 0, bytes.Length, name, fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };

        return formFile;
    }
}
