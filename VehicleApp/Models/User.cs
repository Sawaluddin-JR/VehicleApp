using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace VehicleApp.Models
{
    public enum UserRole
    {
        User,
        Admin
    }

    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        //[JsonIgnore]
        public string Password { get; set; }

        public bool IsAdmin { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public UserRole Role { get; set; }
    }
}
