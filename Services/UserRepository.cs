using Backend.Models;
using Backend.Data;

namespace Backend.Services
{
    public class UserRepository : IUserRepository
    {

        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<User> GetAllUsers()
        {
            var users = _context.Users!.Where(u => u.Role != "Admin");
            return users.ToList();
        }

        public User GetUserById(int id)
        {
               var user = _context.Users!.SingleOrDefault(e => e.Id == id);
               if(user != null)
               {
                return user;
               }
               return null;
        }

        public User DeleteUser(int id)
        {
               var user = _context.Users!.SingleOrDefault(e => e.Id == id);
                 if (user != null)
            {
                _context.Remove(user);
                _context.SaveChanges();
                return user;
            }
            return null;
        }

    }
}