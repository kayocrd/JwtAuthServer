using JwtAuthServerApi.Repositories;
using JwtAuthServerApi.Services;
using JwtAuthServerApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthServerApi.Controllers;

[ApiController]
[Route("[controller]/v1/auth")]
public class AccountController : ControllerBase
{
    [HttpGet]
    [Route("account")]
    public IActionResult Get()
    {
        return Ok("Hello World");
    }
    
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(
        [FromServices] IAccountService accountService,
        [FromBody] RegisterViewModel request)
    {
        var result = await accountService.Register(request);
        return Ok(new ResultViewModel<dynamic>(new { result }));
    }
    
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(
        [FromServices] TokenService tokenService,
        [FromServices] IAccountService accountService,
        [FromBody] LoginViewModel request)
    {
            
        var result = await accountService.Login(request);
        var token = tokenService.GenerateToken(result);
        var refreshToken = tokenService.GenerateRefreshToken();
        var expirationDate = DateTime.UtcNow.AddHours(Settings.ExpirationHours);
        await tokenService.SaveRefreshToken(result.Username, refreshToken, expirationDate);
        return Ok(new ResultViewModel<dynamic>(new {token, refreshToken, expirationDate}));
        
    }
    
    [HttpPost]
    [Route("refresh")]
    public async Task<IActionResult> Refresh(
        [FromServices] TokenService tokenService,
        [FromServices] IAccountService accountService,
        [FromBody] RefreshViewModel request)
    {
        var principal = tokenService.GetPrincipalFromExpiredToken(request.Token);
        var username = principal.Identity!.Name;
        var savedRefreshToken = await tokenService.GetRefreshToken(username!);
        if (savedRefreshToken != request.RefreshToken)
            throw new SecurityTokenException("Invalid refresh token");
        var newJwtToken = tokenService.GenerateToken(principal.Claims);
        var newRefreshToken = tokenService.GenerateRefreshToken();
        var expirationDate = DateTime.UtcNow.AddHours(Settings.ExpirationHours);
        await tokenService.SaveRefreshToken(username!, newRefreshToken, expirationDate);
        return Ok(new ResultViewModel<dynamic>(new {newJwtToken, newRefreshToken, expirationDate}));
    }
    
    [HttpPost]
    [Route("revoke")]
    public async Task<IActionResult> Revoke(string username, 
        [FromServices] IAccountService accountService,
        [FromServices] TokenService tokenService)
    {
        await tokenService.RevokeByUsername(username);
        return Ok("Token revoked");
    }
}