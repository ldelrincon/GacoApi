using System;
using System.Collections.Generic;

namespace gaco_api.Models;

public partial class Seguimento
{
    public long Id { get; set; }

    public long IdReporteServicio { get; set; }

    public long IdUsuario { get; set; }

    public string Seguimento1 { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int IdCatEstatus { get; set; }

    public virtual ICollection<Evidencia> Evidencia { get; set; } = new List<Evidencia>();

    public virtual CatEstatus IdCatEstatusNavigation { get; set; } = null!;

    public virtual ReporteServicio IdReporteServicioNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<RelSeguimentoProducto> RelSeguimentoProductos { get; set; } = new List<RelSeguimentoProducto>();
}
