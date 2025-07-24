using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; // para HttpContext.Session
using HotelCrud.Models;

namespace HotelCrud.Controllers
{
    public class PersonasController : Controller
    {
        private readonly HotelCaliforniaDbContext _context;

        public PersonasController(HotelCaliforniaDbContext context)
        {
            _context = context;
        }

        // GET: Personas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Personas.ToListAsync());
        }

        // GET: Personas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var persona = await _context.Personas.FirstOrDefaultAsync(m => m.Id == id);
            if (persona == null) return NotFound();

            return View(persona);
        }

        // GET: Personas/Create
        public IActionResult Create()
        {
            // Verificar rol en sesión
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Admin")
            {
                // Redirigir a página de acceso denegado
                return RedirectToAction("AccessDenied", "Cuenta");
            }
            return View();
        }

        // POST: Personas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellidos,Correo,Telefono,Contrasena")] Persona persona)
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Admin")
            {
                return RedirectToAction("AccessDenied", "Cuenta");
            }

            if (ModelState.IsValid)
            {
                _context.Add(persona);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(persona);
        }

        // GET: Personas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var persona = await _context.Personas.FindAsync(id);
            if (persona == null) return NotFound();

            var model = new PersonaEditViewModel
            {
                Id = persona.Id,
                Nombre = persona.Nombre,
                Apellidos = persona.Apellidos,
                Correo = persona.Correo,
                Telefono = persona.Telefono
                // No enviamos contraseña por seguridad
            };

            return View(model);
        }

        // POST: Personas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PersonaEditViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var persona = await _context.Personas.FindAsync(id);
                if (persona == null) return NotFound();

                persona.Nombre = model.Nombre;
                persona.Apellidos = model.Apellidos;
                persona.Correo = model.Correo;
                persona.Telefono = model.Telefono;

                // Cambiar contraseña si se especifica y la actual es correcta
                if (!string.IsNullOrEmpty(model.NuevaContrasena))
                {
                    if (string.IsNullOrEmpty(model.ContrasenaActual) || persona.Contrasena != model.ContrasenaActual)
                    {
                        ModelState.AddModelError("ContrasenaActual", "La contraseña actual es incorrecta.");
                        return View(model);
                    }
                    persona.Contrasena = model.NuevaContrasena;
                }

                try
                {
                    _context.Update(persona);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonaExists(persona.Id)) return NotFound();
                    else throw;
                }
            }

            return View(model);
        }

        // GET: Personas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var persona = await _context.Personas.FirstOrDefaultAsync(m => m.Id == id);
            if (persona == null) return NotFound();

            return View(persona);
        }

        // POST: Personas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var persona = await _context.Personas.FindAsync(id);
                if (persona == null)
                {
                    TempData["MensajeError"] = "La persona no existe.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Personas.Remove(persona);
                await _context.SaveChangesAsync();

                TempData["MensajeExito"] = "La persona se eliminó correctamente.";
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("FK__PersonaRol"))
                {
                    TempData["MensajeError"] = "No se puede eliminar la persona porque tiene un rol asignado.";
                }
                else if (ex.InnerException != null && ex.InnerException.Message.Contains("FK__Reserva"))
                {
                    TempData["MensajeError"] = "No se puede eliminar la persona porque tiene una reserva activa.";
                }
                else
                {
                    TempData["MensajeError"] = "Ocurrió un error al intentar eliminar la persona.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PersonaExists(int id)
        {
            return _context.Personas.Any(e => e.Id == id);
        }
    }
}
