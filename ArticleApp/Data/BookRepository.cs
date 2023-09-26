using ArticleApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ArticleApp.Data
{
    public interface IBookRepository
    {
        IEnumerable<Book> GetAllBooks();
        Book GetBookById(int id);
        Task AddBook(Book book);
        List<Book> GetBooksByTopic(string topic);
        List<Book> GetBooksWithMorePagesThan(int number);
    }

    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;
        public BookRepository(AppDbContext dbContext) {
            _context = dbContext;
        }

        public IEnumerable<Book> GetAllBooks()
        {
            return _context.Books.ToList();
        }

        public Book GetBookById(int id)
        {
            return _context.Books.FirstOrDefault(b => b.Id == id);
        }

        public async Task AddBook(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public List<Book> GetBooksByTopic(string topic)
        {
            return _context.Books.Where(b => b.Topic == topic).ToList();
        }

        public List<Book> GetBooksWithMorePagesThan(int number)
        {
            return _context.Books.Where(b => b.NumOfPages > number).ToList();
        }
    }
}
