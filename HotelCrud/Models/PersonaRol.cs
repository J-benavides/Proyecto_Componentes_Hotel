using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelCrud.Models;

public partial class PersonaRol
{
    public int Id { get; set; }

    public int IdPersona { get; set; }

    public int IdRol { get; set; }

    [ValidateNever]
    public virtual Persona IdPersonaNavigation { get; set; } = null!;

    [ValidateNever]
    public virtual Role IdRolNavigation { get; set; } = null!;
}

