using System;
using System.Collections.Generic;

namespace HotelCrud.Models;

public partial class Role
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<PersonaRol> PersonaRols { get; set; } = new List<PersonaRol>();
}
