using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestWeb.Models;

namespace TestWeb.Data
{
    public class TestWebContext : DbContext
    {
        public TestWebContext (DbContextOptions<TestWebContext> options)
            : base(options)
        {
        }

        public DbSet<Login> User { get; set; } = default!;
    }
}
