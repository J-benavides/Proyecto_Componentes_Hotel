using System.ComponentModel.DataAnnotations;

namespace HotelCrud.Models
{
    public class PersonaEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = null!;

        [Required]
        public string Apellidos { get; set; } = null!;

        [EmailAddress]
        public string? Correo { get; set; }

        public string? Telefono { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Actual")]
        public string? ContrasenaActual { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña")]
        public string? NuevaContrasena { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Nueva Contraseña")]
        [Compare("NuevaContrasena", ErrorMessage = "La nueva contraseña y la confirmación no coinciden.")]
        public string? ConfirmarNuevaContrasena { get; set; }
    }
}
