using Microsoft.EntityFrameworkCore;
using ArticleApp.Models;


namespace ArticleApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Article> Articles { get; set; }
    }
}
