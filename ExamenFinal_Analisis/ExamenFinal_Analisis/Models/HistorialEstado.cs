using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamenFinal_Analisis.Models
{
    public class HistorialEstado
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int PaqueteId { get; set; }
        [ForeignKey("PaqueteId")]
        public Paquete? Paquete { get; set; }
        
        [Required]
        public int EstadoId { get; set; }
        [ForeignKey("EstadoId")]
        public Estado? Estado { get; set; }
        
        [Required]
        [StringLength(255)]
        public string Ubicacion { get; set; } = string.Empty;
        
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
        public string? Observaciones { get; set; }
    }
}