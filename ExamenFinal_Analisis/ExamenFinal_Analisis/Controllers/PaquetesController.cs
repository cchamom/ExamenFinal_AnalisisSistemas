using Microsoft.AspNetCore.Mvc;
using ExamenFinal_Analisis.Models;
using ExamenFinal_Analisis.Services;
using System.Threading.Tasks;

namespace ExamenFinal_Analisis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaquetesController : ControllerBase
    {
        private readonly IPaqueteService _paqueteService;

        public PaquetesController(IPaqueteService paqueteService)
        {
            _paqueteService = paqueteService;
        }

        /// <summary>
        /// Endpoint 1: Registrar un nuevo paquete y calcular su costo automático.
        /// URL: POST /api/paquetes/nuevo?ubicacionInicial=Ciudad de Guatemala
        /// </summary>
        [HttpPost("nuevo")]
        public async Task<IActionResult> Nuevo([FromBody] Paquete paquete, [FromQuery] string ubicacionInicial)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(ubicacionInicial))
            {
                return BadRequest(new { mensaje = "La ubicación inicial es obligatoria para el historial." });
            }

            var resultado = await _paqueteService.RegistrarPaqueteAsync(paquete, ubicacionInicial);
            return Ok(new { 
                mensaje = "¡Paquete registrado con éxito en SQLite!", 
                codigoRastreo = resultado.CodigoRastreo, 
                costoEnvio = resultado.CostoEnvio,
                datos = resultado 
            });
        }

        /// <summary>
        /// Endpoint 2: Buscar un paquete por su código y obtener su ciclo de vida con historial.
        /// URL: GET /api/paquetes/rastreo/ENV-YYYYMMDD-XXXX
        /// </summary>
        [HttpGet("rastreo/{codigoRastreo}")]
        public async Task<IActionResult> Buscar(string codigoRastreo)
        {
            if (string.IsNullOrEmpty(codigoRastreo))
            {
                return BadRequest(new { mensaje = "Debe proporcionar un código de rastreo válido." });
            }

            var paquete = await _paqueteService.ObtenerPorCodigoAsync(codigoRastreo.Trim());
            if (paquete == null)
            {
                return NotFound(new { mensaje = $"El código '{codigoRastreo}' no existe en la base de datos." });
            }

            return Ok(paquete);
        }
    }
}