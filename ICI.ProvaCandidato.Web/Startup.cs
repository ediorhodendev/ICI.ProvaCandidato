using ICI.ProvaCandidato.Dados;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.EntityFrameworkCore;
using static System.Formats.Asn1.AsnWriter;
using ICI.ProvaCandidato.Negocio;
namespace ICI.ProvaCandidato.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddScoped<UsuarioService>();
            services.AddScoped<TagService>();

            services.AddScoped<NoticiaService>();


            services.AddDbContext<TagDbContext>(options =>
            {
                options.UseInMemoryDatabase("tagbancodedados");
            });


        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();


            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<TagDbContext>();
                DataSeeder.Initialize(dbContext); // Inicializa o banco de dados com registros de amostra
            }
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "Cadastro",
                    pattern: "Tags/Cadastro",
                    defaults: new { controller = "Home", action = "Cadastro" });

                endpoints.MapControllerRoute(
                    name: "TagsEdicao",
                    pattern: "Tags/Edicao/{id}",
                    defaults: new { controller = "Home", action = "Cadastro" }); 



                                endpoints.MapControllerRoute(
                    name: "CriarNoticia",
                    pattern: "Noticias/Criar",
                    defaults: new { controller = "Noticias", action = "Criar" });


                endpoints.MapControllerRoute(
                 name: "CriarNoticia",
                     pattern: "Noticias/Criar",
                    defaults: new { controller = "Noticias", action = "Criar" }
                );
                endpoints.MapControllerRoute(
       name: "EditarNoticia",
       pattern: "Noticias/Criar/{id}",
       defaults: new { controller = "Noticias", action = "Editar" }); 


                endpoints.MapControllerRoute(
                  name: "CadastroUsuario",
                  pattern: "Usuarios/Criar",
                  defaults: new { controller = "Usuarios", action = "Criar" });

                                endpoints.MapControllerRoute(
                                    name: "EditarUsuario",
                                    pattern: "Usuarios/Criar/{id}",
                                    defaults: new { controller = "Usuarios", action = "Criar" });



                endpoints.MapControllerRoute(
    name: "ConfirmDeleteUsuario",
    pattern: "Usuarios/ConfirmDelete/{id}",
    defaults: new { controller = "Usuarios", action = "ConfirmDelete" });



            });

      




        }
    }
}
