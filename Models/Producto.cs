using System;
using System.Collections.Generic;

namespace gaco_api.Models;

public partial class Producto
{
    public long Id { get; set; }

    public int IdCatGrupoProducto { get; set; }

    public string Producto1 { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int IdCatEstatus { get; set; }

    public string Codigo { get; set; } = null!;

    public virtual CatEstatus IdCatEstatusNavigation { get; set; } = null!;

    public virtual CatGrupoProducto IdCatGrupoProductoNavigation { get; set; } = null!;

    public virtual ICollection<RelSeguimentoProducto> RelSeguimentoProductos { get; set; } = new List<RelSeguimentoProducto>();
}
