using System.Collections.Generic;

namespace HotelCrud.Models
{
    public class DashboardViewModel
    {
        public int TotalReservas { get; set; }
        public int TotalHabitaciones { get; set; }
        public int TotalPersonas { get; set; }
        public int HabitacionesDisponibles { get; set; }

        public List<string> Meses { get; set; } = new();
        public List<int> ReservasPorMes { get; set; } = new();

        public List<Reserva> UltimasReservas { get; set; } = new();
    }
}
