using System.ComponentModel.DataAnnotations;

namespace JwtAuthServerApi.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    public string FullName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "O E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "O E-mail é inválido")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "O Username é obrigatório")]
    [MinLength(4, ErrorMessage = "O Username deve ter no mínimo 4 caracteres")]
    [MaxLength(15, ErrorMessage = "O Username deve ter no máximo 15 caracteres")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória")]
    [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
    [MaxLength(15, ErrorMessage = "A senha deve ter no máximo 15 caracteres")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    [Required(ErrorMessage = "A confirmação de senha é obrigatória")]
    [Compare("Password", ErrorMessage = "As senhas não conferem")]
    public string ConfirmPassword { get; set; } = string.Empty;
}