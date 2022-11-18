using System.ComponentModel.DataAnnotations;

namespace JwtAuthServer.ViewModels;

public class RefreshViewModel
{
    [Required(ErrorMessage="Token is required")]
    public string Token { get; set; } = string.Empty;
    [Required(ErrorMessage="RefreshToken is required")]
    public string RefreshToken { get; set; } = string.Empty;
}