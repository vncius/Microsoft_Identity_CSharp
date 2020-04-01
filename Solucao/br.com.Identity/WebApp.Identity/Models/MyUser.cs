using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Identity.Models
{
    public class MyUser : IdentityUser
    {
        public string nomeCompleto { get; set; }
    }
}