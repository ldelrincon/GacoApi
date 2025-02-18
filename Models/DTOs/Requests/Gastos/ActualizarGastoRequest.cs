namespace gaco_api.Models.DTOs.Requests.Gastos
{
    public class ActualizarGastoRequest
    {
        public long Id { get; set; }

        public string Concepto { get; set; } = null!;

        public DateTime Fecha { get; set; }

        public string Descripcion { get; set; } = null!;

        public decimal Monto { get; set; }

        public bool Factura { get; set; }

        public string? RutaPdffactura { get; set; }

        public string? RutaXmlfactura { get; set; }
    }
}
