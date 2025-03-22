namespace gaco_api.Models.DTOs.Requests.Gastos
{
    public class DetGastoRequest
    {
        public long IdGasto { get; set; }

        public DateTime Fecha { get; set; }

        public string Descripcion { get; set; } = null!;

        public decimal Monto { get; set; }

        public bool Factura { get; set; }

        public string? RutaPdffactura { get; set; }

        public string? RutaXmlfactura { get; set; }

        public int IdCatEstatus { get; set; }
    }
}
