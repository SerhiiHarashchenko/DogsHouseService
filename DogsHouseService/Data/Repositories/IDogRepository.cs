using DogsHouseService.Data.Entities;

namespace DogsHouseService.Data.Repositories
{
    public interface IDogRepository
    {
        Task<IEnumerable<Dog>> GetAllSortedAsync(string sortAttribute, bool isAscending);
        Task<IEnumerable<Dog>> GetPaginatedAndSortedAsync(int pageNumber, int pageSize, string sortAttribute, bool isAscending);
        Task<Dog?> GetByNameAsync(string name);
        Task AddAsync(Dog dog);
    }
}
