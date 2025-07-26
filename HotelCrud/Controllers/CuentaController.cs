using HotelCrud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Linq;

public class CuentaController : Controller
{
    private readonly HotelCaliforniaDbContext _context;
    private readonly PasswordHasher<Persona> _passwordHasher = new PasswordHasher<Persona>();

    public CuentaController(HotelCaliforniaDbContext context)
    {
        _context = context;
    }

    // GET: Registro
    [HttpGet]
    public IActionResult Registro()
    {
        return View();
    }

    // POST: Registro
    [HttpPost]
    public IActionResult Registro(Persona persona)
    {
        if (_context.Personas.Any(p => p.Correo == persona.Correo))
        {
            ViewBag.ErrorRegistro = "Ya existe un usuario con ese correo.";
            return View(persona);
        }

        if (ModelState.IsValid)
        {
            persona.Contrasena = _passwordHasher.HashPassword(persona, persona.Contrasena ?? "");
            _context.Personas.Add(persona);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }
        return View(persona);
    }


    // GET: Login
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // POST: Login
    [HttpPost]
    public IActionResult Login(string correo, string contrasena)
    {
        var user = _context.Personas
            .Include(p => p.PersonaRols)
                .ThenInclude(pr => pr.IdRolNavigation)
            .FirstOrDefault(p => p.Correo == correo);

        if (user != null)
        {
            bool passwordValid;

            // Detecta si la contraseña almacenada está hasheada
            bool esHash = !string.IsNullOrEmpty(user.Contrasena) && user.Contrasena.Length > 50;

            if (esHash)
            {
                var resultado = _passwordHasher.VerifyHashedPassword(user, user.Contrasena!, contrasena);
                passwordValid = resultado == PasswordVerificationResult.Success;
            }
            else
            {
                // Comparación directa para contraseñas antiguas en texto plano
                passwordValid = user.Contrasena == contrasena;

                if (passwordValid)
                {
                    // Si es válida, actualiza la contraseña a una versión segura
                    user.Contrasena = _passwordHasher.HashPassword(user, contrasena);
                    _context.Update(user);
                    _context.SaveChanges();
                }
            }

            if (passwordValid)
            {
                HttpContext.Session.SetInt32("IdPersona", user.Id);
                HttpContext.Session.SetString("Nombre", user.Nombre);
                var rolNombre = user.PersonaRols.FirstOrDefault()?.IdRolNavigation.Nombre ?? "Cliente";
                HttpContext.Session.SetString("Rol", rolNombre);

                return RedirectToAction("Index", "Home");
            }
        }

        ViewBag.Error = "Correo o contraseña incorrectos.";
        return View();
    }

    // Logout
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
