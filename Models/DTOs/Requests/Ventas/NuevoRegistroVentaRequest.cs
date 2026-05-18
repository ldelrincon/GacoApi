namespace gaco_api.Models.DTOs.Requests.Ventas
{
    public class NuevoRegistroVentaRequest
    {
        public string? Descripcion { get; set; }
        public long? IdCliente { get; set; }
        public int? IdVentas { get; set; } // para casos de actualización, si se envía este campo se actualizará el registro existente
        public DateTime? FechaVenta { get; set; }
        public decimal? Cantidad { get; set; }
        public decimal? Precio { get; set; }
        public int? Estatus { get; set; }
        public string? Observaciones { get; set; }
        public long IdUsuarioCreacion { get; set; } // rellenado con usuario logueado
    }
}