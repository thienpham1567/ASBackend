using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class UserLogin
    {
        [Required]
        [MaxLength(100)]
        public string? UserEmail { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Password { get; set; }
    }
}