using System;
using System.Collections.Generic;

namespace gaco_api.Models;

public partial class ReporteServicio
{
    public long Id { get; set; }

    public int IdCatSolicitud { get; set; }

    public long IdUsuarioCreacion { get; set; }

    public long IdCliente { get; set; }

    public string Titulo { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int IdCatEstatus { get; set; }

    public virtual CatEstatus IdCatEstatusNavigation { get; set; } = null!;

    public virtual CatTipoSolicitude IdCatSolicitudNavigation { get; set; } = null!;

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioCreacionNavigation { get; set; } = null!;

    public virtual ICollection<Seguimento> Seguimentos { get; set; } = new List<Seguimento>();
}
