using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Identity.Models;

namespace WebApp.Identity
{
    public class MyUserDbContext : IdentityDbContext<MyUser>
    {
        public MyUserDbContext(DbContextOptions<MyUserDbContext> options) : base(options)
        { 
        
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // NO DBCONTEXT FICA AS REGRAS DE BANCO DE DADOS
            base.OnModelCreating(builder);

            builder.Entity<Organization>(org => {
                // Para a tabela organizations
                org.ToTable("Organizations");
                // É definido qual o ID
                org.HasKey(x => x.Id);
                // Uma organização pode ter muitos Usuários
                org.HasMany<MyUser>()
                    // E um usuário possui uma organização
                    .WithOne()
                    // Define qual o campo ta tabela myuser que é foreign key da tabela organization
                    .HasForeignKey(x => x.OrganizationId)
                    // Define se a foreign key é obrigatória
                    .IsRequired(false);
            });
        }
    }
}
