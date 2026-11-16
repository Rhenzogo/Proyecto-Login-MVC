using System;
using System.ComponentModel.DataAnnotations;

namespace LoginMVC.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "El correo electrónico o nombre de usuario es obligatorio.")]
    [Display(Name = "Correo o Usuario")]
    public string Identificador { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Contrasena { get; set; } = string.Empty;

    [Display(Name = "Recordarme")]
    public bool Recordarme { get; set; }
}
