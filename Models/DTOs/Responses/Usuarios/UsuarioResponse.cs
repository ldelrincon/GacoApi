namespace gaco_api.Models.DTOs.Responses.Usuarios
{
    public class UsuarioResponse
    {
    public long Id { get; set; }

        public int IdCatTipoUsuario { get; set; }

        public string Correo { get; set; } = null!;

        public string Contrasena { get; set; } = null!;

        public bool CorreoConfirmado { get; set; }

        public string Nombres { get; set; } = null!;

        public string Apellidos { get; set; } = null!;

        public string Telefono { get; set; } = null!;

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public int IdCatEstatus { get; set; }

        public string Estatus { get; set; }
        public string TipoUsuario { get; set; }
    }
}
