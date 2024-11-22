using System.ComponentModel.DataAnnotations;

namespace gaco_api.Models.DTOs.Requests.Usuarios
{
    public class UsuarioRequest
    {
        [Required(ErrorMessage = "El tipo de usuario es obligatorio.")]
        public required int TipoUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres.")]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo es inválido.")]
        public required string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 50 caracteres.")]
        public required string Contrasena { get; set; }
        public required string ConfirmarContrasena { get; set; }
        public required string Apellidos { get; set; }
        public required string Telefono { get; set; }
    }
}
