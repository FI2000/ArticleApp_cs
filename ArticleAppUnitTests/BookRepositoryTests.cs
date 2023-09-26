using ArticleApp.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticleAppTests
{
    public class BookRepositoryTests
    {
        private readonly AppDbContext _context;
        private readonly IBookRepository _repository;

        public BookRepositoryTests()
        {
            // Code to start MySQL Docker container
            // Set the connection string to connect to this container

            // Additional setup, such as seeding data into the in-memory database
        }

        [Fact]
        public void GetAllBooks_ShouldReturnAllBooks()
        {
            // Your test code here
        }

        // Other test methods

        public void Dispose()
        {
            // Cleanup: Stop the Docker container
            // Dispose of any unmanaged resources, if necessary
        }
    }
}
