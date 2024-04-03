using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyWebApplication.Models;

namespace MyWebApplication.Data
{
    public class MyWebApplicationContext : DbContext
    {
        public MyWebApplicationContext (DbContextOptions<MyWebApplicationContext> options)
            : base(options)
        {
        }

        public DbSet<MyWebApplication.Models.Cell> Cell { get; set; } = default!;
        
        
        public DbSet<MyWebApplication.Models.Board> Board { get; set; } = default!;
        
        
    }
}
