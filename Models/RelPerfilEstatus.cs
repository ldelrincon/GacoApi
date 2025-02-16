using System;
using System.Collections.Generic;

namespace gaco_api.Models;

public partial class RelPerfilEstatus
{
    public int IdRelPerfilEstatus { get; set; }

    public int? IdPerfil { get; set; }

    public int? IdEstatus { get; set; }

    public virtual CatEstatus? IdEstatusNavigation { get; set; }
}
