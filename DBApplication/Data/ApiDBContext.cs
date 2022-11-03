using DBApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace DBApplication.Data
{
    public class ApiDBContext: DbContext
    {
        public ApiDBContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Auth> Users { get; set; }

        public DbSet<Report> Reports { get; set; }
    }
}
