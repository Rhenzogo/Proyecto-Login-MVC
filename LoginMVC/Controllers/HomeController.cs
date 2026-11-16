using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoginMVC.Models;

namespace LoginMVC.Controllers;

[Authorize]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        var nombre = User.Identity?.Name ?? "Estudiante";
        var correo = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value ?? "";
        var nickname = User.Claims.FirstOrDefault(c => c.Type == "Nickname")?.Value ?? "";

        ViewBag.NombreEstudiante = nombre;
        ViewBag.Correo = correo;
        ViewBag.Nickname = nickname;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
