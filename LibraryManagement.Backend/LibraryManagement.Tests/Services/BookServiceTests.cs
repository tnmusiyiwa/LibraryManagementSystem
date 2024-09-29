using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.API.Data;
using LibraryManagement.API.Models;
using LibraryManagement.API.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibraryManagement.Tests.Services
{
    public class BookServiceTests
    {
        private async Task<ApplicationDbContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new ApplicationDbContext(options);
            databaseContext.Database.EnsureCreated();
            if (await databaseContext.Books.CountAsync() <= 0)
            {
                for (int i = 1; i <= 10; i++)
                {
                    databaseContext.Books.Add(new Book()
                    {
                        Id = i,
                        Title = $"Book {i}",
                        Author = $"Author {i}",
                        ISBN = $"ISBN{i}",
                        PublicationYear = 2000 + i,
                    });
                    await databaseContext.SaveChangesAsync();
                }
            }
            return databaseContext;
        }

        [Fact]
        public async Task GetAllBooks_ReturnsAllBooks()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var bookService = new BookService(dbContext, null, null, null);

            // Act
            var result = await bookService.GetAllBooksAsync(1, 10, "");

            // Assert
            Assert.Equal(10, result.Count());
        }

        [Fact]
        public async Task GetBookById_ReturnsCorrectBook()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var bookService = new BookService(dbContext, null, null, null);

            // Act
            var result = await bookService.GetBookByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Book 1", result.Title);
        }

        [Fact]
        public async Task AddBook_AddsNewBook()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var bookService = new BookService(dbContext, null, null, null);
            var newBook = new Book
            {
                Title = "New Book",
                Author = "New Author",
                ISBN = "NewISBN",
                PublicationYear = 2023,
            };

            // Act
            await bookService.AddBookAsync(newBook);
            var result = await bookService.GetBookByIdAsync(newBook.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Book", result.Title);
        }

        [Fact]
        public async Task UpdateBook_UpdatesExistingBook()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var bookService = new BookService(dbContext, null, null, null);
            var bookToUpdate = await bookService.GetBookByIdAsync(1);
            bookToUpdate.Title = "Updated Book";

            // Act
            await bookService.UpdateBookAsync(bookToUpdate);
            var result = await bookService.GetBookByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Book", result.Title);
        }

        [Fact]
        public async Task DeleteBook_RemovesBook()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var bookService = new BookService(dbContext, null, null, null);

            // Act
            await bookService.DeleteBookAsync(1);
            var result = await bookService.GetBookByIdAsync(1);

            // Assert
            Assert.Null(result);
        }
    }
}