using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) :base(options){}

        public DbSet<Appointment>? Appointments {get;set;}

        public DbSet<User>? Users {get;set;} 

         public DbSet<RefreshToken>? RefreshTokens {get;set;} 
    }
    
}