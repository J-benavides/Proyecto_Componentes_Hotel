using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelCrud.Models;

namespace HotelCrud.Controllers
{
    public class ReservasController : Controller
    {
        private readonly HotelCaliforniaDbContext _context;

        public ReservasController(HotelCaliforniaDbContext context)
        {
            _context = context;
        }

        // GET: Reservas
        public async Task<IActionResult> Index()
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Cliente" && rol != "Admin")
                return RedirectToAction("AccesoDenegado", "Home");

            // Para Cliente, mostrar solo sus reservas:
            if (rol == "Cliente")
            {
                int idPersona = HttpContext.Session.GetInt32("IdPersona") ?? 0;
                var reservas = _context.Reservas
                    .Include(r => r.IdHabitacionNavigation)
                    .Where(r => r.IdPersona == idPersona);
                return View(await reservas.ToListAsync());
            }

            // Para Admin mostrar todo
            return View(await _context.Reservas.Include(r => r.IdHabitacionNavigation).ToListAsync());
        }


        // GET: Reservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var reserva = await _context.Reservas
                .Include(r => r.IdHabitacionNavigation)
                .Include(r => r.IdPersonaNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reserva == null)
                return NotFound();

            return View(reserva);
        }

        // GET: Reservas/Create
        public IActionResult Create()
        {
            ViewData["IdHabitacion"] = new SelectList(_context.Habitaciones.Where(h => h.Estado == "Disponible"), "Id", "Numero");
            ViewData["IdPersona"] = new SelectList(_context.Personas, "Id", "Nombre");
            return View();
        }

        // POST: Reservas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdPersona,IdHabitacion,FechaEntrada,FechaSalida")] Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(reserva);

                    // Cambiar estado habitación a ocupada
                    var habitacion = await _context.Habitaciones.FindAsync(reserva.IdHabitacion);
                    if (habitacion != null)
                    {
                        habitacion.Estado = "Ocupada";
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear la reserva: " + ex.Message);
                }
            }

            ViewData["IdHabitacion"] = new SelectList(_context.Habitaciones.Where(h => h.Estado == "Disponible"), "Id", "Numero", reserva.IdHabitacion);
            ViewData["IdPersona"] = new SelectList(_context.Personas, "Id", "Nombre", reserva.IdPersona);
            return View(reserva);
        }

        // GET: Reservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
                return NotFound();

            ViewData["IdHabitacion"] = new SelectList(_context.Habitaciones, "Id", "Numero", reserva.IdHabitacion);
            ViewData["IdPersona"] = new SelectList(_context.Personas, "Id", "Nombre", reserva.IdPersona);
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdPersona,IdHabitacion,FechaEntrada,FechaSalida")] Reserva reserva)
        {
            if (id != reserva.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener reserva original para comparar habitación
                    var reservaOriginal = await _context.Reservas.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
                    if (reservaOriginal == null)
                        return NotFound();

                    _context.Update(reserva);

                    // Si cambió la habitación, actualizar estados
                    if (reservaOriginal.IdHabitacion != reserva.IdHabitacion)
                    {
                        // Liberar habitación anterior
                        var habitacionAntigua = await _context.Habitaciones.FindAsync(reservaOriginal.IdHabitacion);
                        if (habitacionAntigua != null)
                        {
                            habitacionAntigua.Estado = "Disponible";
                        }

                        // Ocupar habitación nueva
                        var habitacionNueva = await _context.Habitaciones.FindAsync(reserva.IdHabitacion);
                        if (habitacionNueva != null)
                        {
                            habitacionNueva.Estado = "Ocupada";
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdHabitacion"] = new SelectList(_context.Habitaciones, "Id", "Numero", reserva.IdHabitacion);
            ViewData["IdPersona"] = new SelectList(_context.Personas, "Id", "Nombre", reserva.IdPersona);
            return View(reserva);
        }

        // GET: Reservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var reserva = await _context.Reservas
                .Include(r => r.IdHabitacionNavigation)
                .Include(r => r.IdPersonaNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reserva == null)
                return NotFound();

            return View(reserva);
        }

        // POST: Habitaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var habitacion = await _context.Habitaciones
                .Include(h => h.Reservas)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (habitacion != null)
            {
                // Verificar si la habitación tiene alguna reserva activa o futura
                var tieneReservasActivas = habitacion.Reservas.Any(r =>
                    r.FechaSalida == null || r.FechaSalida >= DateOnly.FromDateTime(DateTime.Now));

                if (tieneReservasActivas)
                {
                    TempData["MensajeError"] = "No se puede eliminar la habitación porque tiene reservas activas o futuras.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Habitaciones.Remove(habitacion);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.Id == id);
        }
    }
}
