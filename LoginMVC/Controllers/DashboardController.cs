using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoginMVC.Controllers;

[Authorize]
public class DashboardController : Controller
{
    // GET: /Dashboard/Index
    public IActionResult Index()
    {
        // Obtenemos los datos del usuario autenticado a través de sus Claims
        var nombreUsuario = User.Identity?.Name ?? "Estudiante";
        var correoUsuario = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value ?? "";
        var nickname = User.Claims.FirstOrDefault(c => c.Type == "Nickname")?.Value ?? "";

        ViewBag.NombreCompleto = nombreUsuario;
        ViewBag.Correo = correoUsuario;
        ViewBag.Nickname = nickname;

        return View();
    }
}
