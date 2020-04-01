﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
            base.OnModelCreating(builder);
        }
    }
}
