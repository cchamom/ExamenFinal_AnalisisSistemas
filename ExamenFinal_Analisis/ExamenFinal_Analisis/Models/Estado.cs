using System.ComponentModel.DataAnnotations;

namespace ExamenFinal_Analisis.Models
{
    public class Estado
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(50)]
        public string Nombre { get; set; } = string.Empty; // Registrado, EnReparto, etc.
        public int Orden { get; set; } // Para validar el flujo estricto (1, 2, 3...)
    }
}