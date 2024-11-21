using System;
using System.Collections.Generic;

namespace gaco_api.Models;

public partial class CatEntidadesFederativa
{
    public long Id { get; set; }

    public string? CatalogKey { get; set; }

    public string? EntidadFederativa { get; set; }

    public string? Abreviatura { get; set; }

    public int IdCatEstatus { get; set; }

    public virtual CatEstatus IdCatEstatusNavigation { get; set; } = null!;
}
