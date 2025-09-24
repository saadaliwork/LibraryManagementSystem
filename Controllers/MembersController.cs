using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin,Member")]
    public class MembersController : Controller
    {
        private readonly IRepository<Member> _memberRepository;
        private readonly ILogger<MembersController> _logger;

        public MembersController(IRepository<Member> memberRepository, ILogger<MembersController> logger)
        {
            _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index(int? days)
        {
            try
            {
                _logger.LogInformation("Attempting to load members...");
                var members = _memberRepository.GetAll();
                if (days.HasValue)
                {
                    var cutoffDate = DateTime.Now.AddDays(-days.Value);
                    members = members.Where(m => m.JoinDate >= cutoffDate).ToList();
                }
                if (members == null || !members.Any())
                {
                    _logger.LogWarning("No members found.");
                    ViewBag.Days = days;
                    return View(new List<Member>());
                }
                _logger.LogInformation($"Loaded {members.Count()} members.");
                ViewBag.Days = days;
                return View(members);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading members in Index action.");
                return View("Error");
            }
        }

        [HttpPost]
        public IActionResult SearchMembers(string searchTerm)
        {
            try
            {
                _logger.LogInformation($"Searching members with term: {searchTerm}");
                var members = string.IsNullOrEmpty(searchTerm)
                    ? _memberRepository.GetAll().ToList()
                    : _memberRepository.GetAll()
                        .Where(m => m.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                    m.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                return Json(members.Select(m => new
                {
                    id = m.Id,
                    fullName = m.FullName,
                    email = m.Email,
                    joinDate = m.JoinDate.ToString("yyyy-MM-dd")
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching members.");
                return StatusCode(500, "Error loading members.");
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
        public IActionResult Create([Bind("Id,FullName,Email,JoinDate")] Member member)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _memberRepository.Add(member);
                    _logger.LogInformation($"Member created: {member.FullName}");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating member.");
                    return View("Error");
                }
            }
            return View(member);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            try
            {
                var member = _memberRepository.GetById(id);
                if (member == null)
                {
                    _logger.LogWarning($"Member with ID {id} not found.");
                    return NotFound();
                }
                return View(member);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading member with ID {id} for edit.");
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id, [Bind("Id,FullName,Email,JoinDate")] Member member)
        {
            if (id != member.Id)
            {
                _logger.LogWarning($"Member ID mismatch: {id} vs {member.Id}.");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _memberRepository.Update(member);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating member with ID {id}.");
                    return View("Error");
                }
            }
            return View(member);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                var member = _memberRepository.GetById(id);
                if (member == null)
                {
                    _logger.LogWarning($"Member with ID {id} not found.");
                    return NotFound();
                }
                return View(member);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading member with ID {id} for delete.");
                return View("Error");
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
                _memberRepository.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting member with ID {id}.");
                return View("Error");
            }
        }
    }
}