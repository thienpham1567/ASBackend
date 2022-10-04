using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public string? Title { get; set; }

        public DateTime StartsFrom { get; set; }

        public DateTime EndsAt { get; set; }

        public string? Note { get; set; }


    }
}