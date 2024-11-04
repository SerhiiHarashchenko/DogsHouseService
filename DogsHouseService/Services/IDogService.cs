using DogsHouseService.DTOs;

namespace DogsHouseService.Services
{
    public interface IDogService
    {
        Task<IEnumerable<DogDto>> GetPaginatedAndSortedDogsAsync(int pageNumber, int pageSize, string sortAttribute, string order);
        Task<IEnumerable<DogDto>> GetAllDogsAsync(string sortAttribute, string order);
        Task<bool> AddDogAsync(CreateDogDto dogDto);
    }
}
