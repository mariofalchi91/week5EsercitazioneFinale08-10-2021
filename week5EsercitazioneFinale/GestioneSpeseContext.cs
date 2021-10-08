using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace week5EsercitazioneFinale
{
    public class GestioneSpeseContext : DbContext 
    {
        public DbSet<Spesa> Spese { get; set; }
        public DbSet<Categoria> Categorie { get; set; }
        public GestioneSpeseContext() : base() { }
        public GestioneSpeseContext(DbContextOptions<GestioneSpeseContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
                string connectionStringSQL = config.GetConnectionString("AcademyG");

                optionsBuilder.UseLazyLoadingProxies();
                optionsBuilder.UseSqlServer(connectionStringSQL);
            }
        }
        protected override void OnModelCreating(ModelBuilder mb)
        {
            //spesa
            mb.Entity<Spesa>().HasKey(s => s.Id);
            mb.Entity<Spesa>().Property(s => s.Descrizione).HasMaxLength(500);
            mb.Entity<Spesa>().Property(s => s.Utente).HasMaxLength(100);

            //categoria
            mb.Entity<Categoria>().HasKey(c => c.Id);
            mb.Entity<Categoria>().Property(c => c.Nome).HasMaxLength(100);

            //relazioni
            mb.Entity<Spesa>().HasOne(s => s.Categoria).WithMany(c => c.Spese);
            mb.Entity<Categoria>().HasMany(c => c.Spese).WithOne(s => s.Categoria);
        }
    }
}
