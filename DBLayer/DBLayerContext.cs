using DBLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLayer
{
    public class DBLayerContext: DbContext
    {
        public DbSet<DataSource> Sources { get; set; }
        public DbSet<DataSourceField> SourcesFields { get; set; }
        public DbSet<Publication> Publications{ get; set; }
        public DBLayerContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           /* modelBuilder.Entity<DataSourceField>()
            .HasOne(p => p.Blog)
            .WithMany(b => b.Posts);*/
        }

        public void AddSource(DataSource dataSource)
        {
            Sources.Add(dataSource);
        }

        public DataSource GetDataSourceById(int id)
        {
            return Sources.Where(x => x.Id == id).First();
        }

        public

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=usersdb;Username=postgres;Password=abrakadabra77");
        }
    }
}
