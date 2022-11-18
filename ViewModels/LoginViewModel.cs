using System.ComponentModel.DataAnnotations;

namespace JwtAuthServer.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Informe o Usuário")]
        [MinLength(11, ErrorMessage = "O usuário deve ter no mínimo 11 caracteres")]
        [MaxLength(11, ErrorMessage = "O usuário deve ter no máximo 11 caracteres")]
        public string Identifier { get; set; } = string.Empty;
    
        [Required(ErrorMessage = "Informe a senha")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
        [MaxLength(20, ErrorMessage = "A senha deve ter no máximo 20 caracteres")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}