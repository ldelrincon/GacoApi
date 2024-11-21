using System;
using System.Collections.Generic;

namespace gaco_api.Models;

public partial class CatRegimenFiscale
{
    public int Id { get; set; }

    public string Clave { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int IdCatEstatus { get; set; }

    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

    public virtual CatEstatus IdCatEstatusNavigation { get; set; } = null!;
}
