using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Identity.Autenticacao.Model
{
    public class MyRole : IdentityRole
    {
        public MyRole(String name) : base(name)
        { 
        
        }
    }
}
