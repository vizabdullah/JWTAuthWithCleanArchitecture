using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<ApplicationUser> Users { get; set; }
        public AppDbContext(DbContextOptions options) : base(options)
        {
            
        }
    }
}
