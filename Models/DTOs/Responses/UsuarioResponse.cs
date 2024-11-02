namespace gaco_api.Models.DTOs.Responses
{
    public class UsuarioResponse
    {
        public long Id { get; set; }

        public int TipoUsuario { get; set; }

        public string Usuario { get; set; } = null!;

        public string Correo { get; set; } = null!;

        public bool CorreoConfirmado { get; set; }

        public string NombreCompleto { get; set; } = null!;

        public bool Tfa { get; set; }

        public DateTime FechaCreacion { get; set; }

        public int Estatus { get; set; }
    }
}
