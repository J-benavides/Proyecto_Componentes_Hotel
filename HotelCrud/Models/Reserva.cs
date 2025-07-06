using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace HotelCrud.Models;

public partial class Reserva
{
    public int Id { get; set; }

    public int IdPersona { get; set; }

    public int IdHabitacion { get; set; }

    public DateOnly? FechaEntrada { get; set; }

    public DateOnly? FechaSalida { get; set; }

    [ValidateNever] // Agrega esta anotación
    public virtual Habitacione IdHabitacionNavigation { get; set; } = null!;

    [ValidateNever] // Agrega esta anotación
    public virtual Persona IdPersonaNavigation { get; set; } = null!;
}