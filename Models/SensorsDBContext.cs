using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Apex.RaspberryPiServer.Models
{
    public class SensorsDBContext : DbContext
    {
        private static bool _created = false;
        public SensorsDBContext()
        {
            if (!_created)
            {
                _created = true;
                Database.EnsureDeleted();
                Database.EnsureCreated();
            }
        }

        // This method connects the context with the database
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "sensors.db" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);

            optionsBuilder.UseSqlite(connection);
        }

        public DbSet<Reading> Readings { get; set; }
    }
}