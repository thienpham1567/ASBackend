using Backend.Data;
using Backend.Models;
using BC = BCrypt.Net.BCrypt;


namespace Backend.Services
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _context;

        public AccountRepository(AppDbContext context)
        {
            _context = context;
        }

        public User ValidateLogin(UserLogin model)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == model.UserEmail);
                if (user != null && BC.Verify(model.Password, user.Password))
                {
                    return user;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public ApiResponse ValidateRegister(User model)
        {
            try
            {
                var newUser = new User
                {
                    FullName = model.FullName,
                    Username = model.Username,
                    Email = model.Email,
                    Password = model.Password,
                    PhoneNumber = model.PhoneNumber,
                    Role = model.Role != null ? model.Role : "User"
                };
                newUser.Password = BC.HashPassword(newUser.Password);
                _context.Users.Add(newUser);
                _context.SaveChanges();
                return new ApiResponse
                {
                    Success = true,
                    Message = "signed up successfully"
                };
            }
            catch
            {

                return new ApiResponse
                {
                    Success = false,
                    Message = "Something went wrong"
                };
            }
        }

        //    private bool verifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        // {
        //     using (var hmac = new HMACSHA512(passwordSalt))
        //     {
        //         var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        //         return hash.SequenceEqual(passwordHash);
        //     }
        // }
    }
}