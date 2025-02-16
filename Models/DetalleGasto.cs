using System;
using System.Collections.Generic;

namespace gaco_api.Models;

public partial class DetalleGasto
{
    public long Id { get; set; }

    public long IdGasto { get; set; }

    public long IdProducto { get; set; }

    public decimal Cantidad { get; set; }

    public decimal Monto { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public int IdCatEstatus { get; set; }

    public virtual CatEstatus IdCatEstatusNavigation { get; set; } = null!;

    public virtual Gasto IdGastoNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
