using Microsoft.EntityFrameworkCore;
using Oracle.EntityFrameworkCore;

namespace axiosTest.Models
{
    public class OracleDbContext : DbContext
    {
        public DbSet<FormInfo> FormInfo { get; set; }
        
        public OracleDbContext(DbContextOptions<OracleDbContext> options):base(options)
        {
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
