using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestWeb.Models;

namespace TestWeb.Data
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options)
            : base(options)
        {
        }

        public DbSet<Login> Logins { get; set; }

        public DbSet<Register_Input> Registers { get; set; }

        public DbSet<Board> Boards { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Board_view> BoardViews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Login>().ToView("LOGIN");
            modelBuilder.Entity<Board_view>().ToView("Board_view");
            modelBuilder.Entity<Board>().ToTable("Board");
            modelBuilder.Entity<Register_Input>().ToTable("USER");
            modelBuilder.Entity<Comment>().ToTable("Comment");
        }
    }
}
