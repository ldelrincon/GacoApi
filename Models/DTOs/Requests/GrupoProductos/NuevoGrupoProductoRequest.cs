namespace gaco_api.Models.DTOs.Requests.Productos
{
    public class NuevoGrupoProductoRequest
    {
        public string Grupo { get; set; } = null!;

        public string Descripcion { get; set; } = null!;
    }
}
