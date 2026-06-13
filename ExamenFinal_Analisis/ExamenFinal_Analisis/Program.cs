using Microsoft.EntityFrameworkCore;
using ExamenFinal_Analisis.Data;      
using ExamenFinal_Analisis.Services;  

namespace ExamenFinal_Analisis
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
            builder.WebHost.UseUrls($"http://localhost:{port}");

            // =========================================================================
            // NUEVOS SERVICIOS AGREGADOS AL CONTENEDOR (Sin eliminar lo existente)
            // =========================================================================
            
            // 1. Configuración de la conexión a SQLite usando el string de la app o uno por defecto
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=enviosrapidos.db"));

            // 2. Inyección de dependencia de la capa de lógica de negocio
            builder.Services.AddScoped<IPaqueteService, PaqueteService>();

            // =========================================================================

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // =========================================================================
            // INICIALIZACIÓN DE BASE DE DATOS Y SEMILLAS AL ARRANCAR LA APLICACIÓN
            // =========================================================================
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                // Crea la BD SQLite e inserta automáticamente los 18 departamentos y los 5 estados
                db.Database.EnsureCreated(); 
            }
            // =========================================================================

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllers();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Paquetes}/{action=Rastreo}/{id?}") // Nota: Puedes dejarlo como "Home/Index" si prefieres que esa sea tu landing inicial
                .WithStaticAssets();  
            
            app.Run();
        }
    }
}