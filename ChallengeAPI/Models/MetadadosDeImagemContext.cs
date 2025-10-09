using Microsoft.EntityFrameworkCore;

namespace ChallengeAPI.Models
{
    public class MetadadosDeImagemContext : DbContext
    {
        public MetadadosDeImagemContext(DbContextOptions<MetadadosDeImagemContext> options)
            : base(options)
        {
        }
        public DbSet<MetadadosDeImagem> MetadadosDeImagens { get; set; } = null!;
    }
}
