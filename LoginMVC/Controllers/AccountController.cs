using System.Security.Claims;
using BCrypt.Net;
using LoginMVC.Models;
using LoginMVC.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoginMVC.Controllers;

public class AccountController : Controller
{
    private readonly LoginContext _context;

    public AccountController(LoginContext context)
    {
        _context = context;
    }

    // GET: /Account/Login
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        // Si el usuario ya está autenticado, redirigir al Dashboard / Home
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Buscar el usuario directamente en la base de datos por Nickname o Correo
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Nickname == model.Identificador || u.Correo == model.Identificador);

        if (usuario == null)
        {
            ModelState.AddModelError(string.Empty, "Credenciales incorrectas. Verifique su usuario/correo y contraseña.");
            return View(model);
        }

        // Validación de contraseña con BCrypt
        bool passwordValida = false;

        // Comprobar si el hash guardado tiene el formato de BCrypt ($2a$, $2b$, $2y$)
        if (usuario.Contrasena.StartsWith("$2"))
        {
            try
            {
                passwordValida = BCrypt.Net.BCrypt.Verify(model.Contrasena, usuario.Contrasena);
            }
            catch
            {
                passwordValida = false;
            }
        }
        else
        {
            // Migración automática: si venía en texto plano, validar y migrar a hash BCrypt seguro
            if (usuario.Contrasena == model.Contrasena)
            {
                passwordValida = true;
                // Ciframos con BCrypt y actualizamos la base de datos
                usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(model.Contrasena);
                await _context.SaveChangesAsync();
            }
        }

        if (!passwordValida)
        {
            ModelState.AddModelError(string.Empty, "Credenciales incorrectas. Verifique su usuario/correo y contraseña.");
            return View(model);
        }

        // Crear los Claims de autenticación del usuario
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Email, usuario.Correo),
            new Claim("Nickname", usuario.Nickname)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = model.Recordarme,
            ExpiresUtc = model.Recordarme ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddMinutes(60)
        };

        // Iniciar sesión con Cookie
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        // Redirigir a vista protegida (Dashboard)
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Dashboard");
    }

    // POST: /Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }

    // GET: /Account/AccessDenied
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
