using System;
using System.Collections.Generic;

namespace gaco_api.Models;

public partial class LogUsuario
{
    public int Id { get; set; }

    public long IdUsuario { get; set; }

    public string Pantalla { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public string Accion { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
