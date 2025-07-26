using HotelCrud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HotelCrud.Controllers
{
    public class HabitacionesController : Controller
    {
        private readonly HotelCaliforniaDbContext _context;

        public HabitacionesController(HotelCaliforniaDbContext context)
        {
            _context = context;
        }

        // GET: Habitaciones
        public async Task<IActionResult> Index()
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Admin" && rol != "Cliente")
                return RedirectToAction("AccesoDenegado", "Home");

            ViewData["RolUsuario"] = rol; 

            return View(await _context.Habitaciones.ToListAsync());
        }


        // GET: Habitaciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Admin" && rol != "Cliente")
                return RedirectToAction("AccesoDenegado", "Home");

            if (id == null)
                return NotFound();

            var habitacion = await _context.Habitaciones.FirstOrDefaultAsync(m => m.Id == id);
            if (habitacion == null)
                return NotFound();

            return View(habitacion);
        }

        // GET: Habitaciones/Create
        public IActionResult Create()
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Admin")
                return RedirectToAction("AccesoDenegado", "Home");

            return View();
        }

        // POST: Habitaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Numero,Tipo,Precio")] Habitacione habitacion)
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Admin")
                return RedirectToAction("AccesoDenegado", "Home");

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

            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null) return NotFound();

            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Cliente" && rol != "Admin")
                return RedirectToAction("AccesoDenegado", "Home");

            // Si el usuario es cliente, solo puede editar su propia reserva
            if (rol == "Cliente")
            {
                int idPersonaSesion = HttpContext.Session.GetInt32("IdPersona") ?? 0;
                if (reserva.IdPersona != idPersonaSesion)
                    return RedirectToAction("AccesoDenegado", "Home");
            }

            if (rol == "Admin")
                ViewData["IdPersona"] = new SelectList(_context.Personas, "Id", "Nombre", reserva.IdPersona);

            IQueryable<Habitacione> habitacionesQuery;
            if (rol == "Admin")
            {
                habitacionesQuery = _context.Habitaciones; // Todas
            }
            else
            {
                habitacionesQuery = _context.Habitaciones.Where(h => h.Estado == "Disponible" || h.Id == reserva.IdHabitacion);
            }
            ViewData["IdHabitacion"] = new SelectList(habitacionesQuery, "Id", "Numero", reserva.IdHabitacion);

            return View(reserva);
        }


        // POST: Habitaciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Numero,Tipo,Precio,Estado")] Habitacione habitacion)
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Admin")
                return RedirectToAction("AccesoDenegado", "Home");

            if (id != habitacion.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(habitacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HabitacioneExists(habitacion.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(habitacion);
        }

        // GET: Habitaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Admin")
                return RedirectToAction("AccesoDenegado", "Home");

            if (id == null)
                return NotFound();

            var habitacion = await _context.Habitaciones.FirstOrDefaultAsync(m => m.Id == id);
            if (habitacion == null)
                return NotFound();

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
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Admin")
                return RedirectToAction("AccesoDenegado", "Home");

            var habitacion = await _context.Habitaciones.FindAsync(id);
            if (habitacion == null)
                return NotFound();

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
