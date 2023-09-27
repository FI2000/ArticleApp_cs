using ArticleApp.Models;
using Microsoft.EntityFrameworkCore;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using ArticleApp.Data;
using ArticleApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ArticleAppUnitTests
{
    public class ArticlesControllerTests
    {
        private readonly IFixture _fixture; // AutoFixture
        private readonly Mock<AppDbContext> _mockDbContext; // Mocked DbContext
        private readonly ArticlesController _controller; // System Under Test (SUT)
        private readonly List<Article> _articles;
        public ArticlesControllerTests()
        {
            // Initialize AutoFixture with AutoMoq
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
  
            // Create mock data for DbSet (using AutoFixture)
            _articles = _fixture.CreateMany<Article>(10).AsQueryable().ToList();
            
            for (int i = 0; i < _articles.Count; i++)
            {
                _articles[i].Id = i + 1;
            }

            // Create a mock DbContext with Moq.EntityFrameworkCore
            _mockDbContext = new Mock<AppDbContext>();
            _mockDbContext.Setup(x => x.Articles).ReturnsDbSet(_articles);

            // Mock Update
            _mockDbContext.Setup(x => x.Update(It.IsAny<Article>())).Callback<Article>(article =>
            {
                var index = _articles.FindIndex(a => a.Id == article.Id);
                if (index >= 0)
                {
                    _articles[index] = article;
                }
            });

            // Mock Create
            _mockDbContext.Setup(x => x.Add(It.IsAny<Article>())).Callback<Article>(article =>
            {
                var index = _articles.FindIndex(a => a.Id == article.Id);
                if (index >= 0)
                {
                    _articles[index] = article;
                }
            });

            // Setup mock for SaveChangesAsync
            _mockDbContext.Setup(db => db.SaveChangesAsync(default(CancellationToken))).ReturnsAsync(1);


            // Initialize the controller (SUT)
            _controller = new ArticlesController(_mockDbContext.Object);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Index_ReturnArticlesView_CorrectCount()
        {
            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Article>>(viewResult.Model);
            Assert.Equal(10, model.Count());

        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Index_SortsByDate_WhenOrderByIsDate()
        {
            // Act
            var result = await _controller.Index("date");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Article>>(viewResult.Model);
            var sortedModel = model.OrderByDescending(a => a.PublishedOn).ToList();
            Assert.Equal(sortedModel, model.ToList());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Index_DefaultsToSortingByViews_WhenOrderByIsNotProvided()
        {
            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Article>>(viewResult.Model); // IEnumerable is list
            var sortedModel = model.OrderByDescending(a => a.Views).ToList();

            Assert.Equal(sortedModel, model.ToList());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Details_ReturnArticleView_CorrectArticle()
        {
            // Arrange
            var targetArticleId = 4;
            var targetArticle = _articles.SingleOrDefault(a => a.Id == targetArticleId);

            // Setup mock DbSet behavior for FindAsync
            _mockDbContext.Setup(db => db.Articles.FindAsync(targetArticleId))
                .ReturnsAsync(targetArticle);

            // Act
            var result = await _controller.Details(targetArticleId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Article>(viewResult.Model); // No list
            Assert.Equal(model, targetArticle);
            _mockDbContext.Verify(db => db.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Create_WithValidModel_AddsArticleAndRedirects()
        {
            // Arrange
            var newArticle = new Article
            {
                Id = 11, // Assuming 10 articles already exist
                Author = "New Author",
                Title = "New Title",
                Content = "New Content",
                ArticleTagsAsString = "Tag1,Tag2",
                Category = Category.Travel
            };

            List<Article> addedArticles = new List<Article>();

            _mockDbContext.Setup(db => db.Add(It.IsAny<Article>()))
                .Callback<Article>(article => addedArticles.Add(article));

            // Act
            var result = await _controller.Create(newArticle);

            // Assert
            _mockDbContext.Verify(db => db.SaveChangesAsync(default), Times.Once);
            Assert.Single(addedArticles);
            Assert.Equal(newArticle, addedArticles[0]);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Create_WithInvalidModel_ReturnsViewWithSameArticle()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Sample Error"); // Invalid ModelState

            var newArticle = new Article
            {
                Author = "New Author",
                Title = "New Title",
                Content = "New Content"
            };

            // Act
            var result = await _controller.Create(newArticle);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Article>(viewResult.Model);
            Assert.Equal(newArticle, model);
        }
    }

    
}

