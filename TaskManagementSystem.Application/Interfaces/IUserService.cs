using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementSystem.Application.DTOs;

namespace TaskManagementSystem.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(Guid id);
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<bool> DeleteUserAsync(Guid id);
    }
} 