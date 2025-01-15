using System;
using System.Collections.Generic;

namespace gaco_api.Models;

public partial class RelSeguimentoProducto
{
    public long Id { get; set; }

    public long IdSeguimento { get; set; }

    public long IdProducto { get; set; }

    public long IdUsuario { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public int IdCatEstatus { get; set; }

    public decimal Cantidad { get; set; }

    public string Unidad { get; set; } = null!;

    public decimal? MontoGasto { get; set; }

    public decimal? MontoVenta { get; set; }

    public decimal? Porcentaje { get; set; }

    public virtual CatEstatus IdCatEstatusNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Seguimento IdSeguimentoNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
