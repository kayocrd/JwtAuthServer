using System.ComponentModel.DataAnnotations;

namespace JwtAuthServer.ViewModels;

public class VerifyAccountViewModel
{
    [MinLength(11, ErrorMessage = "O número do celular deve ter no mínimo 11 caracteres")]
    [MaxLength(11, ErrorMessage = "O número do celular deve ter no máximo 11 caracteres")]
    [RegularExpression(@"^(\d{11})$", ErrorMessage = "O número do celular é inválido")]
    public string? PhoneNumber { get; set; }
    [Required(ErrorMessage = "O Código de verificação é obrigatório")]
    [MinLength(6, ErrorMessage = "O Código de verificação deve ter no mínimo 6 caracteres")]
    [MaxLength(6, ErrorMessage = "O Código de verificação deve ter no máximo 6 caracteres")]
    [RegularExpression(@"^[0-9]*$", ErrorMessage = "O Código de verificação deve conter apenas números")]
    public string Code { get; set; } = string.Empty; 
    
    [Required]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
    [MaxLength(20, ErrorMessage = "A senha deve ter no máximo 20 caracteres")]
    public string Password { get; set; } = string.Empty;
    [Required(ErrorMessage = "A confirmação de senha é obrigatória")]
    [Compare(nameof(Password), ErrorMessage = "As senhas não conferem")]
    public string ConfirmPassword { get; set; } = string.Empty;

}