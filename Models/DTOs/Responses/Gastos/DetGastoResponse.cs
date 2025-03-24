namespace gaco_api.Models.DTOs.Responses.Gastos
{
    public class DetGastoResponse
    {
        public long Id { get; set; }

        public long IdGasto { get; set; }

        public DateTime Fecha { get; set; }

        public string Descripcion { get; set; } = null!;

        public decimal Monto { get; set; }

        public bool Factura { get; set; }

        public string? RutaPdffactura { get; set; }

        public string? RutaXmlfactura { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public int IdCatEstatus { get; set; }
    }
}
