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
using WebApp.Identity.Models;

namespace WebApp.Identity
{
    public class Startup
    {

        const string connectionString = @"Integrated Security=SSPI;Persist Security Info=False;User ID=sa;Initial Catalog=IdentityCurso;Data Source=DESKTOP-G0OPKKF\SQLEXPRESS";

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

            services.AddDbContext<MyUserDbContext>(
                opt => opt.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationAssembly))
            );

            services.AddIdentity<MyUser, IdentityRole>(options => {
                options.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<MyUserDbContext>()
                .AddDefaultTokenProviders(); // DEFAULT TOKEN PROVIDER � UM PROVEDOR DE TOKENS PADR�O

            services.AddScoped<IUserClaimsPrincipalFactory<MyUser>, MyUserClaimsPrincipalFactory>();

            services.Configure<DataProtectionTokenProviderOptions>(
                options => options.TokenLifespan = TimeSpan.FromHours(3) // CONFIGURA��O DE TIMER DE VALIDADE DO TOKEN            
            );

            services.ConfigureApplicationCookie(options => options.LoginPath = "/Home/Login"); 


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication(); // FAZ A APLICA��O USAR AUTENTICA��O

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
