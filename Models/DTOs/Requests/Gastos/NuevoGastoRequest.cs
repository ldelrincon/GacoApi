namespace gaco_api.Models.DTOs.Requests.Gastos
{
    public class NuevoGastoRequest
    {
        public long IdUsuarioCreacion { get; set; }

        public bool Factura { get; set; }

        public string? RutaPdffactura { get; set; }

        public string? RutaXmlfactura { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime FechaModificacion { get; set; }

        public int IdCatEstatus { get; set; }

        public virtual ICollection<DetalleGasto> DetalleGastos { get; set; } = new List<DetalleGasto>();
    }
}
