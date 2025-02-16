namespace gaco_api.Models.DTOs.Requests.Gastos
{
    public class BusquedaGastoRequest : BasePaginadorRequest
    {
        public string Busqueda { get; set; }
    }
}
