using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using ExamenFinal_Analisis.Models;

namespace ExamenFinal_Analisis.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Paquete> Paquetes { get; set; }
        public DbSet<HistorialEstado> HistorialEstados { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Estado> Estados { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Estado>().HasData(
                new Estado { Id = 1, Nombre = "Registrado", Orden = 1 },
                new Estado { Id = 2, Nombre = "EnReparto", Orden = 2 },
                new Estado { Id = 3, Nombre = "Entregado", Orden = 3 },
                new Estado { Id = 4, Nombre = "En Devolucion", Orden = 4 },
                new Estado { Id = 5, Nombre = "Devuelto", Orden = 5 }
            );

            string[] deptos = {
                "Guatemala", "Alta Verapaz", "Baja Verapaz", "Chimaltenango", "Chiquimula", 
                "El Progreso", "Escuintla", "Huehuetenango", "Izabal", "Jalapa", 
                "Jutiapa", "Petén", "Quetzaltenango", "Quiché", "Retalhuleu", 
                "Sacatepéquez", "San Marcos", "Santa Rosa"
            };

            for (int i = 0; i < deptos.Length; i++)
            {
                modelBuilder.Entity<Departamento>().HasData(
                    new Departamento { Id = i + 1, Nombre = deptos[i] }
                );
            }
        }
    }
}