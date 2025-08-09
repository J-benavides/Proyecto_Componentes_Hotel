using HotelCrud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HotelCrud.Controllers
{
    public class PagosController : Controller
    {
        private readonly HotelCaliforniaDbContext _context;

        public PagosController(HotelCaliforniaDbContext context)
        {
            _context = context;
        }

        // GET: Pagos
        public async Task<IActionResult> Index()
        {
            var rol = HttpContext.Session.GetString("Rol");

            if (rol != "Cliente" && rol != "Admin")
                return RedirectToAction("AccesoDenegado", "Home");

            // Si es un cliente, filtra los pagos por su ID de persona a través de la reserva.
            if (rol == "Cliente")
            {
                int idPersona = HttpContext.Session.GetInt32("IdPersona") ?? 0;
                var pagosCliente = _context.Pagos
                    .Include(p => p.IdReservacionNavigation)
                    .ThenInclude(r => r.IdPersonaNavigation)
                    .Where(p => p.IdReservacionNavigation.IdPersona == idPersona);

                return View(await pagosCliente.ToListAsync());
            }

            // Si es un administrador, muestra todos los pagos.
            var pagosAdmin = _context.Pagos
                .Include(p => p.IdReservacionNavigation)
                .ThenInclude(r => r.IdPersonaNavigation);

            return View(await pagosAdmin.ToListAsync());
        }

        // GET: Pagos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Admin" && rol != "Cliente")
                return RedirectToAction("AccesoDenegado", "Home");

            if (id == null) return NotFound();

            var pago = await _context.Pagos
                .Include(p => p.IdReservacionNavigation)
                .ThenInclude(r => r.IdPersonaNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pago == null) return NotFound();

            if (rol == "Cliente")
            {
                int idPersonaSesion = HttpContext.Session.GetInt32("IdPersona") ?? 0;
                if (pago.IdReservacionNavigation?.IdPersona != idPersonaSesion)
                    return RedirectToAction("AccesoDenegado", "Home");
            }

            return View(pago);
        }

        // GET: Pagos/Create
        public IActionResult Create()
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Cliente" && rol != "Admin")
                return RedirectToAction("AccesoDenegado", "Home");

            ViewData["IdReservacion"] = new SelectList(_context.Reservas, "Id", "Id");
            return View();
        }

        // POST: Pagos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdReservacion, MontoTotal, FechaPago, MetodoPago")] Pagos pago)
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Admin" && rol != "Cliente")
                return RedirectToAction("AccesoDenegado", "Home");

            if (ModelState.IsValid)
            {
                _context.Add(pago);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdReservacion"] = new SelectList(_context.Reservas, "Id", "Id", pago.IdReservacion);
            return View(pago);
        }

        // GET: Pagos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Admin")
                return RedirectToAction("AccesoDenegado", "Home");

            if (id == null) return NotFound();

            var pago = await _context.Pagos.FindAsync(id);
            if (pago == null) return NotFound();

            ViewData["IdReservacion"] = new SelectList(_context.Reservas, "Id", "Id", pago.IdReservacion);
            return View(pago);
        }

        // POST: Pagos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, IdReservacion, MontoTotal, FechaPago, MetodoPago")] Pagos pago)
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Admin")
                return RedirectToAction("AccesoDenegado", "Home");

            if (id != pago.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pago);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PagoExists(pago.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdReservacion"] = new SelectList(_context.Reservas, "Id", "Id", pago.IdReservacion);
            return View(pago);
        }

        // GET: Pagos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Admin")
                return RedirectToAction("AccesoDenegado", "Home");

            if (id == null) return NotFound();

            var pago = await _context.Pagos
                .Include(p => p.IdReservacionNavigation)
                .ThenInclude(r => r.IdPersonaNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pago == null) return NotFound();

            return View(pago);
        }

        // POST: Pagos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Admin")
                return RedirectToAction("AccesoDenegado", "Home");

            var pago = await _context.Pagos.FindAsync(id);
            if (pago != null)
            {
                _context.Pagos.Remove(pago);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PagoExists(int id)
        {
            return _context.Pagos.Any(e => e.Id == id);
        }
    }
}