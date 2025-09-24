using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IActionResult Index()
        {
            var totalBooks = _context.Books.Count();
            var totalMembers = _context.Members.Count();
            var mostRecentMember = _context.Members.OrderByDescending(m => m.JoinDate).FirstOrDefault();

            ViewBag.TotalBooks = totalBooks;
            ViewBag.TotalMembers = totalMembers;
            ViewBag.MostRecentMember = mostRecentMember;

            return View();
        }
    }
}