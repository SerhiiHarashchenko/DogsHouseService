using DogsHouseService.Data.Entities;
using DogsHouseService.Data.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogsHouseService.Tests.Repositories
{
    public class DogRepositoryTests
    {
        private readonly DogsHouseServiceDbContext _dbContext;
        private readonly DogRepository _dogRepository;

        public DogRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DogsHouseServiceDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new DogsHouseServiceDbContext(options);
            _dogRepository = new DogRepository(_dbContext);

            SeedDatabase();
        }

        [Fact]
        public async Task GetAllSortedAsync_ShouldReturnDogsSortedByNameAscending()
        {
            var result = await _dogRepository.GetAllSortedAsync("name", true);

            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Select(d => d.Name).Should().ContainInOrder("Bella", "Buddy", "Charlie");
        }

        [Fact]
        public async Task GetAllSortedAsync_ShouldReturnDogsSortedByWeightDescending()
        {
            var result = await _dogRepository.GetAllSortedAsync("weight", false);

            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Select(d => d.Weight).Should().ContainInOrder(25, 20, 18);
        }

        [Fact]
        public async Task GetAllSortedAsync_ShouldReturnEmptyListIfNoDogsExist()
        {
            _dbContext.Dogs.RemoveRange(_dbContext.Dogs);
            await _dbContext.SaveChangesAsync();

            var result = await _dogRepository.GetAllSortedAsync("name", true);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetPaginatedAndSortedAsync_ShouldReturnPaginatedResults()
        {
            var result = await _dogRepository.GetPaginatedAndSortedAsync(pageNumber: 1, pageSize: 2,
                sortAttribute: "name", isAscending: true);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Select(d => d.Name).Should().ContainInOrder("Bella", "Buddy");
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnCorrectDog()
        {
            var result = await _dogRepository.GetByNameAsync("Charlie");

            result.Should().NotBeNull();
            result.Name.Should().Be("Charlie");
            result.Color.Should().Be("Black");
            result.TailLength.Should().Be(5);
            result.Weight.Should().Be(20);
        }

        [Fact]
        public async Task AddAsync_ShouldAddNewDogToDatabase()
        {
            var newDog = new Dog { Name = "Max", Color = "Gray", TailLength = 7, Weight = 30 };

            await _dogRepository.AddAsync(newDog);
            var result = await _dbContext.Dogs.FirstOrDefaultAsync(d => d.Name == "Max");

            result.Should().NotBeNull();
            result.Name.Should().Be("Max");
            result.Color.Should().Be("Gray");
            result.TailLength.Should().Be(7);
            result.Weight.Should().Be(30);
        }
        private void SeedDatabase()
        {
            _dbContext.Dogs.RemoveRange(_dbContext.Dogs);
            _dbContext.SaveChanges();

            var dogs = new List<Dog>
            {
                new Dog { Name = "Charlie", Color = "Black", TailLength = 5, Weight = 20 },
                new Dog { Name = "Bella", Color = "Golden", TailLength = 4, Weight = 18 },
                new Dog { Name = "Buddy", Color = "Brown", TailLength = 6, Weight = 25 }
            };

            _dbContext.Dogs.AddRange(dogs);
            _dbContext.SaveChanges();
        }
    }
}

