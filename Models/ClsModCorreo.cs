namespace gaco_api.Models
{
    public class ClsModCorreo
    {
        public string? strBody { get; set; }
        public string? strTo { get; set; }
        public List<string>? ListstrTo { get; set; }
        public string? strFromNombre { get; set; }
        public string? strFrom { get; set; }
        public string? strCC { get; set; }
        public string? strSubject { get; set; }
        public string? strPassword { get; set; }
        public int? intPuerto { get; set; }
        public string? strHost { get; set; }
        public bool? usaSSL { get; set; }
        public byte[]? attach { get; set; }
        public string? FileName { get; set; }
        public string archivo { get; set; }
        public string? correo { get; set; }
        public string? folio { get; set; }
        public string? nombre { get; set; }
        public int? IdEncRegistroSuscripcion { get; set; }
        public string? latitud { get; set; }
        public string? longitud { get; set; }
        public string? TipoDocumento { get; set; }
        public bool isGaleria { get; set; }
        public string? Gestor { get; set; }
        public List<ClsModAttachment>? attachments { get; set; }

        public class ClsModAttachment
        {
            public string FileName { get; set; } // Nombre del archivo
            public string ContentType { get; set; } // Tipo de contenido del archivo
            public byte[] FileContent { get; set; } // Contenido del archivo como bytes
        }
    }
}
