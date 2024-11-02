using System;
using System.Collections.Generic;

namespace gaco_api.Models;

public partial class Evidencia
{
    public long Id { get; set; }

    public long IdSeguimento { get; set; }

    public string Nombre { get; set; } = null!;

    public string Extension { get; set; } = null!;

    public string Ruta { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int IdCatEstatus { get; set; }

    public virtual CatEstatus IdCatEstatusNavigation { get; set; } = null!;

    public virtual Seguimento IdSeguimentoNavigation { get; set; } = null!;
}
