namespace Backend.Models
{
    public class AppointmentVM
    {
        public int id {get;set;}

        public int userId {get;set;}

        public string? title {get;set;}

        public string? start {get;set;}

        public string? end {get;set;}

        public string? note {get;set;}

        
    }
}