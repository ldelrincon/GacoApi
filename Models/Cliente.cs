using System;
using System.Collections.Generic;

namespace gaco_api.Models;

public partial class Cliente
{
    public long Id { get; set; }

    public string Telefono { get; set; } = null!;

    public string Rfc { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int IdCatEstatus { get; set; }

    public virtual CatEstatus IdCatEstatusNavigation { get; set; } = null!;

    public virtual ICollection<ReporteServicio> ReporteServicios { get; set; } = new List<ReporteServicio>();
}
