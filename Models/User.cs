using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeatherEmergencyAPI.Models
{
    [Table("USERS")]
    public class User
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Required]
        [Column("NAME")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("EMAIL")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("PASSWORD_HASH")]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UPDATED_AT")]
        public DateTime? UpdatedAt { get; set; }

        [Column("IS_ACTIVE")]
        public bool IsActive { get; set; } = true;

        // Relacionamentos
        public virtual ICollection<Location> SavedLocations { get; set; } = new List<Location>();
        public virtual ICollection<EmergencyContact> EmergencyContacts { get; set; } = new List<EmergencyContact>();
    }
}