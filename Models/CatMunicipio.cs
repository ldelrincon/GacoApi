using System;
using System.Collections.Generic;

namespace gaco_api.Models;

public partial class CatMunicipio
{
    public long Id { get; set; }

    public string? CatalogKey { get; set; }

    public string? Municipio { get; set; }

    public string? EfeKey { get; set; }

    public string? Estatus { get; set; }

    public int IdCatEstatus { get; set; }

    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

    public virtual CatEstatus IdCatEstatusNavigation { get; set; } = null!;
}
