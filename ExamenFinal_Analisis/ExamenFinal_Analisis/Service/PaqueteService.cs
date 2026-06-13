using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExamenFinal_Analisis.Data;
using ExamenFinal_Analisis.Models;

namespace ExamenFinal_Analisis.Services
{
    public class PaqueteService : IPaqueteService
    {
        private readonly ApplicationDbContext _context;

        public PaqueteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Departamento>> ObtenerDepartamentosAsync()
        {
            return await _context.Departamentos.OrderBy(d => d.Nombre).ToListAsync();
        }

        public async Task<Paquete> RegistrarPaqueteAsync(Paquete paquete, string ubicacionInicial)
        {
            // Generación de Código Único: ENV-YYYYMMDD-XXXX
            string fechaStr = DateTime.Today.ToString("yyyyMMdd");
            int conteoHoy = await _context.Paquetes.CountAsync(p => p.FechaRegistro >= DateTime.Today) + 1;
            paquete.CodigoRastreo = $"ENV-{fechaStr}-{conteoHoy:D4}";
            
            paquete.EstadoActualId = 1; // 1 = Registrado
            paquete.CostoEnvio = 35.00m; // Cálculo base automático
            paquete.FechaRegistro = DateTime.UtcNow;

            _context.Paquetes.Add(paquete);
            await _context.SaveChangesAsync();

            // Agregar registro inicial al historial
            _context.HistorialEstados.Add(new HistorialEstado
            {
                PaqueteId = paquete.Id,
                EstadoId = 1,
                Ubicacion = ubicacionInicial,
                Observaciones = "Paquete ingresado al sistema automáticamente."
            });
            await _context.SaveChangesAsync();

            return paquete;
        }

        public async Task<Paquete?> ObtenerPorCodigoAsync(string codigo)
        {
            return await _context.Paquetes
                .Include(p => p.DepartamentoDestino)
                .Include(p => p.EstadoActual)
                .Include(p => p.HistorialEstados).ThenInclude(h => h.Estado)
                .FirstOrDefaultAsync(p => p.CodigoRastreo == codigo);
        }

        public async Task<(bool Success, string Message)> ActualizarEstadoAsync(string codigo, int nuevoEstadoId, string ubicacion, bool esIntentoFallido)
        {
            var paquete = await _context.Paquetes
                .Include(p => p.EstadoActual)
                .FirstOrDefaultAsync(p => p.CodigoRastreo == codigo);

            if (paquete == null) return (false, "El paquete no existe.");

            var estadoActual = await _context.Estados.FindAsync(paquete.EstadoActualId);
            var nuevoEstado = await _context.Estados.FindAsync(nuevoEstadoId);

            if (nuevoEstado == null || estadoActual == null) return (false, "Estado inválido.");

            // Validación de flujo estricto hacia adelante utilizando la columna 'Orden'
            if (nuevoEstado.Orden < estadoActual.Orden)
            {
                return (false, $"Transición inválida. No se puede cambiar de {estadoActual.Nombre} a {nuevoEstado.Nombre}.");
            }

            string observacion = "Actualización rutinaria de logística.";

            if (esIntentoFallido && paquete.EstadoActualId == 2) // 2 = EnReparto
            {
                paquete.IntentosEntrega++;
                observacion = $"Intento de entrega fallido de reparto #{paquete.IntentosEntrega}.";

                if (paquete.IntentosEntrega >= 3)
                {
                    paquete.EstadoActualId = 4; // 4 = En Devolucion
                    observacion += " Límite alcanzado. Pasó automáticamente a 'En Devolucion'.";
                }
            }
            else
            {
                paquete.EstadoActualId = nuevoEstadoId;
            }

            _context.HistorialEstados.Add(new HistorialEstado
            {
                PaqueteId = paquete.Id,
                EstadoId = paquete.EstadoActualId,
                Ubicacion = ubicacion,
                Observaciones = observacion
            });

            await _context.SaveChangesAsync();
            return (true, "Estado actualizado con éxito.");
        }

        public async Task<List<Paquete>> ObtenerPaquetesConMultiplesIntentosAsync()
        {
            return await _context.Paquetes
                .Include(p => p.DepartamentoDestino)
                .Include(p => p.EstadoActual)
                .Where(p => p.IntentosEntrega > 0)
                .OrderByDescending(p => p.IntentosEntrega)
                .ToListAsync();
        }
    }
}