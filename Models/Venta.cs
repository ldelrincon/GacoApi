using System;
using System.Collections.Generic;

namespace gaco_api.Models;

public partial class Venta
{
    public int IdVentas { get; set; }

    public string? Descripcion { get; set; }

    public long? IdCliente { get; set; }

    public DateTime? FechaVenta { get; set; }

    public int? Cantidad { get; set; }

    public decimal? Precio { get; set; }

    public decimal? Subtotal { get; set; }

    public decimal? Iva { get; set; }

    public decimal? Total { get; set; }

    public int? Estatus { get; set; }

    public string? Observaciones { get; set; }

    public long? IdUsuarioCreacion { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual CatEstatus? EstatusNavigation { get; set; }

    public virtual Cliente? IdClienteNavigation { get; set; }

    public virtual Usuario? IdUsuarioCreacionNavigation { get; set; }
}
