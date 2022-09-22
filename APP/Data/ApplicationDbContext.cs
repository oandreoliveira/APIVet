using ApiVet.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiVet.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Veterinario> Veterinarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Cachorro> Cachorros { get; set; }
        public DbSet<Consulta> Consultas { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }
    }
}