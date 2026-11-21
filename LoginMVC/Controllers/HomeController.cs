using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoginMVC.Models;

namespace LoginMVC.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly LoginContext _context;

    public HomeController(LoginContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId))
        {
            return Forbid();
        }

        var usuario = await _context.Usuarios
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.IdUsuario == usuarioId);

        if (usuario == null)
        {
            return NotFound();
        }

        ViewBag.NombreEstudiante = usuario.Nombre;
        ViewBag.Correo = usuario.Correo;
        ViewBag.Nickname = usuario.Nickname;

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
