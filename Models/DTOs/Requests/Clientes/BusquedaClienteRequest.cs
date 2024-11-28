namespace gaco_api.Models.DTOs.Requests.Clientes
{
    public class BusquedaClienteRequest : BasePaginadorRequest
    {
        public string Busqueda { get; set; }
    }
}
