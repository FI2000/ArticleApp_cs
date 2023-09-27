using ArticleApp.Data;
using ArticleApp.Models;
using Docker.DotNet.Models;
using Docker.DotNet;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Polly;
using Polly.Retry;
using System;
using AutoFixture;
using AutoFixture.AutoMoq;

namespace ArticleAppTests
{
    public class BookRepositoryTests : IAsyncLifetime
    {
        private DockerClient _dockerClient;
        private string _containerId;
        private BookRepository repository;
        private string _connectionString = "server=localhost;port=3307;database=test_books;user=root;password=root";
        private readonly IFixture _fixture; 

        public BookRepositoryTests()
        {
            _dockerClient = new DockerClientConfiguration().CreateClient();
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
        }

        public async Task InitializeAsync()
        {
            // Pull Docker MySQL image
            Debug.WriteLine(" // Pull Docker MySQL image");
            await _dockerClient.Images.CreateImageAsync(new ImagesCreateParameters
            {
                FromImage = "mysql",
                Tag = "latest"
            }, null, new Progress<JSONMessage>());

            // Create and start a MySQL container
            Debug.WriteLine(" // Create and start a MySQL container");
            var containerCreateResponse = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = "mysql:latest",
                Env = new List<string>
                {
                    "MYSQL_ROOT_PASSWORD=root",
                    "MYSQL_DATABASE=test_books"
                },
                HostConfig = new HostConfig {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                        {
                            { "3306/tcp", new List<PortBinding> { new PortBinding { HostPort = "3307" } } }
                        }
                }
            });
            _containerId = containerCreateResponse.ID;
            Debug.WriteLine($" // Starting Container with id: {containerCreateResponse.ID}");
            await _dockerClient.Containers.StartContainerAsync(containerCreateResponse.ID, new ContainerStartParameters());

            //Initialize db Context
            var options = new DbContextOptionsBuilder<AppDbContext>()
             .UseMySql(_connectionString, new MySqlServerVersion(new System.Version(8, 0, 34)))
             .Options;

            //Waiting until database has started.
            var retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(5));
            var _context = new AppDbContext(options);
            await retryPolicy.ExecuteAsync(async () => {
                using (var context = new AppDbContext(options))
                {
                    Debug.WriteLine(" // Checking connection");
                    await context.Database.OpenConnectionAsync();
                }
            });

            repository = new BookRepository(_context);
            _context.Database.EnsureCreated();
        }

        public async Task DisposeAsync()
        {
            // Stop and remove the container
            await _dockerClient.Containers.StopContainerAsync(_containerId, new ContainerStopParameters());
            await _dockerClient.Containers.RemoveContainerAsync(_containerId, new ContainerRemoveParameters());
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Create_Book()
        {
            // Arrange e
            Console.WriteLine("Try to see me during CICD");
            Debug.WriteLine("Try to see me during CICD - Debug");
            Book book = _fixture.Create<Book>();

            // Act
            await repository.AddBook(book);

            // Assert
            var found = repository.GetBookById(book.Id);
            Assert.Equal(book, found);
        }
    }
}
