using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelCrud.Models
{
    public class Pagos
    {
        public int Id { get; set; }

        [ForeignKey("IdReservacion")]
        public int IdReservacion { get; set; }

        public decimal MontoTotal { get; set; }

        public DateOnly? FechaPago { get; set; }

        public string MetodoPago { get; set; }

        [ValidateNever]
        public virtual Reserva IdReservacionNavigation { get; set; } = null!;
    }
}
