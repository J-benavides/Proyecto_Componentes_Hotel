using System;
using System.Collections.Generic;

namespace HotelCrud.Models;

public partial class Habitacione
{
    public int Id { get; set; }

    public string Numero { get; set; } = null!;

    public string? Tipo { get; set; }

    public decimal? Precio { get; set; }

    public string? Estado { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
