using System;
using System.Collections.Generic;

namespace gaco_api.Models;

public partial class Gasto
{
    public long Id { get; set; }

    public long IdUsuarioCreacion { get; set; }

    public string Concepto { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public string Descripcion { get; set; } = null!;

    public decimal Monto { get; set; }

    public bool Factura { get; set; }

    public string? RutaPdffactura { get; set; }

    public string? RutaXmlfactura { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int IdCatEstatus { get; set; }

    public virtual CatEstatus IdCatEstatusNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioCreacionNavigation { get; set; } = null!;
}
