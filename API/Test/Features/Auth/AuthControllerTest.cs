using System.Security.Claims;
using Features.Auth;
using Features.Auth.Constants;
using Infrastructure.Models.DB.Repositories;
using Features.Auth.Dto;
using Shared.Exceptions;
using Features.Auth.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Test.Features.Auth;

public class AuthControllerTest : TestBase
{
    [Fact]
    public async Task LoginAsyncTestSuccess()
    {
        var authService = CreateAuthService();
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
        Assert.Contains(AuthCookie.RefreshName, controller.Response.Headers.SetCookie.ToString());
    }

    [Fact]
    public async Task LoginAsyncTestFailure()
    {
        var authService = CreateAuthService();
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
    public async Task RefreshAsyncTestSuccess()
    {
        var authService = CreateAuthService();
        var controller = CreateController(authService);
        var loginRequest = new LoginRequestDto
        {
            UserName = "admin",
            Password = "admin"
        };
        var loginResult = await authService.LoginAsync(loginRequest);
        controller.Request.Headers.Cookie = $"{AuthCookie.RefreshName}={loginResult.RefreshToken}";

        var result = await controller.RefreshAsync();

        var content = Assert.IsType<LoginResponseDto>(result.Value);
        Assert.Equal("admin", content.UserName);
        Assert.Contains(AuthCookie.Name, controller.Response.Headers.SetCookie.ToString());
        Assert.Contains(AuthCookie.RefreshName, controller.Response.Headers.SetCookie.ToString());
    }

    [Fact]
    public async Task RefreshAsyncTestUnauthorizedWhenRefreshCookieMissing()
    {
        var authService = CreateAuthService();
        var controller = CreateController(authService);

        var exception = await Assert.ThrowsAsync<UnauthorizedException>
        (
            controller.RefreshAsync
        );

        Assert.Equal("Unauthorized.", exception.Error.Message);
    }

    [Fact]
    public void GetMeTest()
    {
        var authService = CreateAuthService();
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
        var authService = CreateAuthService();
        var controller = CreateController(authService);

        var exception = Assert.Throws<UnauthorizedException>
        (
            controller.GetMe
        );

        Assert.Equal("Unauthorized.", exception.Error.Message);
    }

    [Fact]
    public async Task LogoutTest()
    {
        var authService = CreateAuthService();
        var controller = CreateController(authService);

        var result = await controller.LogoutAsync();

        Assert.IsType<NoContentResult>(result);
        Assert.Contains(AuthCookie.Name, controller.Response.Headers.SetCookie.ToString());
        Assert.Contains(AuthCookie.RefreshName, controller.Response.Headers.SetCookie.ToString());
    }

    private static AuthController CreateController(IAuthService authService)
    {
        var controller = new AuthController(authService)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
        return controller;
    }

    private IAuthService CreateAuthService()
    {
        var authRepository = new AuthRepository(DbContext);
        return new AuthService(Configuration, authRepository);
    }
}
