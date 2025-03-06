using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Interfaces;
using TaskManagementSystem.Infrastructure.Data;

namespace TaskManagementSystem.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDatabaseContext _databaseContext;

        public UserRepository(IDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _databaseContext.GetAllUsersAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _databaseContext.GetUserByIdAsync(id);
        }

        public async Task<User> AddAsync(User user)
        {
            return await _databaseContext.AddUserAsync(user);
        }

        public async Task UpdateAsync(User user)
        {
            await _databaseContext.UpdateUserAsync(user);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _databaseContext.DeleteUserAsync(id);
        }
    }
} 