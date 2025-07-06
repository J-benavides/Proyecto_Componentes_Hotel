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
            var totalReservas = _context.Reservas.Count();
            var totalHabitaciones = _context.Habitaciones.Count();
            var totalPersonas = _context.Personas.Count();
            var habitacionesDisponibles = _context.Habitaciones.Count(h => h.Estado == "Disponible");

            var reservasList = _context.Reservas
                .Where(r => r.FechaEntrada.HasValue)
                .AsEnumerable()
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
                .OrderByDescending(r => r.FechaEntrada)
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

            return View(modelo);
        }


        public IActionResult Privacy()
        {
            return View();
        }
    }
}
