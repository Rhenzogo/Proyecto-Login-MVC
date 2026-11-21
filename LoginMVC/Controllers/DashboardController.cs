using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoginMVC.Models;

namespace LoginMVC.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly LoginContext _context;

    public DashboardController(LoginContext context)
    {
        _context = context;
    }

    // GET: /Dashboard/Index
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

        ViewBag.NombreCompleto = usuario.Nombre;
        ViewBag.Correo = usuario.Correo;
        ViewBag.Nickname = usuario.Nickname;

        return View();
    }
}
