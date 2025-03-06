using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(Guid id);
        Task<User> AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(Guid id);
    }
} 