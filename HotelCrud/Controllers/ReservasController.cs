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

            if (rol == "Cliente")
            {
                int idPersona = HttpContext.Session.GetInt32("IdPersona") ?? 0;
                var reservasCliente = _context.Reservas
                    .Include(r => r.IdHabitacionNavigation)
                    .Include(r => r.IdPersonaNavigation)
                    .Where(r => r.IdPersona == idPersona);
                return View(await reservasCliente.ToListAsync());
            }

            return View(await _context.Reservas
                .Include(r => r.IdHabitacionNavigation)
                .Include(r => r.IdPersonaNavigation)
                .ToListAsync());
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

                    var habitacion = await _context.Habitaciones.FindAsync(reserva.IdHabitacion);
                    if (habitacion != null)
                    {
                        habitacion.Estado = "Ocupada";
                        _context.Habitaciones.Update(habitacion);
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
                    var reservaOriginal = await _context.Reservas.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
                    if (reservaOriginal == null)
                        return NotFound();

                    _context.Update(reserva);

                    if (reservaOriginal.IdHabitacion != reserva.IdHabitacion)
                    {
                        var habitacionAntigua = await _context.Habitaciones.FindAsync(reservaOriginal.IdHabitacion);
                        if (habitacionAntigua != null)
                            habitacionAntigua.Estado = "Disponible";

                        var habitacionNueva = await _context.Habitaciones.FindAsync(reserva.IdHabitacion);
                        if (habitacionNueva != null)
                            habitacionNueva.Estado = "Ocupada";
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.Id))
                        return NotFound();
                    else
                        throw;
                }
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

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reservas
                .Include(r => r.IdHabitacionNavigation)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva != null)
            {
                var habitacion = reserva.IdHabitacionNavigation;
                if (habitacion != null)
                {
                    habitacion.Estado = "Disponible";
                    _context.Habitaciones.Update(habitacion);
                }

                _context.Reservas.Remove(reserva);
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
