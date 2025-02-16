namespace gaco_api.Models.DTOs.Requests.Gastos
{
    public class ActualizarGastoRequest
    {
        public long Id { get; set; }
        public long IdUsuarioCreacion { get; set; }

        public bool Factura { get; set; }

        public string? RutaPdffactura { get; set; }

        public string? RutaXmlfactura { get; set; }

        public DateTime FechaModificacion { get; set; }

        public int IdCatEstatus { get; set; }

        public virtual ICollection<DetalleGasto> DetalleGastos { get; set; } = new List<DetalleGasto>();
    }
}
