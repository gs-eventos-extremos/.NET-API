using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeatherEmergencyAPI.Models
{
    [Table("LOCATIONS")]
    public class Location
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
        [Column("LATITUDE")]
        public double Latitude { get; set; }

        [Required]
        [Column("LONGITUDE")]
        public double Longitude { get; set; }

        [Column("CITY")]
        [StringLength(100)]
        public string? City { get; set; }

        [Column("STATE")]
        [StringLength(100)]
        public string? State { get; set; }

        [Column("COUNTRY")]
        [StringLength(100)]
        public string? Country { get; set; }

        [Column("IS_FAVORITE")]
        public bool IsFavorite { get; set; } = false;

        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UPDATED_AT")]
        public DateTime? UpdatedAt { get; set; }

        // Relacionamento
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}