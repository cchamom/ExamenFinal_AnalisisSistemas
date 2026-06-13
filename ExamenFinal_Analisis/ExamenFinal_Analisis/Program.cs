using Microsoft.EntityFrameworkCore;
using ExamenFinal_Analisis.Data;
using ExamenFinal_Analisis.Services;
using Microsoft.OpenApi.Models;

namespace ExamenFinal_Analisis
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Configuración de puertos para Render
            var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
            builder.WebHost.UseUrls($"http://localhost:{port}");

            // 2. Configuración de Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                // Usamos el nombre completo para evitar errores de namespace
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "API Envíos Rápidos GT", Version = "v1" });
            });

            // 3. Servicios de Base de Datos y Lógica
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=enviosrapidos.db"));

            builder.Services.AddScoped<IPaqueteService, PaqueteService>();
            builder.Services.AddControllersWithViews();

            // 4. ÚNICA DEFINICIÓN DE app
            var app = builder.Build();

            // 5. Habilitar Swagger en la RAÍZ
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Envíos Rápidos GT V1");
                c.RoutePrefix = string.Empty; 
            });

            // 6. Inicialización de Base de Datos
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllers();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Paquetes}/{action=Rastreo}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}