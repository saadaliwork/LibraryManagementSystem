using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin,Member")]
    public class BooksController : Controller
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IRepository<Book> bookRepository, ApplicationDbContext context, ILogger<BooksController> logger)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index()
        {
            try
            {
                _logger.LogInformation("Attempting to load books directly from context...");
                var books = _context.Books?.ToList() ?? new List<Book>();
                if (!books.Any())
                {
                    _logger.LogWarning("No books found in database.");
                    return View(new List<Book>());
                }
                _logger.LogInformation("Loaded {Count} books.", books.Count);
                return View(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading books in Index action: {Message}", ex.Message);
                return View("Error", new { message = "Failed to load books. Please try again later." });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult SearchBooks(string searchTerm)
        {
            try
            {
                _logger.LogInformation("SearchBooks called with term: {SearchTerm}, User: {User}, Context State: {CanConnect}", searchTerm, User.Identity?.Name, _context.Database.CanConnect());
                if (_context.Books == null)
                {
                    _logger.LogError("Books DbSet is null in SearchBooks.");
                    return StatusCode(500, new { error = "Database context is not initialized." });
                }
                var books = _context.Books.AsNoTracking().ToList();
                if (string.IsNullOrEmpty(searchTerm))
                {
                    _logger.LogInformation("Returning all {Count} books.", books.Count);
                }
                else
                {
                    books = books
                        .Where(b => b.Title != null && b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                    b.Author != null && b.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    _logger.LogInformation("Found {Count} books matching '{SearchTerm}'.", books.Count, searchTerm);
                }
                return Json(books.Select(b => new
                {
                    id = b.Id,
                    title = b.Title ?? "",
                    author = b.Author ?? "",
                    publishedYear = b.PublishedYear,
                    price = b.Price,
                    genre = b.Genre ?? ""
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SearchBooks: {Message}, StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode(500, new { error = $"Error loading books: {ex.Message}" });
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([Bind("Id,Title,Author,PublishedYear,Price,Genre")] Book book)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _bookRepository.Add(book);
                    _logger.LogInformation("Book created: {Title}", book.Title);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating book: {Message}", ex.Message);
                    return View("Error", new { message = "Failed to create book." });
                }
            }
            return View(book);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            try
            {
                var book = _bookRepository.GetById(id);
                if (book == null)
                {
                    _logger.LogWarning("Book with ID {Id} not found.", id);
                    return NotFound();
                }
                return View(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading book with ID {Id} for edit: {Message}", id, ex.Message);
                return View("Error", new { message = "Failed to load book for editing." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id, [Bind("Id,Title,Author,PublishedYear,Price,Genre")] Book book)
        {
            if (id != book.Id)
            {
                _logger.LogWarning("Book ID mismatch: {ProvidedId} vs {BookId}.", id, book.Id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation("Updating book with ID {Id}.", id);
                    _bookRepository.Update(book);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating book with ID {Id}: {Message}", id, ex.Message);
                    return View("Error", new { message = "Failed to update book." });
                }
            }
            return View(book);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                var book = _bookRepository.GetById(id);
                if (book == null)
                {
                    _logger.LogWarning("Book with ID {Id} not found.", id);
                    return NotFound();
                }
                return View(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading book with ID {Id} for delete: {Message}", id, ex.Message);
                return View("Error", new { message = "Failed to load book for deletion." });
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _bookRepository.Delete(id);
                _logger.LogInformation("Deleted book with ID {Id}.", id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting book with ID {Id}: {Message}", id, ex.Message);
                return View("Error", new { message = "Failed to delete book." });
            }
        }

        public IActionResult BooksByYear(int year)
        {
            try
            {
                _logger.LogInformation("Attempting to load books for year {Year} or later...", year);
                var books = _context.Books?.Where(b => b.PublishedYear >= year).ToList() ?? new List<Book>();
                ViewBag.Year = year;
                _logger.LogInformation("Loaded {Count} books for year {Year} or later.", books.Count, year);
                return View(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading books for year {Year} or later: {Message}", year, ex.Message);
                return View("Error", new { message = "Failed to load books for year filter." });
            }
        }
    }
}