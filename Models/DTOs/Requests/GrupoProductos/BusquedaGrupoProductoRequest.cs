namespace gaco_api.Models.DTOs.Requests.Productos
{
    public class BusquedaGrupoProductoRequest : BasePaginadorRequest
    {
        public string Busqueda { get; set; }
    }
}
