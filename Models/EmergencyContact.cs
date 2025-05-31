using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeatherEmergencyAPI.Models
{
    [Table("EMERGENCY_CONTACTS")]
    public class EmergencyContact
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Required]
        [Column("USER_ID")]
        public int UserId { get; set; }

        [Required]
        [Column("NAME")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("PHONE_NUMBER")]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [Column("COUNTRY_CODE")]
        [StringLength(5)]
        public string CountryCode { get; set; } = string.Empty;

        [Column("RELATIONSHIP")]
        [StringLength(50)]
        public string? Relationship { get; set; }

        [Column("IS_PRIMARY")]
        public bool IsPrimary { get; set; } = false;

        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UPDATED_AT")]
        public DateTime? UpdatedAt { get; set; }

        // Relacionamento
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}