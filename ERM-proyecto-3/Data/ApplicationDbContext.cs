using System;
using System.Collections.Generic;
using System.Text;
using ERM_proyecto_3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ERM_proyecto_3.Data
{
    public class ApplicationDbContext : IdentityDbContext<Employee, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ERM_proyecto_3.Models.hollidays> hollidays { get; set; }
    }
}
