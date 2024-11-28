namespace gaco_api.Models.DTOs.Responses.Clientes
{
    public class ClienteCatalogoResponse
    {
        public long Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
    }
}
