namespace gaco_api.Models.DTOs.Responses.Gastos
{
    public class GastoResponse
    {
        public long Id { get; set; }

        public long IdUsuarioCreacion { get; set; }

        public bool Factura { get; set; }

        public string? RutaPdffactura { get; set; }

        public string? RutaXmlfactura { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime FechaModificacion { get; set; }

        public int IdCatEstatus { get; set; }

        public string UsuarioCreacion { get; set; }

        public string Estatus { get; set; }

        public decimal Total { get; set; }
        public int productos { get; set; }
    }
}
