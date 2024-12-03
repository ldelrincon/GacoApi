namespace gaco_api.Models.DTOs.Requests.Productos
{
    public class BusquedaProductoRequest : BasePaginadorRequest
    {
        public string Busqueda { get; set; }
    }
}
