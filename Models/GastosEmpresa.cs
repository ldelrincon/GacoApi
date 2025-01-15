using System;
using System.Collections.Generic;

namespace gaco_api.Models;

public partial class GastosEmpresa
{
    public int Id { get; set; }

    public int IdUsuarioCreacion { get; set; }

    public string? Descripcion { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int? IdUsuarioModificacion { get; set; }

    public bool Activo { get; set; }
}
