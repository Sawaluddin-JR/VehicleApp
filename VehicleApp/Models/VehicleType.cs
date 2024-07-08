using System.ComponentModel.DataAnnotations;

namespace VehicleApp.Models
{
    public class VehicleType
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public int BrandId { get; set; }
        public VehicleBrand? Brand { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }
    }
}
