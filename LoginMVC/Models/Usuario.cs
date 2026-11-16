using System;
using System.Collections.Generic;

namespace LoginMVC.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string Nickname { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Contrasena { get; set; } = null!;
}
