using AutoMapper;
using DogsHouseService.Data.Entities;
using DogsHouseService.Data.Repositories;
using DogsHouseService.DTOs;
using DogsHouseService.Services;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogsHouseService.Tests.Services
{
    public class DogServiceTests
    {
        private readonly DogService _dogService;
        private readonly Mock<IDogRepository> _dogRepositoryMock;
        private readonly IMapper _mapper;

        public DogServiceTests()
        {
            _dogRepositoryMock = new Mock<IDogRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateDogDto, Dog>();
                cfg.CreateMap<Dog, DogDto>().ReverseMap();
            });
            _mapper = config.CreateMapper();

            _dogService = new DogService(_dogRepositoryMock.Object, _mapper);
        }

        [Fact]
        public async Task GetAllDogsAsync_ShouldReturnAllDogsSortedByNameAscending()
        {
            var dogs = GetSampleDogs();
            _dogRepositoryMock
                .Setup(repo => repo.GetAllSortedAsync("name", true)).ReturnsAsync(dogs.OrderBy(d => d.Name).ToList());

            var result = await _dogService.GetAllDogsAsync("name", "asc");

            result.Should().NotBeNull();
            result.Should().HaveCount(4);
            result.Select(d => d.Name).Should().BeInAscendingOrder();
        }

        [Fact]
        public async Task GetAllDogsAsync_ShouldReturnEmptyList_WhenNoDogsAvailable()
        {
            _dogRepositoryMock
                .Setup(repo => repo.GetAllSortedAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new List<Dog>());

            var result = await _dogService.GetAllDogsAsync();

            result.Should().BeEmpty();
        }

        [Theory]
        [InlineData("weight", "asc")]
        [InlineData("color", "desc")]
        [InlineData("tailLength", "asc")]
        public async Task GetAllDogsAsync_ShouldHandleDifferentSortingAttributes(string attribute, string order)
        {
            var dogs = GetSampleDogs();
            var isAscending = order == "asc";
            _dogRepositoryMock
                .Setup(repo => repo.GetAllSortedAsync(attribute, isAscending)).ReturnsAsync(dogs);

            var result = await _dogService.GetAllDogsAsync(attribute, order);

            result.Should().NotBeNull();
            result.Should().HaveCount(4);
            _dogRepositoryMock.Verify(repo => repo.GetAllSortedAsync(attribute, isAscending), Times.Once);
        }

        [Fact]
        public async Task AddDogAsync_ShouldAddNewDog_WhenDogDoesNotExist()
        {
            var dogDto = new CreateDogDto { Name = "Rocky", Color = "Gray", TailLength = 9, Weight = 25 };
            _dogRepositoryMock.Setup(repo => repo.GetByNameAsync(dogDto.Name)).ReturnsAsync((Dog)null);

            Func<Task> act = async () => await _dogService.AddDogAsync(dogDto);

            await act.Should().NotThrowAsync();
            _dogRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Dog>()), Times.Once);
        }

        [Fact]
        public async Task AddDogAsync_ShouldThrowException_WhenDogWithSameNameExists()
        {
            var dogDto = new CreateDogDto { Name = "Buddy", Color = "Brown", TailLength = 12, Weight = 30 };
            var existingDog = new Dog { Name = "Buddy", Color = "Browny", TailLength = 121, Weight = 301 };
            _dogRepositoryMock.Setup(repo => repo.GetByNameAsync(dogDto.Name)).ReturnsAsync(existingDog);

            Func<Task> act = async () => await _dogService.AddDogAsync(dogDto);

            await act.Should().ThrowAsync<Exception>().WithMessage("A dog with name Buddy already exists.");
            _dogRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Dog>()), Times.Never);
        }

        [Fact]
        public async Task GetPaginatedAndSortedDogsAsync_ShouldReturnPaginatedAndSortedDogs()
        {
            var dogs = GetSampleDogs();
            _dogRepositoryMock
                .Setup(repo => repo.GetPaginatedAndSortedAsync(1, 2, "name", true))
                .ReturnsAsync(dogs.OrderBy(d => d.Name).Take(2).ToList());

            var result = await _dogService.GetPaginatedAndSortedDogsAsync(1, 2, "name", "asc");

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Select(d => d.Name).Should().BeInAscendingOrder();
        }

        [Fact]
        public async Task GetPaginatedAndSortedDogsAsync_ShouldReturnEmptyListWhenPageNumberIsOutOfRange()
        {
            var dogs = GetSampleDogs();
            _dogRepositoryMock
                .Setup(repo => repo.GetPaginatedAndSortedAsync(999, 10, "name", true)).ReturnsAsync(new List<Dog>());

            var result = await _dogService.GetPaginatedAndSortedDogsAsync(999, 10, "name", "asc");

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetPaginatedAndSortedDogsAsync_ShouldReturnAllDogsIfPageSizeExceedsTotalCount()
        {
            var dogs = GetSampleDogs();
            _dogRepositoryMock
                .Setup(repo => repo.GetPaginatedAndSortedAsync(1, 100, "name", true)).ReturnsAsync(dogs);

            var result = await _dogService.GetPaginatedAndSortedDogsAsync(1, 100, "name", "asc");

            result.Should().HaveCount(dogs.Count);
        }

        private List<Dog> GetSampleDogs()
        {
            return new List<Dog>
            {
                new Dog { Name = "Buddy", Color = "Brown", TailLength = 5, Weight = 20 },
                new Dog { Name = "Charlie", Color = "Black", TailLength = 7, Weight = 25 },
                new Dog { Name = "Max", Color = "White", TailLength = 6, Weight = 30 },
                new Dog { Name = "Bella", Color = "Golden", TailLength = 4, Weight = 18 }
            };
        }
    }
}
