using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class User
    {
        public int Id { get; set; }

        [Column(TypeName = "varchar(100)")]
        [Required]
        public string? FullName { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string? Username { get; set; }

        [Column(TypeName = "varchar(100)")]
        [Required]
        public string? Email { get; set; }

        [Column(TypeName = "varchar(100)")]
        [Required]
        public string? Password { get; set; }


        [Column(TypeName = "varchar(20)")]
        [Required]
        public string? PhoneNumber { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string? Role { get; set; }

        public ICollection<Appointment>? Appointments { get; set; }
    }
}