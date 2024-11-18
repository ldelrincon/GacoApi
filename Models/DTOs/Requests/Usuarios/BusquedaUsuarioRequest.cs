namespace gaco_api.Models.DTOs.Requests.Usuarios
{
    public class BusquedaUsuarioRequest : BasePaginadorRequest
    {
        public string Busqueda { get; set; }
    }
}
