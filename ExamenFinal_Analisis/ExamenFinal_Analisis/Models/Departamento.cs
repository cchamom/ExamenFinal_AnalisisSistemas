using System.ComponentModel.DataAnnotations;

namespace ExamenFinal_Analisis.Models
{
    public class Departamento
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Nombre { get; set; } = string.Empty;
    }
}