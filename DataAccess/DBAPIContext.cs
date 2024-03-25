using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace DataAccess
{
    public class DBAPIContext : DbContext
    {
        public DBAPIContext(DbContextOptions<DBAPIContext> options)
            : base(options)
        {
            
        }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categoria>().ToTable("Categoria");
            modelBuilder.Entity<Producto>().ToTable("Producto");
            modelBuilder.Entity<Usuario>().ToTable("Usuario");
        }
    }
}
