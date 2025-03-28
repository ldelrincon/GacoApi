﻿using gaco_api.Models.DTOs.Responses.Evidencias;
using gaco_api.Models.DTOs.Responses.Relaciones;

namespace gaco_api.Models.DTOs.Responses.ReporteSolicitudes
{
    public class EditarReporteServicioResponse
    {
        public long Id { get; set; }
        public int IdCatSolicitud { get; set; }
        public long IdUsuarioCreacion { get; set; }
        public long IdCliente { get; set; }
        public string Titulo { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int IdCatEstatus { get; set; }
        public DateTime? FechaInicio { get; set; }
        public string Accesorios { get; set; } = null!;
        public bool ServicioPreventivo { get; set; }
        public bool ServicioCorrectivo { get; set; }
        public string ObservacionesRecomendaciones { get; set; } = null!;
        //public long IdUsuarioTecnico { get; set; }
        public string UsuarioTecnico { get; set; }
        public string UsuarioEncargado { get; set; } = null!;

        public List<RelSeguimentoProductoResponse>? Productos { get; set; }
        public List<EvidenciaResponse>? Evidencias { get; set; }
        public DateTime? ProximaVisita {  get; set; }
        public string? DescripcionProximaVisita { get; set; }

        // Mano de Obra.
        public decimal MontoGasto { get; set; }
        public decimal Porcentaje { get; set; }
        public decimal MontoVenta { get; set; }
    }
}
