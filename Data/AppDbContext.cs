using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HelpDeskMvc.Models;

namespace HelpDeskMvc.Data
{
    public class AppDbContext : IdentityDbContext<Usuario>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Chamado> Chamados => Set<Chamado>();

        public DbSet<Produto> Produtos => Set<Produto>();
    }
}
