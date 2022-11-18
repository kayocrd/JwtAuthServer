using System.Security.Claims;
using JwtAuthServer.Extensions;
using JwtAuthServer.Services;
using JwtAuthServer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthServer.Controllers;

[ApiController]
[Route("/v1/auth")]
public class AccountController : ControllerBase
{
    [HttpGet]
    [Route("/")]
    public IActionResult Get()
    {
        return Ok("Hello World");
    }
    
    [HttpPost]
    [AllowAnonymous]
    [Route("register")]
    public async Task<IActionResult> Register(
        [FromServices] IAccountService accountService,
        [FromServices] TokenService tokenService,
        [FromBody] RegisterViewModel request)
    {
        if (!ModelState.IsValid) return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));
        try
        {
            var result = await accountService.Register(request);
            var token = tokenService.GenerateToken(result);
            return Ok(new ResultViewModel<dynamic>(new { User = result, Token = token }));
        }
        catch (Exception e)
        {
            return BadRequest(new ResultViewModel<string>(e.Message));
        }
    }
    
    [HttpPost]
    [Authorize("PendingVerification")]
    [Route("SendVerificationCode")]
    public async Task<IActionResult> SendVerificationCode(
        [FromServices] IAccountService accountService,
        [FromServices] IWhatsappService sender,
        [FromQuery] string? newPhoneNumber)
    {
        if (!ModelState.IsValid) return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));
        try
        {
            var phoneNumber = User.Claims.First(x => x.Type == ClaimTypes.MobilePhone).Value;
            if (newPhoneNumber != null) phoneNumber = newPhoneNumber;

            var user = await accountService.GetAccount(User.Identity.Name);
            await accountService.GenerateAndSendVerificationCode(user.Identifier, phoneNumber);
            return Ok(new ResultViewModel<dynamic>(new { Message = "Senha Enviada com sucesso" }));
        }
        catch (Exception e)
        {
            return BadRequest(new ResultViewModel<string>(e.Message));
        }
    }
        
    [HttpPost]
    [Route("validateAccount")]
    [Authorize("PendingVerification")]
    public async Task<IActionResult> ValidateAccount(
        [FromServices] IWhatsappService sender,
        [FromServices] IAccountService accountService,
        [FromServices] TokenService tokenService,
        [FromBody]  VerifyAccountViewModel request)
    {
        if (!ModelState.IsValid) return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));
        try
        {
            var user = await accountService.GetAccount(User.Identity.Name);
            
            var result = await accountService.VerifyAccount(user.Identifier, request.PhoneNumber, request.Code, request.Password);
            var token = tokenService.GenerateToken(result);
            var refreshToken = tokenService.GenerateRefreshToken();
            var expirationDate = DateTime.UtcNow.AddHours(Settings.ExpirationHours);
            await accountService.SaveRefreshToken(result.Identifier, refreshToken, expirationDate);
            return Ok(new ResultViewModel<dynamic>(new { User = result, Token = token, refreshToken = refreshToken, ExpirationDate = expirationDate }));
        }
        catch (Exception e)
        {
            return BadRequest(new ResultViewModel<string>(e.Message));
        }
    }
    
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(
        [FromServices] TokenService tokenService,
        [FromServices] IAccountService accountService,
        [FromBody] LoginViewModel request)
    {
        if (!ModelState.IsValid) return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));
        try
        {
            var result = await accountService.Login(request);
            var token = tokenService.GenerateToken(result);
            var refreshToken = tokenService.GenerateRefreshToken();
            var expirationDate = DateTime.UtcNow.AddHours(Settings.ExpirationHours);
            await tokenService.SaveRefreshToken(result.Identifier, refreshToken, expirationDate);
            return Ok(new ResultViewModel<dynamic>(new {token, refreshToken, expirationDate}));
        }
        catch (Exception e)
        {
            return BadRequest(new ResultViewModel<string>(e.Message));
        }
    }
    
    [HttpPost]
    [Route("refresh")]
    public async Task<IActionResult> Refresh(
        [FromServices] TokenService tokenService,
        [FromServices] IAccountService accountService,
        [FromBody] RefreshViewModel request)
    {
        var principal = tokenService.GetPrincipalFromExpiredToken(request.Token);
        var identifier = principal.Identity!.Name;
        var savedRefreshToken = await tokenService.GetRefreshToken(identifier!);
        if (savedRefreshToken != request.RefreshToken)
            throw new SecurityTokenException("Invalid refresh token");
        var newJwtToken = tokenService.GenerateToken(principal.Claims);
        var newRefreshToken = tokenService.GenerateRefreshToken();
        var expirationDate = DateTime.UtcNow.AddHours(Settings.ExpirationHours);
        await tokenService.SaveRefreshToken(identifier!, newRefreshToken, expirationDate);
        return Ok(new ResultViewModel<dynamic>(new {newJwtToken, newRefreshToken, expirationDate}));
    }
    
    [HttpPost]
    [Route("revoke")]
    public async Task<IActionResult> Revoke(string identifier, 
        [FromServices] IAccountService accountService,
        [FromServices] TokenService tokenService)
    {
        await tokenService.RevokeByIdentifier(identifier);
        return Ok("Token revoked");
    }
    
}