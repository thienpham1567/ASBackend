using Backend.Models;

namespace Backend.Services
{
    public interface IAppointmentRepository
    {
        List<AppointmentVM> GetAll(  );
        AppointmentVM GetById(int id);
        AppointmentVM Create(AppointmentModel ap);
        bool Update(int id, AppointmentModel ap);
        AppointmentVM Delete(int id);
    }
}