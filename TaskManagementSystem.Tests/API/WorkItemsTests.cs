using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementSystem.API.Controllers;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;
using TaskManagementSystem.Domain.Interfaces;
using Xunit;

namespace TaskManagementSystem.Tests.API
{
    public class WorkItemsTests
    {
        private readonly Mock<IWorkItemService> _mockWorkItemService;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ILogger<WorkItemsController>> _mockLogger;
        private readonly WorkItemsController _controller;

        public WorkItemsTests()
        {
            _mockWorkItemService = new Mock<IWorkItemService>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<WorkItemsController>>();
            
            _controller = new WorkItemsController(
                _mockWorkItemService.Object,
                _mockLogger.Object,
                _mockUserRepository.Object);
        }

        [Fact]
        public async Task GetAllWorkItems_ShouldReturnAllWorkItems()
        {
            // Arrange
            var workItems = new List<WorkItemDto>
            {
                new WorkItemDto { Id = Guid.NewGuid(), Title = "Test Item 1" },
                new WorkItemDto { Id = Guid.NewGuid(), Title = "Test Item 2" }
            };
            
            _mockWorkItemService.Setup(service => service.GetAllWorkItemsAsync())
                .ReturnsAsync(workItems);

            // Act
            var result = await _controller.GetAllWorkItems();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedWorkItems = Assert.IsAssignableFrom<IEnumerable<WorkItemDto>>(okResult.Value);
            Assert.Equal(2, returnedWorkItems.Count());
        }

        [Fact]
        public async Task GetWorkItemById_WithValidId_ShouldReturnWorkItem()
        {
            // Arrange
            var workItemId = Guid.NewGuid();
            var workItem = new WorkItemDto 
            { 
                Id = workItemId, 
                Title = "Test Item" 
            };
            
            _mockWorkItemService.Setup(service => service.GetWorkItemByIdAsync(workItemId))
                .ReturnsAsync(workItem);

            // Act
            var result = await _controller.GetWorkItemById(workItemId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedWorkItem = Assert.IsType<WorkItemDto>(okResult.Value);
            Assert.Equal(workItemId, returnedWorkItem.Id);
        }

        [Fact]
        public async Task GetWorkItemById_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var workItemId = Guid.NewGuid();
            
            _mockWorkItemService.Setup(service => service.GetWorkItemByIdAsync(workItemId))
                .ReturnsAsync((WorkItemDto)null);

            // Act
            var result = await _controller.GetWorkItemById(workItemId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateWorkItem_WithValidData_ShouldReturnCreatedWorkItem()
        {
            // Arrange
            var createDto = new CreateWorkItemDto
            {
                Title = "New Work Item",
                Description = "Description for the new work item",
                DueDate = DateTime.Now.AddDays(7),
                Priority = Priority.Medium
            };
            
            var createdWorkItem = new WorkItemDto
            {
                Id = Guid.NewGuid(),
                Title = createDto.Title,
                Description = createDto.Description,
                DueDate = createDto.DueDate,
                Priority = createDto.Priority,
                Status = Status.ToDo,
                CreatedAt = DateTime.UtcNow
            };
            
            _mockWorkItemService.Setup(service => service.CreateWorkItemAsync(createDto))
                .ReturnsAsync(createdWorkItem);

            // Act
            var result = await _controller.CreateWorkItem(createDto);

            // Assert
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(201, createdAtResult.StatusCode);
            
            var returnedWorkItem = Assert.IsType<WorkItemDto>(createdAtResult.Value);
            Assert.Equal(createdWorkItem.Id, returnedWorkItem.Id);
            Assert.Equal(createDto.Title, returnedWorkItem.Title);
            Assert.Equal(createDto.Description, returnedWorkItem.Description);
            
            _mockWorkItemService.Verify(service => service.CreateWorkItemAsync(createDto), Times.Once);
        }

        [Fact]
        public async Task CreateWorkItem_WithInvalidData_ShouldReturnBadRequest()
        {
            // Arrange
            var createDto = new CreateWorkItemDto
            {
                // Missing required fields
                Title = "",
                DueDate = DateTime.Now.AddDays(-1) // Past date
            };
            
            _mockWorkItemService.Setup(service => service.CreateWorkItemAsync(createDto))
                .ThrowsAsync(new ArgumentException("Invalid work item data"));

            // Act
            var result = await _controller.CreateWorkItem(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Invalid work item data", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteWorkItem_WithValidId_ShouldReturnNoContent()
        {
            // Arrange
            var workItemId = Guid.NewGuid();

            _mockWorkItemService.Setup(service => service.DeleteWorkItemAsync(workItemId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteWorkItem(workItemId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);

            _mockWorkItemService.Verify(service => service.DeleteWorkItemAsync(workItemId), Times.Once);
        }

    }
} 