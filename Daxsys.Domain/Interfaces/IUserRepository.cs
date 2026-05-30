using Daxsys.Domain.Entities;

namespace Daxsys.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string idUsuario);
}