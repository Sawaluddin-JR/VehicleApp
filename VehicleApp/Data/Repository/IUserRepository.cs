using VehicleApp.Models;

namespace VehicleApp.Data.Repository
{
    public interface IUserRepository
    {
        User Create(User user);
        User GetByEmail(string email);
        User GetById(int id);
    }
}
