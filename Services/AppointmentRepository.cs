using System.Globalization;
using Backend.Models;
using Backend.Data;

namespace Backend.Services
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;

        public AppointmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<AppointmentVM> GetAll()
        {
            var appointments = _context.Appointments!.Where(a => DateTime.Compare(a.StartsFrom,DateTime.Now) > 0).Select(e => new AppointmentVM
            { 
                id = e.Id,
                userId = e.UserId,
                title = e.Title,
                start = e.StartsFrom.ToString("yyyy-MM-dd HH:mm:ss"),
                end = e.EndsAt.ToString("yyyy-MM-dd HH:mm:ss"),
                note = e.Note

            });
            return appointments.ToList();
        }
        public AppointmentVM GetById(int id)
        {
            var appointment = _context.Appointments!.SingleOrDefault(e => e.Id == id);
            if (appointment != null)
            {
                return new AppointmentVM
                {
                    id = appointment.Id,
                    userId = appointment.UserId,
                    title = appointment.Title,
                    start = appointment.StartsFrom.ToString("yyyy-MM-dd HH:mm:ss"),
                    end = appointment.EndsAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    note = appointment.Note
                };
            }
            return null;
        }
        public AppointmentVM Create(AppointmentModel ap)
        {
            var _ap = new Appointment
            {
                UserId = ap.userId,
                Title = ap.title,
                StartsFrom = DateTime.ParseExact(ap.start!, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                EndsAt = DateTime.ParseExact(ap.end!, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                Note = ap.note
            };
            _context.Add(_ap);
            _context.SaveChanges();
            return returnDataToFrontend(_ap);
        }
        public bool Update(int id, AppointmentModel ap)
        {
            var appointment = _context.Appointments!.SingleOrDefault(e => e.Id == id);
            if (appointment != null)
            {
                appointment.UserId = ap.userId;
                appointment.Title = ap.title;
                appointment.StartsFrom = DateTime.ParseExact(ap.start!, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                appointment.EndsAt = DateTime.ParseExact(ap.end!, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                appointment.Note = ap.note;
                _context.SaveChanges();
                return true;
            }
            return false;

        }
        public AppointmentVM Delete(int id)
        {
            var _ap = _context.Appointments!.SingleOrDefault(e => e.Id == id);
            if (_ap != null)
            {
                _context.Remove(_ap);
                _context.SaveChanges();
                return returnDataToFrontend(_ap);
            }
            return null;
        }

        private AppointmentVM returnDataToFrontend(Appointment _ap)
        {
            return new AppointmentVM
            {
                id = _ap.Id,
                userId = _ap.UserId,
                title = _ap.Title,
                start = _ap.StartsFrom.ToString("yyyy-MM-dd HH:mm:ss"),
                end = _ap.EndsAt.ToString("yyyy-MM-dd HH:mm:ss"),
                note = _ap.Note
            };
        }
    }
}