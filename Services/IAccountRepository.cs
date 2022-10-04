using Backend.Models;

namespace Backend.Services
{
    public interface IAccountRepository
    {
        User ValidateLogin(UserLogin model);

        ApiResponse ValidateRegister(User model);
    }
}