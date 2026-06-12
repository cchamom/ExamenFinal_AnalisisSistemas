using Microsoft.AspNetCore.Mvc;

namespace ExamenFinal_Analisis.Controllers
{
    // Define que la ruta base será: http://tu-dominio/api/prueba
    [ApiController]
    [Route("api/[controller]")]
    public class PruebaApiController : ControllerBase
    {
        // Endpoint de tipo GET: api/prueba
        [HttpGet]
        public IActionResult GetDatos()
        {
            var respuesta = new
            {
                Mensaje = "¡Hola! Esta es una respuesta desde la API en Render",
                FechaServidor = DateTime.Now,
                Estado = "Activo",
                Estudiante = "Cristian Chamo"
            };

            return Ok(respuesta); // Devuelve un estado 200 OK junto con el objeto JSON
        }

        // Endpoint GET con parámetros: api/prueba/saludo?nombre=Cristian
        [HttpGet("saludo")]
        public IActionResult GetSaludo([FromQuery] string nombre)
        {
            if (string.IsNullOrEmpty(nombre))
            {
                return BadRequest(new { Error = "El parámetro 'nombre' es requerido." });
            }

            return Ok(new { Saludo = $"Hola {nombre}, bienvenido a mi API." });
        }
    }
}