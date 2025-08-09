using HotelCrud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

            var query = _context.Pagos
                .Include(p => p.IdReservacionNavigation)
                    .ThenInclude(r => r.IdHabitacionNavigation)
                .Include(p => p.IdReservacionNavigation.IdPersonaNavigation)
                .AsQueryable();

            if (rol == "Cliente")
            {
                int idPersona = HttpContext.Session.GetInt32("IdPersona") ?? 0;
                var pagosCliente = await query
                    .Where(p => p.IdReservacionNavigation.IdPersona == idPersona)
                    .ToListAsync();

                return View(pagosCliente);
            }

            var pagosAdmin = await query.ToListAsync();
            ViewBag.TotalIngresos = pagosAdmin.Sum(p => p.MontoTotal ?? 0m);
            return View(pagosAdmin);
        }

        // GET: Pagos/Create or Pagos/Create?idReservacion=5
        public async Task<IActionResult> Create(int? idReservacion)
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Cliente")
                return RedirectToAction("AccesoDenegado", "Home");

            int idPersona = HttpContext.Session.GetInt32("IdPersona") ?? 0;

            if (idReservacion.HasValue)
            {
                var reserva = await _context.Reservas
                    .Include(r => r.IdHabitacionNavigation)
                    .FirstOrDefaultAsync(r => r.Id == idReservacion.Value && r.IdPersona == idPersona);

                if (reserva == null)
                    return NotFound();

                var model = new Pagos
                {
                    IdReservacion = reserva.Id,
                    MontoTotal = reserva.IdHabitacionNavigation.Precio ?? 0m,
                    FechaPago = DateOnly.FromDateTime(DateTime.Now),
                    MetodoPago = "Transferencia"
                };

                ViewData["ReservaDescripcion"] = $"Hab {reserva.IdHabitacionNavigation.Numero} — {reserva.FechaEntrada?.ToString("yyyy-MM-dd")} a {reserva.FechaSalida?.ToString("yyyy-MM-dd")}";
                ViewData["MontoReserva"] = model.MontoTotal?.ToString("N2") ?? "0.00";

                return View(model);
            }
            else
            {
                var reservas = await _context.Reservas
                    .Include(r => r.IdHabitacionNavigation)
                    .Where(r => r.IdPersona == idPersona)
                    .ToListAsync();

                ViewData["IdReservacion"] = new SelectList(reservas.Select(r => new
                {
                    r.Id,
                    Descripcion = $"#{r.Id} - Hab {r.IdHabitacionNavigation.Numero} ({r.FechaEntrada?.ToString("yyyy-MM-dd")} - {r.FechaSalida?.ToString("yyyy-MM-dd")}) - ₡{(r.IdHabitacionNavigation.Precio?.ToString("N2") ?? "0.00")}"
                }), "Id", "Descripcion");

                var model = new Pagos { MetodoPago = "Transferencia", FechaPago = DateOnly.FromDateTime(DateTime.Now) };
                return View(model);
            }
        }

        // POST: Pagos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdReservacion,MontoTotal,FechaPago,MetodoPago,NumeroTarjeta,CVV,NombreTarjeta,FechaExpiracion")] Pagos pago)
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Cliente")
                return RedirectToAction("AccesoDenegado", "Home");

            int idPersona = HttpContext.Session.GetInt32("IdPersona") ?? 0;

            var reserva = await _context.Reservas
                .Include(r => r.IdHabitacionNavigation)
                .FirstOrDefaultAsync(r => r.Id == pago.IdReservacion);

            if (reserva == null || reserva.IdPersona != idPersona)
            {
                ModelState.AddModelError("", "Reserva inválida o no pertenece al usuario autenticado.");
            }
            else
            {
                pago.MontoTotal = reserva.IdHabitacionNavigation.Precio ?? 0m;
                pago.FechaPago = DateOnly.FromDateTime(DateTime.Now);
                if (string.IsNullOrWhiteSpace(pago.MetodoPago))
                    pago.MetodoPago = "Transferencia";
            }

            if (pago.MetodoPago.Equals("Tarjeta", StringComparison.OrdinalIgnoreCase))
            {
                var numeroSinEspacios = (pago.NumeroTarjeta ?? "").Replace(" ", "");
                if (string.IsNullOrWhiteSpace(numeroSinEspacios) || numeroSinEspacios.Length < 13)
                    ModelState.AddModelError("NumeroTarjeta", "Número de tarjeta inválido.");

                if (string.IsNullOrWhiteSpace(pago.CVV) || (pago.CVV.Length < 3 || pago.CVV.Length > 4))
                    ModelState.AddModelError("CVV", "CVV inválido.");

                if (string.IsNullOrWhiteSpace(pago.NombreTarjeta))
                    ModelState.AddModelError("NombreTarjeta", "Nombre en la tarjeta es requerido.");
            }

            if (!ModelState.IsValid)
            {
                var reservas = await _context.Reservas
                    .Include(r => r.IdHabitacionNavigation)
                    .Where(r => r.IdPersona == idPersona)
                    .ToListAsync();

                ViewData["IdReservacion"] = new SelectList(reservas.Select(r => new
                {
                    r.Id,
                    Descripcion = $"#{r.Id} - Hab {r.IdHabitacionNavigation.Numero} ({r.FechaEntrada?.ToString("yyyy-MM-dd")} - {r.FechaSalida?.ToString("yyyy-MM-dd")}) - ₡{(r.IdHabitacionNavigation.Precio?.ToString("N2") ?? "0.00")}"
                }), "Id", "Descripcion", pago.IdReservacion);

                if (reserva != null)
                {
                    ViewData["ReservaDescripcion"] = $"Hab {reserva.IdHabitacionNavigation.Numero} — {reserva.FechaEntrada?.ToString("yyyy-MM-dd")} a {reserva.FechaSalida?.ToString("yyyy-MM-dd")}";
                    ViewData["MontoReserva"] = pago.MontoTotal?.ToString("N2") ?? "0.00";
                }

                return View(pago);
            }

            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Pagos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var pago = await _context.Pagos
                .Include(p => p.IdReservacionNavigation)
                    .ThenInclude(r => r.IdPersonaNavigation)
                .Include(p => p.IdReservacionNavigation)
                    .ThenInclude(r => r.IdHabitacionNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pago == null) return NotFound();

            var rol = HttpContext.Session.GetString("Rol");
            if (rol == "Cliente")
            {
                int idPersona = HttpContext.Session.GetInt32("IdPersona") ?? 0;
                if (pago.IdReservacionNavigation.IdPersona != idPersona)
                    return RedirectToAction("AccesoDenegado", "Home");
            }

            return View(pago);
        }

        // GET: Pagos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var pago = await _context.Pagos
                .Include(p => p.IdReservacionNavigation)
                    .ThenInclude(r => r.IdPersonaNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pago == null) return NotFound();

            var rol = HttpContext.Session.GetString("Rol");
            if (rol == "Cliente")
            {
                int idPersona = HttpContext.Session.GetInt32("IdPersona") ?? 0;
                if (pago.IdReservacionNavigation.IdPersona != idPersona)
                    return RedirectToAction("AccesoDenegado", "Home");
            }

            return View(pago);
        }

        // POST: Pagos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pago = await _context.Pagos.FindAsync(id);
            if (pago != null)
            {
                _context.Pagos.Remove(pago);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private string MaskCardNumber(string card)
        {
            if (string.IsNullOrWhiteSpace(card)) return card;
            var digits = card.Replace(" ", "");
            if (digits.Length <= 4) return digits;
            return new string('*', digits.Length - 4) + digits.Substring(digits.Length - 4);
        }
    }
}
