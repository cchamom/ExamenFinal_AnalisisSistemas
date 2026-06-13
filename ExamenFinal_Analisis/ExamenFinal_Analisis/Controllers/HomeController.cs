using Microsoft.AspNetCore.Mvc;

namespace ExamenFinal_Analisis.Controllers
{
    [ApiController]
    [Route("")] // Escucha en la raíz del sitio (http://localhost:5000/)
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok(new { 
                sistema = "API de Envíos Rápidos GT", 
                estado = "Operacional", 
                baseDatos = "SQLite Conectada",
                documentacion = new {
                    registrarPaquete = "POST /api/paquetes/nuevo",
                    rastrearPaquete = "GET /api/paquetes/rastreo/{codigo}",
                    actualizarEstado = "POST /api/pruebaapi/actualizar"
                }
            });
        }
    }
}