using Microsoft.EntityFrameworkCore;
using ArticleApp.Models;


namespace ArticleApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public virtual DbSet<Article> Articles { get; set; }

        public virtual DbSet<Book> Books { get; set; }
    }
}
