using System;
using System.Collections.Generic;

namespace gaco_api.Models;

public partial class CatEstatus
{
    public int Id { get; set; }

    public string Estatus { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<CatGrupoProducto> CatGrupoProductos { get; set; } = new List<CatGrupoProducto>();

    public virtual ICollection<CatTipoSolicitude> CatTipoSolicitudes { get; set; } = new List<CatTipoSolicitude>();

    public virtual ICollection<CatTipoUsuario> CatTipoUsuarios { get; set; } = new List<CatTipoUsuario>();

    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

    public virtual ICollection<Evidencia> Evidencia { get; set; } = new List<Evidencia>();

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

    public virtual ICollection<RelSeguimentoProducto> RelSeguimentoProductos { get; set; } = new List<RelSeguimentoProducto>();

    public virtual ICollection<ReporteServicio> ReporteServicios { get; set; } = new List<ReporteServicio>();

    public virtual ICollection<Seguimento> Seguimentos { get; set; } = new List<Seguimento>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
