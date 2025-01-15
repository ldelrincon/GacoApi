namespace gaco_api.Models.DTOs.Responses.Relaciones
{
    public class RelSeguimentoProductoResponse
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

        public string? Codigo { get; set; }
        public string? Producto { get; set; }
        public decimal? Porcentaje { get; set; }
    }
}
