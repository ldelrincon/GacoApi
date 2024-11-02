using System;
using System.Collections.Generic;

namespace gaco_api.Models;

public partial class CatTipoUsuario
{
    public int Id { get; set; }

    public string TipoUsuario { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int IdCatEstatus { get; set; }

    public virtual CatEstatus IdCatEstatusNavigation { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
