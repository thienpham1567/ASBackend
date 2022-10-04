using Backend.Models;

namespace Backend.Services
{
    public interface IUserRepository
    {
        List<User> GetAllUsers();
        User GetUserById(int id);

        User DeleteUser(int id);
    }
}