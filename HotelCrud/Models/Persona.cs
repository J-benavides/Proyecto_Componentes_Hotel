using System;
using System.Collections.Generic;

namespace HotelCrud.Models;

public partial class Persona
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string? Correo { get; set; }

    public string? Telefono { get; set; }

    public string? Contrasena { get; set; }

    public virtual ICollection<PersonaRol> PersonaRols { get; set; } = new List<PersonaRol>();

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
