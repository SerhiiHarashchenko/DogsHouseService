using System.ComponentModel.DataAnnotations;

namespace DogsHouseService.DTOs
{
    public class CreateDogDto
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Required]
        [StringLength(100)]
        public required string Color { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Tail length must be a non-negative number.")]
        public required int TailLength { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Weight must be a non-negative number.")]
        public required int Weight { get; set; }
    }
}
