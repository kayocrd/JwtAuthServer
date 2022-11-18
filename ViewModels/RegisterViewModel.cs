using System.ComponentModel.DataAnnotations;

namespace JwtAuthServer.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "O E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "O E-mail é inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O Username é obrigatório")]
    [MinLength(4, ErrorMessage = "O Identificador deve ter no mínimo 4 caracteres")]
    [MaxLength(15, ErrorMessage = "O Identificador deve ter no máximo 15 caracteres")]
    public string Identifier { get; set; } = string.Empty;

    [Required(ErrorMessage = "O número do celular é obrigatório")]
    [MinLength(11, ErrorMessage = "O número do celular deve ter no mínimo 11 caracteres")]
    [MaxLength(11, ErrorMessage = "O número do celular deve ter no máximo 11 caracteres")]
    [RegularExpression(@"^(\d{11})$", ErrorMessage = "O número do celular é inválido")]
    public string PhoneNumber { get; set; } = string.Empty;
}