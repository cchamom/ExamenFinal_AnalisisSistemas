using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExamenFinal_Analisis.Models;

namespace ExamenFinal_Analisis.Models
{
    public class Paquete
    {
        [Key]
        public int Id { get; set; }
        
        [Required, StringLength(20)]
        public string CodigoRastreo { get; set; } = string.Empty;
        
        [Required, StringLength(150)]
        public string Remitente { get; set; } = string.Empty;
        
        [Required, StringLength(150)]
        public string Destinatario { get; set; } = string.Empty;
        
        [Required]
        public int DepartamentoDestinoId { get; set; }
        [ForeignKey("DepartamentoDestinoId")]
        public Departamento? DepartamentoDestino { get; set; }
        
        [Required]
        public int EstadoActualId { get; set; }
        [ForeignKey("EstadoActualId")]
        public Estado? EstadoActual { get; set; }
        
        public int IntentosEntrega { get; set; } = 0;
        public decimal CostoEnvio { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public List<HistorialEstado> HistorialEstados { get; set; } = new();
    }
}