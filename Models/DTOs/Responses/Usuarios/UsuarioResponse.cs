namespace gaco_api.Models.DTOs.Responses.Usuarios
{
    public class UsuarioResponse
    {
        public long Id { get; set; }

        public int IdTipoUsuario { get; set; }
        public string TipoUsuario { get; set; }

        public string Usuario { get; set; } = null!;

        public string Correo { get; set; } = null!;

        public bool CorreoConfirmado { get; set; }

        public string NombreCompleto { get; set; } = null!;

        public bool Tfa { get; set; }

        public DateTime FechaCreacion { get; set; }

        public int IdEstatus { get; set; }
        public string Estatus { get; set; }
    }
}
