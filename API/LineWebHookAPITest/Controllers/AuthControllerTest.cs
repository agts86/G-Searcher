using System.Security.Claims;
using LineWebHookAPI.Constants.Auth;
using LineWebHookAPI.Controllers;
using LineWebHookAPI.Models.Dto.Auth;
using LineWebHookAPI.Models.Exceptions;
using LineWebHookAPI.Models.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace LineWebHookAPITest.Controllers;

public class AuthControllerTest : TestBase
{
    [Fact]
    public async Task LoginAsyncTestSuccess()
    {
        var authService = new AuthService(CreateConfiguration());
        var controller = CreateController(authService);
        var request = new LoginRequestDto
        {
            UserName = "admin",
            Password = "admin"
        };

        var result = await controller.LoginAsync(request);

        var content = Assert.IsType<LoginResponseDto>(result.Value);
        Assert.Equal("admin", content.UserName);
        Assert.Contains(AuthCookie.Name, controller.Response.Headers.SetCookie.ToString());
    }

    [Fact]
    public async Task LoginAsyncTestFailure()
    {
        var authService = new AuthService(CreateConfiguration());
        var controller = CreateController(authService);
        var request = new LoginRequestDto
        {
            UserName = "admin",
            Password = "invalid"
        };

        var exception = await Assert.ThrowsAsync<UnauthorizedException>
        (
            () => controller.LoginAsync(request)
        );

        Assert.Equal("Invalid user name or password.", exception.Error.Message);
        Assert.Equal(string.Empty, controller.Response.Headers.SetCookie.ToString());
    }

    [Fact]
    public void GetMeTest()
    {
        var authService = new AuthService(CreateConfiguration());
        var controller = CreateController(authService);
        var claims = new List<Claim>
        {
            new (ClaimTypes.Name, "admin")
        };
        controller.HttpContext.User = new ClaimsPrincipal
        (
            new ClaimsIdentity(claims, "test")
        );

        var result = controller.GetMe();

        var content = Assert.IsType<MeResponseDto>(result.Value);
        Assert.Equal("admin", content.UserName);
    }

    [Fact]
    public void GetMeTestUnauthorized()
    {
        var authService = new AuthService(CreateConfiguration());
        var controller = CreateController(authService);

        var exception = Assert.Throws<UnauthorizedException>
        (
            () => controller.GetMe()
        );

        Assert.Equal("Unauthorized.", exception.Error.Message);
    }

    [Fact]
    public void LogoutTest()
    {
        var authService = new AuthService(CreateConfiguration());
        var controller = CreateController(authService);

        var result = controller.Logout();

        Assert.IsType<NoContentResult>(result);
        Assert.Contains(AuthCookie.Name, controller.Response.Headers.SetCookie.ToString());
    }

    private static AuthController CreateController(IAuthService authService)
    {
        var controller = new AuthController(authService);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        return controller;
    }

    private static IConfiguration CreateConfiguration()
    {
        var values = new Dictionary<string, string>
        {
            { "Auth:AdminUserName", "admin" },
            { "Auth:AdminPassword", "admin" },
            { "Auth:JwtKey", "DevelopmentOnlyJwtKeyMustBeAtLeast32Chars" },
            { "Auth:Issuer", "LineWebHookAPI" },
            { "Auth:Audience", "LineWebHookAdmin" },
            { "Auth:ExpiresMinutes", "120" }
        };
        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }
}
