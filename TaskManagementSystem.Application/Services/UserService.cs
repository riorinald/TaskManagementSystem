using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Interfaces;

namespace TaskManagementSystem.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToDto);
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user != null ? MapToDto(user) : null;
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            if (string.IsNullOrWhiteSpace(createUserDto.Name))
            {
                throw new ArgumentException("User name is required");
            }

            if (string.IsNullOrWhiteSpace(createUserDto.Email))
            {
                throw new ArgumentException("User email is required");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = createUserDto.Name,
                Email = createUserDto.Email
            };

            var createdUser = await _userRepository.AddAsync(user);
            _logger.LogInformation("User created: {Id}", createdUser.Id);

            return MapToDto(createdUser);
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            await _userRepository.DeleteAsync(id);
            _logger.LogInformation("User deleted: {Id}", id);
            
            return true;
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            };
        }
    }
} 