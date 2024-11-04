using DogsHouseService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DogsHouseService.Data.Repositories
{
    public class DogRepository : IDogRepository
    {
        private readonly DogsHouseServiceDbContext _dbContext;

        public DogRepository(DogsHouseServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Dog>> GetAllSortedAsync(string sortAttribute, bool isAscending)
        {
            return await ApplySorting(_dbContext.Dogs, sortAttribute, isAscending).ToListAsync();
        }

        public async Task<IEnumerable<Dog>> GetPaginatedAndSortedAsync(int pageNumber, int pageSize, string sortAttribute, bool isAscending)
        {
            return await ApplySorting(_dbContext.Dogs, sortAttribute, isAscending)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Dog?> GetByNameAsync(string name)
        {
            return await _dbContext.Dogs.FirstOrDefaultAsync(d => d.Name == name);
        }

        public async Task AddAsync(Dog dog)
        {
            await _dbContext.Dogs.AddAsync(dog);
            await _dbContext.SaveChangesAsync();
        }
        private IQueryable<Dog> ApplySorting(IQueryable<Dog> query, string sortAttribute, bool isAscending)
        {
            return sortAttribute switch
            {
                "weight" => isAscending ? query.OrderBy(d => d.Weight) : query.OrderByDescending(d => d.Weight),
                "tailLength" => isAscending ? query.OrderBy(d => d.TailLength) : query.OrderByDescending(d => d.TailLength),
                "color" => isAscending ? query.OrderBy(d => d.Color) : query.OrderByDescending(d => d.Color),
                _ => isAscending ? query.OrderBy(d => d.Name) : query.OrderByDescending(d => d.Name)
            };
        }
    }
}
