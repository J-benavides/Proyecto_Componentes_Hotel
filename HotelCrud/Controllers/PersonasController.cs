using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using HotelCrud.Models;

namespace HotelCrud.Controllers
{
    public class PersonasController : Controller
    {
        private readonly HotelCaliforniaDbContext _context;
        private readonly PasswordHasher<Persona> _passwordHasher = new PasswordHasher<Persona>();

        public PersonasController(HotelCaliforniaDbContext context)
        {
            _context = context;
        }

        // GET: Personas
        public async Task<IActionResult> Index()
        {
            var rol = HttpContext.Session.GetString("Rol");
            var idSesion = HttpContext.Session.GetInt32("IdPersona") ?? 0;

            if (rol == "Admin")
            {
                return View(await _context.Personas.ToListAsync());
            }
            else if (rol == "Cliente")
            {
                // Cliente no ve lista, lo redirigimos a su perfil para editar
                if (idSesion == 0) return RedirectToAction("AccessDenied", "Cuenta");
                return RedirectToAction("Edit", new { id = idSesion });
            }
            else
            {
                return RedirectToAction("AccessDenied", "Cuenta");
            }
        }

        // GET: Personas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var rol = HttpContext.Session.GetString("Rol");
            var idSesion = HttpContext.Session.GetInt32("IdPersona") ?? 0;

            if (id == null) return NotFound();

            if (rol != "Admin" && id != idSesion)
            {
                return RedirectToAction("AccessDenied", "Cuenta");
            }

            var persona = await _context.Personas.FirstOrDefaultAsync(m => m.Id == id);
            if (persona == null) return NotFound();

            return View(persona);
        }

        // GET: Personas/Create
        public IActionResult Create()
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Admin")
            {
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
                persona.Contrasena = _passwordHasher.HashPassword(persona, persona.Contrasena!);

                _context.Add(persona);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(persona);
        }

        // GET: Personas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var rol = HttpContext.Session.GetString("Rol");
            var idSesion = HttpContext.Session.GetInt32("IdPersona") ?? 0;

            if (id == null) return NotFound();

            if (rol != "Admin" && id != idSesion)
            {
                return RedirectToAction("AccessDenied", "Cuenta");
            }

            var persona = await _context.Personas.FindAsync(id);
            if (persona == null) return NotFound();

            var model = new PersonaEditViewModel
            {
                Id = persona.Id,
                Nombre = persona.Nombre,
                Apellidos = persona.Apellidos,
                Correo = persona.Correo,
                Telefono = persona.Telefono
            };

            return View(model);
        }

        // POST: Personas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PersonaEditViewModel model)
        {
            var rol = HttpContext.Session.GetString("Rol");
            var idSesion = HttpContext.Session.GetInt32("IdPersona") ?? 0;

            if (id != model.Id) return NotFound();

            if (rol != "Admin" && id != idSesion)
            {
                return RedirectToAction("AccessDenied", "Cuenta");
            }

            if (ModelState.IsValid)
            {
                var persona = await _context.Personas.FindAsync(id);
                if (persona == null) return NotFound();

                persona.Nombre = model.Nombre;
                persona.Apellidos = model.Apellidos;
                persona.Correo = model.Correo;
                persona.Telefono = model.Telefono;

                if (!string.IsNullOrEmpty(model.NuevaContrasena))
                {
                    if (string.IsNullOrEmpty(model.ContrasenaActual))
                    {
                        ModelState.AddModelError("ContrasenaActual", "La contraseña actual es requerida.");
                        return View(model);
                    }

                    var resultadoVerificacion = _passwordHasher.VerifyHashedPassword(persona, persona.Contrasena!, model.ContrasenaActual);

                    if (resultadoVerificacion == PasswordVerificationResult.Failed)
                    {
                        ModelState.AddModelError("ContrasenaActual", "La contraseña actual es incorrecta.");
                        return View(model);
                    }

                    persona.Contrasena = _passwordHasher.HashPassword(persona, model.NuevaContrasena);
                }

                try
                {
                    _context.Update(persona);
                    await _context.SaveChangesAsync();

                    if (!string.IsNullOrEmpty(model.NuevaContrasena))
                    {
                        HttpContext.Session.Clear(); // Forzar login con nueva contraseña
                        return RedirectToAction("Login", "Cuenta");
                    }

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
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Admin")
                return RedirectToAction("AccessDenied", "Cuenta");

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
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Admin")
                return RedirectToAction("AccessDenied", "Cuenta");

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
