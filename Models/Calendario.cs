using System;
using System.Collections.Generic;

namespace gaco_api.Models;

public partial class Calendario
{
    public int IdCalendario { get; set; }

    public string? Descripcion { get; set; }

    public DateTime? FechaTarea { get; set; }

    public long? IdUsuarioTarea { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public long IdUsuarioCreacion { get; set; }

    public bool? Terminado { get; set; }

    public DateTime? FechaTerminado { get; set; }

    public virtual Usuario IdUsuarioCreacionNavigation { get; set; } = null!;

    public virtual Usuario? IdUsuarioTareaNavigation { get; set; }
}
