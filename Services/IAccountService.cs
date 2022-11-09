using JwtAuthServerApi.Models;
using JwtAuthServerApi.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthServerApi.Services;

public interface IAccountService
{
    public Task<User> Register(RegisterViewModel request);
    public Task<User> Login(LoginViewModel request);
}