using System.ComponentModel.DataAnnotations;

namespace JwtAuthServerApi.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Informe o Usuário")]
        public string Username { get; set; } = string.Empty;
    
        [Required(ErrorMessage = "Informe a senha")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}