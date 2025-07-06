using System;
using System.Collections.Generic;
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
            var hotelCaliforniaDbContext = _context.Reservas.Include(r => r.IdHabitacionNavigation).Include(r => r.IdPersonaNavigation);
            return View(await hotelCaliforniaDbContext.ToListAsync());
        }

        // GET: Reservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.IdHabitacionNavigation)
                .Include(r => r.IdPersonaNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // GET: Reservas/Create
        public IActionResult Create()
        {
            ViewData["IdHabitacion"] = new SelectList(_context.Habitaciones, "Id", "Numero"); // Mostrar número en lugar de ID
            ViewData["IdPersona"] = new SelectList(_context.Personas, "Id", "Nombre"); // Mostrar nombre en lugar de ID
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
                    // Asignar las propiedades de navegación como null (opcional)
                    reserva.IdPersonaNavigation = null;
                    reserva.IdHabitacionNavigation = null;

                    _context.Add(reserva);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear la reserva: " + ex.Message);
                }
            }

            // Recargar los SelectList si hay error
            ViewData["IdHabitacion"] = new SelectList(_context.Habitaciones, "Id", "Numero", reserva.IdHabitacion);
            ViewData["IdPersona"] = new SelectList(_context.Personas, "Id", "Nombre", reserva.IdPersona);
            return View(reserva);
        }

        // GET: Reservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }
            ViewData["IdHabitacion"] = new SelectList(_context.Habitaciones, "Id", "Id", reserva.IdHabitacion);
            ViewData["IdPersona"] = new SelectList(_context.Personas, "Id", "Id", reserva.IdPersona);
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdPersona,IdHabitacion,FechaEntrada,FechaSalida")] Reserva reserva)
        {
            if (id != reserva.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reserva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdHabitacion"] = new SelectList(_context.Habitaciones, "Id", "Id", reserva.IdHabitacion);
            ViewData["IdPersona"] = new SelectList(_context.Personas, "Id", "Id", reserva.IdPersona);
            return View(reserva);
        }

        // GET: Reservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.IdHabitacionNavigation)
                .Include(r => r.IdPersonaNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva != null)
            {
                _context.Reservas.Remove(reserva);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.Id == id);
        }
    }
}
