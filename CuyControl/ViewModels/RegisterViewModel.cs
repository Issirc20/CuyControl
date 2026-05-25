using System.ComponentModel.DataAnnotations;

namespace CuyControl.Web.ViewModels;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "Usuario (nombre de usuario)")]
    [StringLength(50, MinimumLength = 3)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Display(Name = "Nombre completo")]
    [StringLength(100)]
    public string? FullName { get; set; }

    [Display(Name = "Rol")]
    public string? Role { get; set; }
}
