using System.Security.Claims;
using BCrypt.Net;
using LoginMVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoginMVC.Controllers;

public class AccessController : Controller
{
    private readonly LoginContext _context;

    public AccessController(LoginContext context)
    {
        _context = context;
    }

    // HTTP GET: /Access/Login
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        // Si el usuario ya está autenticado, redirigir a la pantalla principal
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    // HTTP POST: /Access/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string nickname, string password, bool keepLoggedIn = false, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        // Validar que los campos requeridos no vengan vacíos
        if (string.IsNullOrWhiteSpace(nickname) || string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error = "Por favor ingrese su usuario o correo y contraseña.";
            return View();
        }

        // Consultar el usuario en la BD vía EF Core (por Nickname o por Correo)
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Nickname == nickname || u.Correo == nickname);

        if (usuario == null)
        {
            ViewBag.Error = "Credenciales incorrectas. Verifique su usuario y contraseña.";
            return View();
        }

        // Verificar la contraseña con BCrypt
        bool passwordValida = false;

        if (usuario.Contrasena.StartsWith("$2"))
        {
            try
            {
                passwordValida = BCrypt.Net.BCrypt.Verify(password, usuario.Contrasena);
            }
            catch
            {
                passwordValida = false;
            }
        }
        else
        {
            // Migración automática del en texto plano
            if (usuario.Contrasena == password)
            {
                passwordValida = true;
                usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(password);
                await _context.SaveChangesAsync();
            }
        }

        if (!passwordValida)
        {
            ViewBag.Error = "Credenciales incorrectas. Verifique su usuario y contraseña.";
            return View();
        }

        // Crear la identidad y los Claims del usuario autenticado
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
            IsPersistent = keepLoggedIn,
            ExpiresUtc = keepLoggedIn ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddMinutes(60)
        };

        // Iniciar sesión con Cookie
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        // Redirección a la vista protegida (Home / Dashboard)
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    // Acción Logout: Destruir cookies de sesión activos y redirigir al Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Access");
    }

    [HttpGet]
    public async Task<IActionResult> Logout(string? redirect = null)
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Access");
    }
}
