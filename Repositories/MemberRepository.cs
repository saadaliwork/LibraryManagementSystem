using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Repositories
{
    public class MemberRepository : IRepository<Member>
    {
        private readonly ApplicationDbContext _context;

        public MemberRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Member> GetAll()
        {
            return _context.Members.ToList();
        }

        public Member? GetById(int id) 
        {
            return _context.Members.Find(id);
        }

        public void Add(Member entity)
        {
            _context.Members.Add(entity);
            _context.SaveChanges();
        }

        public void Update(Member entity)
        {
            _context.Members.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var member = _context.Members.Find(id);
            if (member != null)
            {
                _context.Members.Remove(member);
                _context.SaveChanges();
            }
        }
    }
}