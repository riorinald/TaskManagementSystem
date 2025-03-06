using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;
using TaskManagementSystem.Domain.Interfaces;

namespace TaskManagementSystem.API.Controllers
{
    /// <summary>
    /// API endpoints for managing work items
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class WorkItemsController : ControllerBase
    {
        private readonly IWorkItemService _workItemService;
        private readonly ILogger<WorkItemsController> _logger;
        private readonly IUserRepository _userRepository;

        public WorkItemsController(IWorkItemService workItemService, ILogger<WorkItemsController> logger, IUserRepository userRepository)
        {
            _workItemService = workItemService;
            _logger = logger;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Gets all work items
        /// </summary>
        /// <returns>A list of all work items</returns>
        /// <response code="200">Returns the list of work items</response>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Gets all work items",
            Description = "Retrieves a list of all work items in the system",
            OperationId = "GetAllWorkItems",
            Tags = new[] { "WorkItems" }
        )]
        [SwaggerResponse(200, "The list of work items was successfully retrieved", typeof(IEnumerable<WorkItemDto>))]
        public async Task<ActionResult<IEnumerable<WorkItemDto>>> GetAllWorkItems()
        {
            var workItems = await _workItemService.GetAllWorkItemsAsync();
            return Ok(workItems);
        }

        /// <summary>
        /// Gets a specific work item by ID
        /// </summary>
        /// <param name="id">The ID of the work item to retrieve</param>
        /// <returns>The requested work item</returns>
        /// <response code="200">Returns the requested work item</response>
        /// <response code="404">If the work item is not found</response>
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Gets a specific work item",
            Description = "Retrieves a specific work item by its unique identifier",
            OperationId = "GetWorkItemById",
            Tags = new[] { "WorkItems" }
        )]
        [SwaggerResponse(200, "The work item was successfully retrieved", typeof(WorkItemDto))]
        [SwaggerResponse(404, "The work item was not found")]
        public async Task<ActionResult<WorkItemDto>> GetWorkItemById(Guid id)
        {
            var workItem = await _workItemService.GetWorkItemByIdAsync(id);
            
            if (workItem == null)
            {
                return NotFound();
            }
            
            return Ok(workItem);
        }

        /// <summary>
        /// Gets all work items assigned to a specific user
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>A list of work items assigned to the user</returns>
        /// <response code="200">Returns the list of work items</response>
        [HttpGet("user/{userId}")]
        [SwaggerOperation(
            Summary = "Gets work items by user",
            Description = "Retrieves all work items assigned to a specific user",
            OperationId = "GetWorkItemsByUserId",
            Tags = new[] { "WorkItems" }
        )]
        [SwaggerResponse(200, "The list of work items was successfully retrieved", typeof(IEnumerable<WorkItemDto>))]
        public async Task<ActionResult<IEnumerable<WorkItemDto>>> GetWorkItemsByUserId(Guid userId)
        {
            var workItems = await _workItemService.GetWorkItemsByUserIdAsync(userId);
            return Ok(workItems);
        }

        /// <summary>
        /// Creates a new work item
        /// </summary>
        /// <param name="createWorkItemDto">The work item data</param>
        /// <returns>The newly created work item</returns>
        /// <response code="201">Returns the newly created work item</response>
        /// <response code="400">If the work item data is invalid</response>
        [HttpPost]
        [SwaggerOperation(
            Summary = "Creates a new work item",
            Description = "Creates a new work item with the provided data",
            OperationId = "CreateWorkItem",
            Tags = new[] { "WorkItems" }
        )]
        [SwaggerResponse(201, "The work item was successfully created", typeof(WorkItemDto))]
        [SwaggerResponse(400, "The work item data is invalid")]
        public async Task<ActionResult<WorkItemDto>> CreateWorkItem(CreateWorkItemDto createWorkItemDto)
        {
            try
            {
                var workItem = await _workItemService.CreateWorkItemAsync(createWorkItemDto);
                return CreatedAtAction(nameof(GetWorkItemById), new { id = workItem.Id }, workItem);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing work item
        /// </summary>
        /// <param name="id">The ID of the work item to update</param>
        /// <param name="updateWorkItemDto">The updated work item data</param>
        /// <returns>The updated work item</returns>
        /// <response code="200">Returns the updated work item</response>
        /// <response code="400">If the work item data is invalid</response>
        /// <response code="404">If the work item is not found</response>
        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Updates a work item",
            Description = "Updates an existing work item with the provided data",
            OperationId = "UpdateWorkItem",
            Tags = new[] { "WorkItems" }
        )]
        [SwaggerResponse(200, "The work item was successfully updated", typeof(WorkItemDto))]
        [SwaggerResponse(400, "The work item data is invalid")]
        [SwaggerResponse(404, "The work item was not found")]
        public async Task<ActionResult<WorkItemDto>> UpdateWorkItem(Guid id, UpdateWorkItemDto updateWorkItemDto)
        {
            try
            {
                var workItem = await _workItemService.UpdateWorkItemAsync(id, updateWorkItemDto);
                
                if (workItem == null)
                {
                    return NotFound();
                }
                
                return Ok(workItem);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a specific work item
        /// </summary>
        /// <param name="id">The ID of the work item to delete</param>
        /// <returns>No content</returns>
        /// <response code="204">If the work item was successfully deleted</response>
        /// <response code="404">If the work item is not found</response>
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Deletes a work item",
            Description = "Deletes a specific work item by its unique identifier",
            OperationId = "DeleteWorkItem",
            Tags = new[] { "WorkItems" }
        )]
        [SwaggerResponse(204, "The work item was successfully deleted")]
        [SwaggerResponse(404, "The work item was not found")]
        public async Task<ActionResult> DeleteWorkItem(Guid id)
        {
            var result = await _workItemService.DeleteWorkItemAsync(id);
            
            if (!result)
            {
                return NotFound();
            }
            
            return NoContent();
        }

        /// <summary>
        /// Gets all available users
        /// </summary>
        /// <returns>A list of all users</returns>
        /// <response code="200">Returns the list of users</response>
        [HttpGet("users")]
        [SwaggerOperation(
            Summary = "Gets all available users",
            Description = "Retrieves a list of all available users in the system",
            OperationId = "GetAllUsers",
            Tags = new[] { "Users" }
        )]
        [SwaggerResponse(200, "The list of users was successfully retrieved", typeof(IEnumerable<UserDto>))]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _userRepository.GetAllAsync();
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email
            });
            return Ok(userDtos);
        }
    }
} 