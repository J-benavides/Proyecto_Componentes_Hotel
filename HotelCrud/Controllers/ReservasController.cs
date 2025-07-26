using HotelCrud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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

        var reservasAdmin = _context.Reservas
            .Include(r => r.IdHabitacionNavigation)
            .Include(r => r.IdPersonaNavigation);

        return View(await reservasAdmin.ToListAsync());
    }

    // GET: Reservas/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var reserva = await _context.Reservas
            .Include(r => r.IdHabitacionNavigation)
            .Include(r => r.IdPersonaNavigation)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (reserva == null) return NotFound();

        return View(reserva);
    }

    // GET: Reservas/Create
    public IActionResult Create()
    {
        var rol = HttpContext.Session.GetString("Rol");
        if (rol != "Cliente" && rol != "Admin")
            return RedirectToAction("AccesoDenegado", "Home");

        if (rol == "Admin")
            ViewData["IdPersona"] = new SelectList(_context.Personas, "Id", "Nombre");

        ViewData["IdHabitacion"] = new SelectList(_context.Habitaciones.Where(h => h.Estado == "Disponible"), "Id", "Numero");
        return View();
    }

    // POST: Reservas/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdPersona,IdHabitacion,FechaEntrada,FechaSalida")] Reserva reserva)
    {
        var rol = HttpContext.Session.GetString("Rol");
        if (rol != "Cliente" && rol != "Admin")
            return RedirectToAction("AccesoDenegado", "Home");

        if (rol == "Cliente")
        {
            int idPersonaSesion = HttpContext.Session.GetInt32("IdPersona") ?? 0;
            if (idPersonaSesion == 0)
            {
                ModelState.AddModelError("", "Usuario no identificado en sesión.");
                ViewData["IdHabitacion"] = new SelectList(_context.Habitaciones.Where(h => h.Estado == "Disponible"), "Id", "Numero", reserva.IdHabitacion);
                return View(reserva);
            }
            reserva.IdPersona = idPersonaSesion;
        }

        if (reserva.FechaEntrada == default || reserva.FechaSalida == default)
        {
            ModelState.AddModelError("", "Debe ingresar fechas válidas de entrada y salida.");
        }
        else if (reserva.FechaSalida <= reserva.FechaEntrada)
        {
            ModelState.AddModelError("", "La fecha de salida debe ser mayor que la fecha de entrada.");
        }

        if (!ModelState.IsValid)
        {
            if (rol == "Admin")
                ViewData["IdPersona"] = new SelectList(_context.Personas, "Id", "Nombre", reserva.IdPersona);

            ViewData["IdHabitacion"] = new SelectList(_context.Habitaciones.Where(h => h.Estado == "Disponible"), "Id", "Numero", reserva.IdHabitacion);
            return View(reserva);
        }

        var resultadoParam = new Microsoft.Data.SqlClient.SqlParameter
        {
            ParameterName = "@Resultado",
            SqlDbType = System.Data.SqlDbType.Int,
            Direction = System.Data.ParameterDirection.Output
        };

        try
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC dbo.sp_AddReserva @IdPersona, @IdHabitacion, @FechaEntrada, @FechaSalida, @Resultado OUT",
                new Microsoft.Data.SqlClient.SqlParameter("@IdPersona", reserva.IdPersona),
                new Microsoft.Data.SqlClient.SqlParameter("@IdHabitacion", reserva.IdHabitacion),
                new Microsoft.Data.SqlClient.SqlParameter("@FechaEntrada", reserva.FechaEntrada),
                new Microsoft.Data.SqlClient.SqlParameter("@FechaSalida", reserva.FechaSalida),
                resultadoParam);

            int resultado = (int)resultadoParam.Value;

            if (resultado == 1)
                return RedirectToAction(nameof(Index));
            else
                ModelState.AddModelError("", "La habitación no está disponible para las fechas seleccionadas.");
        }
        catch (System.Exception ex)
        {
            ModelState.AddModelError("", "Error al crear la reserva: " + ex.Message);
        }

        if (rol == "Admin")
            ViewData["IdPersona"] = new SelectList(_context.Personas, "Id", "Nombre", reserva.IdPersona);

        ViewData["IdHabitacion"] = new SelectList(_context.Habitaciones.Where(h => h.Estado == "Disponible"), "Id", "Numero", reserva.IdHabitacion);
        return View(reserva);
    }

    // GET: Reservas/Edit/5
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

        // Mostrar habitaciones disponibles o la habitación actual para permitir conservarla
        var habitacionesQuery = _context.Habitaciones.Where(h => h.Estado == "Disponible" || h.Id == reserva.IdHabitacion);
        ViewData["IdHabitacion"] = new SelectList(habitacionesQuery, "Id", "Numero", reserva.IdHabitacion);

        return View(reserva);
    }

    // POST: Reservas/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,IdPersona,IdHabitacion,FechaEntrada,FechaSalida")] Reserva reserva)
    {
        if (id != reserva.Id)
            return NotFound();

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

        if (reserva.FechaEntrada == default || reserva.FechaSalida == default)
        {
            ModelState.AddModelError("", "Debe ingresar fechas válidas de entrada y salida.");
        }
        else if (reserva.FechaSalida <= reserva.FechaEntrada)
        {
            ModelState.AddModelError("", "La fecha de salida debe ser mayor que la fecha de entrada.");
        }

        if (!ModelState.IsValid)
        {
            if (rol == "Admin")
                ViewData["IdPersona"] = new SelectList(_context.Personas, "Id", "Nombre", reserva.IdPersona);

            var habitacionesQuery = _context.Habitaciones.Where(h => h.Estado == "Disponible" || h.Id == reserva.IdHabitacion);
            ViewData["IdHabitacion"] = new SelectList(habitacionesQuery, "Id", "Numero", reserva.IdHabitacion);

            return View(reserva);
        }

        try
        {
            _context.Update(reserva);
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

    // GET: Reservas/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var reserva = await _context.Reservas
            .Include(r => r.IdHabitacionNavigation)
            .Include(r => r.IdPersonaNavigation)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (reserva == null) return NotFound();

        var rol = HttpContext.Session.GetString("Rol");
        if (rol != "Cliente" && rol != "Admin")
            return RedirectToAction("AccesoDenegado", "Home");

        // Si es cliente, solo puede eliminar su propia reserva
        if (rol == "Cliente")
        {
            int idPersonaSesion = HttpContext.Session.GetInt32("IdPersona") ?? 0;
            if (reserva.IdPersona != idPersonaSesion)
                return RedirectToAction("AccesoDenegado", "Home");
        }

        return View(reserva);
    }

    // POST: Reservas/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var reserva = await _context.Reservas.FindAsync(id);
        if (reserva == null) return NotFound();

        var rol = HttpContext.Session.GetString("Rol");
        if (rol != "Cliente" && rol != "Admin")
            return RedirectToAction("AccesoDenegado", "Home");

        // Si es cliente, solo puede eliminar su propia reserva
        if (rol == "Cliente")
        {
            int idPersonaSesion = HttpContext.Session.GetInt32("IdPersona") ?? 0;
            if (reserva.IdPersona != idPersonaSesion)
                return RedirectToAction("AccesoDenegado", "Home");
        }

        _context.Reservas.Remove(reserva);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ReservaExists(int id)
    {
        return _context.Reservas.Any(e => e.Id == id);
    }
}
