using System.Diagnostics;
using HotelCrud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelCrud.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HotelCaliforniaDbContext _context;

        public HomeController(ILogger<HomeController> logger, HotelCaliforniaDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var rol = HttpContext.Session.GetString("Rol") ?? "Invitado";

            if (rol == "Admin")
            {
                var totalReservas = _context.Reservas.Count();
                var totalHabitaciones = _context.Habitaciones.Count();
                var totalPersonas = _context.Personas.Count();
                var habitacionesDisponibles = _context.Habitaciones.Count(h => h.Estado == "Disponible");

                var reservasList = _context.Reservas
                    .Where(r => r.FechaEntrada.HasValue)
                    .AsEnumerable() // Para usar DateOnly.ToDateTime
                    .GroupBy(r => r.FechaEntrada.Value.Month)
                    .OrderBy(g => g.Key)
                    .Select(g => new
                    {
                        Mes = g.Key,
                        Cantidad = g.Count()
                    })
                    .ToList();

                var meses = reservasList.Select(g =>
                    System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Mes)
                ).ToList();

                var reservasPorMes = reservasList.Select(g => g.Cantidad).ToList();

                var ultimasReservas = _context.Reservas
                    .Include(r => r.IdHabitacionNavigation)
                    .Include(r => r.IdPersonaNavigation)
                    .AsEnumerable()
                    .OrderByDescending(r => r.FechaEntrada.HasValue
                        ? r.FechaEntrada.Value.ToDateTime(TimeOnly.MinValue)
                        : DateTime.MinValue)
                    .Take(5)
                    .ToList();

                var modelo = new DashboardViewModel
                {
                    TotalReservas = totalReservas,
                    TotalHabitaciones = totalHabitaciones,
                    TotalPersonas = totalPersonas,
                    HabitacionesDisponibles = habitacionesDisponibles,
                    Meses = meses,
                    ReservasPorMes = reservasPorMes,
                    UltimasReservas = ultimasReservas
                };

                ViewData["Rol"] = rol;
                return View(modelo);
            }
            else if (rol == "Cliente")
            {
                int idPersona = HttpContext.Session.GetInt32("IdPersona") ?? 0;

                if (idPersona <= 0)
                {
                    ViewData["Rol"] = rol;
                    return View(null);
                }

                var reservasCliente = _context.Reservas
                    .Include(r => r.IdHabitacionNavigation)
                    .Where(r => r.IdPersona == idPersona && r.FechaSalida.HasValue)
                    .AsEnumerable()
                    .Where(r => r.FechaSalida.Value.ToDateTime(TimeOnly.MinValue) >= DateTime.Today)
                    .OrderBy(r => r.FechaEntrada.HasValue
                        ? r.FechaEntrada.Value.ToDateTime(TimeOnly.MinValue)
                        : DateTime.MaxValue)
                    .Take(5)
                    .ToList();

                var totalReservasCliente = reservasCliente.Count;

                var modeloCliente = new DashboardViewModel
                {
                    TotalReservas = totalReservasCliente,
                    UltimasReservas = reservasCliente
                };

                ViewData["Rol"] = rol;
                return View(modeloCliente);
            }
            else // Invitado
            {
                ViewData["Rol"] = rol;
                return View(null);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult AccesoDenegado()
        {
            return View();
        }
        public IActionResult Informacion()
        {
            return View();
        }
    }
}
