using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApp.Identity.Autenticacao;
using WebApp.Identity.Autenticacao.Model;
using WebApp.Identity.Enum;
using WebApp.Identity.Models;

namespace WebApp.Identity
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

            var migrationAssembly = typeof(Startup)
               .GetTypeInfo().Assembly
               .GetName().Name;

            services.AddDbContext<WebAppIdentityDbContext>(
                opt => opt.UseMySql(Configuration.GetConnectionString("WebAppIdentityContext"),
                builder => builder.MigrationsAssembly(migrationAssembly))
            );

            services.AddIdentity<MyUser, MyRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;

                // CONFIGURAÇÕES DE SENHAS
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
                .AddEntityFrameworkStores<WebAppIdentityDbContext>()
                .AddDefaultTokenProviders() // DEFAULT TOKEN PROVIDER É UM PROVEDOR DE TOKENS PADRÃO
                .AddPasswordValidator<ValidatorPassword<MyUser>>(); // ADICIONADO CLASSE DE VALIDAÇÕES DE PASSWORD

            services.AddScoped<IUserClaimsPrincipalFactory<MyUser>, MyUserClaimsPrincipalFactory>();

            services.Configure<DataProtectionTokenProviderOptions>(
                options => options.TokenLifespan = TimeSpan.FromHours(3) // CONFIGURAÇÃO DE TIMER DE VALIDADE DO TOKEN            
            );

            services.ConfigureApplicationCookie(options => options.LoginPath = "/Home/Login");

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication(); // FAZ A APLICAÇÃO USAR AUTENTICAÇÃO

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Login}/{id?}");
            });
            CreateRoles(serviceProvider).Wait();
        }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<MyRole>>();
            string[] rolesNames = { EnumPerfil.Administrador.ToString(), 
                                    EnumPerfil.Coordenador.ToString(), 
                                    EnumPerfil.Professor.ToString() };
            IdentityResult result;
            foreach (var namesRole in rolesNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(namesRole);
                if (!roleExist)
                {
                    result = await roleManager.CreateAsync(new MyRole(namesRole));
                }
            }
        }
    }
}
