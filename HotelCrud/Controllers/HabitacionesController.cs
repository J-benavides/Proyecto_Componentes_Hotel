using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelCrud.Models;

namespace HotelCrud.Controllers
{
    public class HabitacionesController : Controller
    {
        private readonly HotelCaliforniaDbContext _context;

        public HabitacionesController(HotelCaliforniaDbContext context)
        {
            _context = context;
        }

        // GET: AccesoDenegado
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Rol") != "Admin")
                return RedirectToAction("AccesoDenegado", "Home");

            return View(await _context.Habitaciones.ToListAsync());
        }


        // GET: Habitaciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var habitacion = await _context.Habitaciones.FirstOrDefaultAsync(m => m.Id == id);
            if (habitacion == null) return NotFound();

            return View(habitacion);
        }

        // GET: Habitaciones/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Habitaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Numero,Tipo,Precio")] Habitacione habitacion)
        {
            if (ModelState.IsValid)
            {
                habitacion.Estado = "Disponible"; // Estado por defecto
                _context.Add(habitacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(habitacion);
        }

        // GET: Habitaciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var habitacion = await _context.Habitaciones.FindAsync(id);
            if (habitacion == null) return NotFound();

            return View(habitacion);
        }

        // POST: Habitaciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Numero,Tipo,Precio,Estado")] Habitacione habitacion)
        {
            if (id != habitacion.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(habitacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HabitacioneExists(habitacion.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(habitacion);
        }

        // GET: Habitaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var habitacion = await _context.Habitaciones.FirstOrDefaultAsync(m => m.Id == id);
            if (habitacion == null) return NotFound();

            // Verificar si la habitación tiene reservas activas
            bool tieneReservas = await _context.Reservas.AnyAsync(r => r.IdHabitacion == id);
            if (tieneReservas)
            {
                TempData["Error"] = "No se puede eliminar la habitación porque tiene reservas activas.";
                return RedirectToAction(nameof(Index));
            }

            return View(habitacion);
        }

        // POST: Habitaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var habitacion = await _context.Habitaciones.FindAsync(id);
            if (habitacion == null) return NotFound();

            // Validar si la habitación tiene reservas activas antes de eliminar
            bool tieneReservas = await _context.Reservas.AnyAsync(r => r.IdHabitacion == id);
            if (tieneReservas)
            {
                TempData["Error"] = "No se puede eliminar la habitación porque tiene reservas activas.";
                return RedirectToAction(nameof(Index));
            }

            _context.Habitaciones.Remove(habitacion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HabitacioneExists(int id)
        {
            return _context.Habitaciones.Any(e => e.Id == id);
        }
    }
}
