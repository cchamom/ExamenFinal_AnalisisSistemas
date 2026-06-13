using System.Collections.Generic;
using System.Threading.Tasks;
using ExamenFinal_Analisis.Models;

namespace ExamenFinal_Analisis.Services
{
    public interface IPaqueteService
    {
        Task<Paquete> RegistrarPaqueteAsync(Paquete paquete, string ubicacionInicial);
        Task<Paquete?> ObtenerPorCodigoAsync(string codigo);
        Task<(bool Success, string Message)> ActualizarEstadoAsync(string codigo, int nuevoEstadoId, string ubicacion, bool esIntentoFallido);
        Task<List<Paquete>> ObtenerPaquetesConMultiplesIntentosAsync();
        Task<List<Departamento>> ObtenerDepartamentosAsync();
    }
}