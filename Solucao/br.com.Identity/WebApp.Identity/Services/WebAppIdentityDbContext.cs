using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApp.Identity.Autenticacao.Model;
using WebApp.Identity.Models;

namespace WebApp.Identity
{
    public class WebAppIdentityDbContext : IdentityDbContext<MyUser, MyRole, string>
    {
        public WebAppIdentityDbContext(DbContextOptions<WebAppIdentityDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // NO DBCONTEXT FICA AS REGRAS DE BANCO DE DADOS
            base.OnModelCreating(builder);




        }
    }
}
