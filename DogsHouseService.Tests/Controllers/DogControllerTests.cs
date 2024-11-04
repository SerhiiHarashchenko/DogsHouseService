using DogsHouseService.Controllers;
using DogsHouseService.DTOs;
using DogsHouseService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using FluentAssertions;

namespace DogsHouseService.Tests.Controllers
{
    public class DogControllerTests
    {
        private readonly Mock<IDogService> _mockDogService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly DogController _controller;

        public DogControllerTests()
        {
            _mockDogService = new Mock<IDogService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _controller = new DogController(_mockDogService.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task Ping_ShouldReturnVersion()
        {
            string expectedVersion = "DogsHouseService.Version1.0.1";
            _mockConfiguration.Setup(config => config["Version"]).Returns(expectedVersion);

            var result = _controller.Ping() as OkObjectResult;

            result.Should().NotBeNull();
            result!.Value.Should().Be(expectedVersion);
        }

        [Fact]
        public async Task GetSomeDogs_ShouldReturnAllDogs_WhenDogsExist()
        {
            var dogs = new List<DogDto>
            {
                new DogDto { Name = "Buddy", Color = "Brown", TailLength = 10, Weight = 20 },
                new DogDto { Name = "Max", Color = "Black", TailLength = 15, Weight = 25 },
                new DogDto { Name = "Bella", Color = "Golden", TailLength = 12, Weight = 22 },
                new DogDto { Name = "Luna", Color = "White", TailLength = 14, Weight = 23 }
            };
            _mockDogService.Setup(service => service.GetAllDogsAsync("name", "asc")).ReturnsAsync(dogs);

            var result = await _controller.GetSomeDogs() as OkObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
            result.Value.Should().BeEquivalentTo(dogs);
        }

        [Fact]
        public async Task GetSomeDogs_ShouldReturnEmptyList_WhenNoDogsExist()
        {
            _mockDogService
                .Setup(service => service.GetAllDogsAsync("name", "asc")).ReturnsAsync(new List<DogDto>());

            var result = await _controller.GetSomeDogs() as OkObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
            ((IEnumerable<DogDto>)result.Value).Should().BeEmpty();
        }

        [Fact]
        public async Task GetSomeDogs_ShouldReturnBadRequest_WhenInvalidAttribute()
        {
            var result = await _controller.GetSomeDogs(attribute: "invalid") as BadRequestObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(400);
            result.Value.Should().Be("Invalid attribute value. Supported values are 'name', 'weight', 'color', and 'tailLength'.");
        }

        [Fact]
        public async Task GetSomeDogs_ShouldReturnBadRequest_WhenInvalidOrderValue()
        {
            var result = await _controller.GetSomeDogs(order: "descending") as BadRequestObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(400);
            result.Value.Should().Be("Invalid order value. Supported values are 'asc' and 'desc'.");
        }

        [Fact]
        public async Task CreateDogAsync_ShouldReturnSuccess_WhenDogIsValid()
        {
            var newDog = new CreateDogDto { Name = "Buddy", Color = "Brown", TailLength = 10, Weight = 20 };
            _mockDogService.Setup(service => service.AddDogAsync(newDog)).ReturnsAsync(true);

            var result = await _controller.CreateDogAsync(newDog) as OkObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
            result.Value.Should().Be("Dog Buddy added successfully");
        }

        [Fact]
        public async Task CreateDogAsync_ShouldReturnConflict_WhenDogWithNameExists()
        {
            var newDog = new CreateDogDto { Name = "Buddy", Color = "Brown", TailLength = 10, Weight = 20 };
            _mockDogService.Setup(service => service.AddDogAsync(newDog)).ThrowsAsync(new Exception("A dog with this name already exists."));

            var result = await _controller.CreateDogAsync(newDog) as ConflictObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(409);
            result.Value.Should().Be("A dog with this name already exists.");
        }
    }
}
