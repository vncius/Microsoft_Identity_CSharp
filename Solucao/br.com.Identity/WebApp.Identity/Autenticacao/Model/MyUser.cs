using Microsoft.AspNetCore.Identity;
using System;

namespace WebApp.Identity.Models
{
    public class MyUser : IdentityUser
    {
        public string NomeCompleto { get; set; }
        //public int SchoolId { get; set; }
        //public EnumPerfil EnumPerfil { get; set; }
        public override Boolean TwoFactorEnabled { get; set; } = false;
    }
}