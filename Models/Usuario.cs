using System;
using System.Collections.Generic;

namespace gaco_api.Models;

public partial class Usuario
{
    public long Id { get; set; }

    public int IdCatTipoUsuario { get; set; }

    public string Correo { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public bool CorreoConfirmado { get; set; }

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int IdCatEstatus { get; set; }

    public virtual CatEstatus IdCatEstatusNavigation { get; set; } = null!;

    public virtual CatTipoUsuario IdCatTipoUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<LogUsuario> LogUsuarios { get; set; } = new List<LogUsuario>();

    public virtual ICollection<RelSeguimentoProducto> RelSeguimentoProductos { get; set; } = new List<RelSeguimentoProducto>();

    public virtual ICollection<ReporteServicio> ReporteServicioIdUsuarioCreacionNavigations { get; set; } = new List<ReporteServicio>();

    public virtual ICollection<ReporteServicio> ReporteServicioIdUsuarioTecnicoNavigations { get; set; } = new List<ReporteServicio>();

    public virtual ICollection<Seguimento> Seguimentos { get; set; } = new List<Seguimento>();
}
