using HotelCrud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class CuentaController : Controller
{
    private readonly HotelCaliforniaDbContext _context;

    public CuentaController(HotelCaliforniaDbContext context)
    {
        _context = context;
    }

    // REGISTRO
    [HttpGet]
    public IActionResult Registro()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Registro(Persona persona)
    {
        if (ModelState.IsValid)
        {
            _context.Personas.Add(persona);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }
        return View(persona);
    }

    // LOGIN
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string correo, string contrasena)
    {
        // Busca el usuario incluyendo los roles asignados
        var user = _context.Personas
            .Include(p => p.PersonaRols)
                .ThenInclude(pr => pr.IdRolNavigation)
            .FirstOrDefault(p => p.Correo == correo && p.Contrasena == contrasena);

        if (user != null)
        {
            // Guardar datos en sesión
            HttpContext.Session.SetInt32("IdPersona", user.Id);
            HttpContext.Session.SetString("Nombre", user.Nombre);

            // Obtener el nombre del primer rol asignado (o "Cliente" por defecto)
            var rolNombre = user.PersonaRols.FirstOrDefault()?.IdRolNavigation.Nombre ?? "Cliente";
            HttpContext.Session.SetString("Rol", rolNombre);

            return RedirectToAction("Index", "Home");
        }

        // Si no encontró usuario o contraseña incorrecta, muestra error
        ViewBag.Error = "Correo o contraseña incorrectos.";
        return View();
    }


    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
