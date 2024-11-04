using DogsHouseService.Data.Entities;
using DogsHouseService.DTOs;
using DogsHouseService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DogsHouseService.Controllers
{
    [ApiController]
    [Route("")]
    [EnableRateLimiting("fixed")]
    public class DogController : ControllerBase
    {
        private readonly IDogService _dogService;
        private readonly IConfiguration _configuration;

        public DogController(IDogService dogService, IConfiguration configuration)
        {
            _dogService = dogService;
            _configuration = configuration;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            var version = _configuration["Version"];
            return Ok(version);
        }

        [HttpGet("dogs")]
        public async Task<IActionResult> GetSomeDogs(
            [FromQuery] string? attribute = "name",
            [FromQuery] string? order = "asc",
            [FromQuery] int? pageNumber = null,
            [FromQuery] int? pageSize = null)
        {
            try
            {
                if (!IsValidAttribute(attribute)) return BadRequest("Invalid attribute value. Supported values are 'name', 'weight', 'color', and 'tailLength'.");
                if (!IsValidOrder(order)) return BadRequest("Invalid order value. Supported values are 'asc' and 'desc'.");

                var dogs = pageNumber.HasValue && pageSize.HasValue
                    ? await _dogService.GetPaginatedAndSortedDogsAsync(pageNumber.Value, pageSize.Value, attribute, order)
                    : await _dogService.GetAllDogsAsync(attribute, order);

                return Ok(dogs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("dog")]
        public async Task<IActionResult> CreateDogAsync([FromBody] CreateDogDto createDogDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _dogService.AddDogAsync(createDogDto);
                return Ok($"Dog {createDogDto.Name} added successfully");
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        private bool IsValidAttribute(string? attribute)
        {
            return new[] { "name", "weight", "color", "taillength" }.Contains(attribute?.ToLower());
        }


        private bool IsValidOrder(string? order)
        {
            return new[] { "asc", "desc" }.Contains(order?.ToLower());
        }
    }
}
