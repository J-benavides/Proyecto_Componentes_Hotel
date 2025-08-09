using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelCrud.Models
{
    public partial class Pagos
    {
        public int Id { get; set; }

        [ForeignKey("IdReservacionNavigation")]
        [Required(ErrorMessage = "Debe seleccionar una reservación.")]
        public int IdReservacion { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? MontoTotal { get; set; } 

        [DataType(DataType.Date)]
        public DateOnly? FechaPago { get; set; } 

        [Required(ErrorMessage = "Debe seleccionar un método de pago.")]
        [StringLength(50)]
        public string MetodoPago { get; set; } = null!;

        [StringLength(20)]
        public string? NumeroTarjeta { get; set; } 

        [StringLength(4)]
        public string? CVV { get; set; }

        [NotMapped]
        [Display(Name = "Nombre en la tarjeta")]
        public string? NombreTarjeta { get; set; }

        [NotMapped]
        [Display(Name = "Fecha de expiración")]
        public string? FechaExpiracion { get; set; }

        [ValidateNever]
        public virtual Reserva IdReservacionNavigation { get; set; } = null!;
    }
}
