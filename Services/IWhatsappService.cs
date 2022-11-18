using Microsoft.AspNetCore.Mvc;

namespace JwtAuthServer.Services;

public interface IWhatsappService
{
    Task<HttpResponseMessage> SendWhatsappMessage(string phoneNumber, string message);
}