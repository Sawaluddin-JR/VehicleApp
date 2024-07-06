using VehicleApp.Models;

namespace VehicleApp.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly VehicleContext _context;
        public UserRepository(VehicleContext context)
        {
            _context = context;
        }
        User IUserRepository.Create(User user)
        {
            _context.Users.Add(user);
            user.Id = _context.SaveChanges();
            //_context.SaveChanges();

            return user;
        }

        User IUserRepository.GetByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        User IUserRepository.GetById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }
    }
}
