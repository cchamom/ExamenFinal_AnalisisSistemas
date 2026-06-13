using Microsoft.AspNetCore.Mvc;
using ExamenFinal_Analisis.Models;
using ExamenFinal_Analisis.Services;
using System.Threading.Tasks;

namespace ExamenFinal_Analisis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PruebaApiController : ControllerBase
    {
        private readonly IPaqueteService _paqueteService;

        public PruebaApiController(IPaqueteService paqueteService)
        {
            _paqueteService = paqueteService;
        }

        /// <summary>
        /// Endpoint 3: Actualizar el estado físico de un paquete validando el flujo lógico 
        /// e incrementando reintentos fallidos si aplica (Automático a Devolución al 3er fallo).
        /// URL: POST /api/pruebaapi/actualizar?codigo=ENV-XXX&nuevoEstadoId=2&ubicacion=Escuintla&esIntentoFallido=false
        /// </summary>
        [HttpPost("actualizar")]
        public async Task<IActionResult> Actualizar(
            [FromQuery] string codigo, 
            [FromQuery] int nuevoEstadoId, 
            [FromQuery] string ubicacion, 
            [FromQuery] bool esIntentoFallido = false,
            [FromQuery] string? observaciones = null)
        {
            if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(ubicacion))
            {
                return BadRequest(new { error = "El código del paquete y la ubicación actual son campos obligatorios." });
            }

            var resultado = await _paqueteService.ActualizarEstadoAsync(codigo, nuevoEstadoId, ubicacion, esIntentoFallido);
            
            if (!resultado.Success)
            {
                return BadRequest(new { error = resultado.Message });
            }
            
            return Ok(new { mensaje = resultado.Message });
        }

        /// <summary>
        /// Endpoint 4: Alerta / Reporte de paquetes críticos con múltiples intentos de entrega fallidos.
        /// URL: GET /api/pruebaapi/multiples-intentos
        /// </summary>
        [HttpGet("multiples-intentos")]
        public async Task<IActionResult> ObtenerPaquetesConAlertas()
        {
            var lista = await _paqueteService.ObtenerPaquetesConMultiplesIntentosAsync();
            return Ok(new {
                descripcion = "Paquetes que registran incidencias o reintentos en su distribución",
                conteo = lista.Count,
                paquetes = lista
            });
        }

        /// <summary>
        /// Endpoint 5: Listar todos los departamentos guardados en la semilla de SQLite (Para combos/selects si se requiere).
        /// URL: GET /api/pruebaapi/departamentos
        /// </summary>
        [HttpGet("departamentos")]
        public async Task<IActionResult> ObtenerDepartamentos()
        {
            var departamentos = await _paqueteService.ObtenerDepartamentosAsync();
            return Ok(departamentos);
        }
    }
}