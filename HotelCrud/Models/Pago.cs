using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelCrud.Models
{
    public  class Pago
    {
        public int Id { get; set; }

        public int IdReservacion { get; set; }

        public decimal MontoTotal { get; set; }

        public DateOnly? FechaPago { get; set; }

        public string MetodoPago { get; set; }

        [ForeignKey("IdReservacion")]
        public virtual Reserva Reservacion { get; set; }
    }
}
