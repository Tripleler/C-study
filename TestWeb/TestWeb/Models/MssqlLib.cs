using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace TestWeb.Models
{
    public class AspnetNoteDbContext : DbContext
    {

        public DbSet<User> Users { get; set; } // 테이블 생성하는 코드

        //public DbSet<Note> Notes { get; set; }

        /* DB와 연결하는 커넥션 */
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=192.168.1.3,1433;Initial Catalog=USER;User ID=symation;Password=tlapdltus!@#; ");
        }
    }
}
