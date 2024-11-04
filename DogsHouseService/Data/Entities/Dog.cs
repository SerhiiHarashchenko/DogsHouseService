using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogsHouseService.Data.Entities
{
    [Table("dog_table")]
    public class Dog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Color { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int TailLength { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Weight { get; set; }
    }
}

