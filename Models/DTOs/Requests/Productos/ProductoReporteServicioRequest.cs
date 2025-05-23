﻿using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace gaco_api.Models.DTOs.Requests.Productos
{
    public class ProductoReporteServicioRequest
    {
        public int Id { get; set; }
        public decimal Cantidad { get; set; }
        public decimal MontoGasto { get; set; }
        public decimal? Porcentaje { get; set; }
        public decimal? MontoVenta { get; set; }
    }
}