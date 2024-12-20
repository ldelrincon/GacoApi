namespace gaco_api.Models.DTOs.Requests.Productos
{
    public class ActualizarGrupoProductoRequest
    {
        public long Id { get; set; }
        public string Grupo { get; set; } = null!;

        public string Descripcion { get; set; } = null!;
    }
}
