using Daxsys.Domain.Entities;
using Daxsys.Domain.Interfaces;
using Daxsys.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Daxsys.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(string idUsuario)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdUsuario == idUsuario);
    }
}