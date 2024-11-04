using AutoMapper;
using DogsHouseService.Data.Entities;
using DogsHouseService.Data.Repositories;
using DogsHouseService.DTOs;

namespace DogsHouseService.Services
{
    public class DogService : IDogService
    {
        private readonly IDogRepository _dogRepository;
        private readonly IMapper _mapper;

        public DogService(IDogRepository dogRepository, IMapper mapper)
        {
            _dogRepository = dogRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DogDto>> GetAllDogsAsync(string sortAttribute = "name", string order = "asc")
        {
            bool isAscending = ParseOrderParameter(order);
            var dogs = await _dogRepository.GetAllSortedAsync(sortAttribute, isAscending);
            return _mapper.Map<IEnumerable<DogDto>>(dogs);
        }

        public async Task<IEnumerable<DogDto>> GetPaginatedAndSortedDogsAsync(int pageNumber, int pageSize,
            string sortAttribute = "name", string order = "asc")
        {
            bool isAscending = ParseOrderParameter(order);
            var dogs = await _dogRepository.GetPaginatedAndSortedAsync(pageNumber, pageSize, sortAttribute, isAscending);
            return _mapper.Map<IEnumerable<DogDto>>(dogs);
        }

        public async Task<bool> AddDogAsync(CreateDogDto dogDto)
        {
            var existingDog = await _dogRepository.GetByNameAsync(dogDto.Name);

            if (existingDog != null)
            {
                throw new Exception($"A dog with name {dogDto.Name} already exists.");
            }

            var dog = _mapper.Map<Dog>(dogDto);
            await _dogRepository.AddAsync(dog);
            return true;
        }

        private bool ParseOrderParameter(string? order)
        {
            return string.IsNullOrEmpty(order) || order.Equals("asc", StringComparison.OrdinalIgnoreCase);
        }
    }
}
