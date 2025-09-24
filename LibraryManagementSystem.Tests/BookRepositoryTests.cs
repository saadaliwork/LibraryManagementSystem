using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibraryManagementSystem.Tests
{
    public class BookRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly BookRepository _repository;

        public BookRepositoryTests()
        {
            // Set up a fake in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _repository = new BookRepository(_context); // Use BookRepository
        }

        [Fact]
        public void Add_ShouldAddBookToContext()
        {
            // Arrange: Prepare a book to add
            var book = new Book { Title = "Test Book", Author = "Test Author", Price = 10.99m };

            // Act: Add the book
            _repository.Add(book);

            // Assert: Check if the book was added
            var addedBook = _context.Books.FirstOrDefault(b => b.Title == "Test Book");
            Assert.NotNull(addedBook); // Check that a book was found
            Assert.Equal("Test Book", addedBook.Title); // Check the title matches
        }

        [Fact]
        public void GetById_ShouldReturnCorrectBook()
        {
            // Arrange: Add a book to the fake database
            var book = new Book { Title = "Test Book", Author = "Test Author", Price = 10.99m };
            _context.Books.Add(book);
            _context.SaveChanges();
            var id = book.Id;

            // Act: Get the book by its ID
            var retrievedBook = _repository.GetById(id);

            // Assert: Check if the retrieved book is correct
            Assert.NotNull(retrievedBook); // Check that a book was found
            Assert.Equal("Test Book", retrievedBook.Title); // Check the title matches
        }

        [Fact]
        public void GetAll_ShouldReturnAllBooks()
        {
            // Arrange: Add two books to the fake database
            var book1 = new Book { Title = "Book 1", Author = "Author 1", Price = 9.99m };
            var book2 = new Book { Title = "Book 2", Author = "Author 2", Price = 12.99m };
            _context.Books.AddRange(book1, book2);
            _context.SaveChanges();

            // Act: Get all books
            var allBooks = _repository.GetAll();

            // Assert: Check if both books are returned
            Assert.NotNull(allBooks);
            Assert.Equal(2, allBooks.Count()); // Check that 2 books are returned
        }

        [Fact]
        public void Update_ShouldUpdateBook()
        {
            // Arrange: Add a book and save it
            var book = new Book { Title = "Original Title", Author = "Original Author", Price = 10.99m };
            _context.Books.Add(book);
            _context.SaveChanges();
            var id = book.Id;

            // Act: Update the book
            book.Title = "Updated Title";
            _repository.Update(book);

            // Assert: Check if the update worked
            var updatedBook = _context.Books.Find(id);
            Assert.Equal("Updated Title", updatedBook.Title); // Check the title changed
        }

        [Fact]
        public void Delete_ShouldRemoveBook()
        {
            // Arrange: Add a book and save it
            var book = new Book { Title = "Test Book", Author = "Test Author", Price = 10.99m };
            _context.Books.Add(book);
            _context.SaveChanges();
            var id = book.Id;

            // Act: Delete the book
            _repository.Delete(id);

            // Assert: Check if the book is gone
            var deletedBook = _context.Books.Find(id);
            Assert.Null(deletedBook); // Check that no book is found
        }
    }
}