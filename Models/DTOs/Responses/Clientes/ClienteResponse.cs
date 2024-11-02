namespace gaco_api.Models.DTOs.Responses.Clientes
{
    public class ClienteResponse
    {
        public long Id { get; set; }

        public string Telefono { get; set; } = null!;

        public string Rfc { get; set; } = null!;

        public string Direccion { get; set; } = null!;

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public int IdCatEstatus { get; set; }
    }
}
