using Daxsys.Application.Menus.DTOs;
using Daxsys.Domain.Entities;
using Daxsys.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Daxsys.Infrastructure.Services;

public class MenuService
{
    private readonly AppDbContext _context;

    public MenuService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MenuTreeDto>> GetMenuTreeAsync(string userId, int companyId, string systemId)
    {
        var menus = await _context.Menus
            .AsNoTracking()
            .Where(x => x.IdSistema == systemId && x.Activo)
            .OrderBy(x => x.Orden)
            .ToListAsync();

        var userAccess = await _context.UserAccesses
            .AsNoTracking()
            .Where(x => x.IdUsuario == userId
                     && x.IdEmpresa == companyId
                     && x.IdSistema == systemId
                     && x.Accesos == "T")
            .Select(x => x.IdOpcion)
            .ToListAsync();

        List<MenuTreeDto> BuildTree(int? parentId)
        {
            return menus
                .Where(x => x.IdPadre == parentId)
                .OrderBy(x => x.Orden)
                .Select(x => new MenuTreeDto
                {
                    IdMenu = x.IdMenu,
                    Codigo = x.Codigo,
                    Nombre = x.Nombre,
                    HasAccess = userAccess.Contains(x.Codigo),
                    Children = BuildTree(x.IdMenu)
                })
                .ToList();
        }

        return BuildTree(null);
    }

    public async Task AssignMenuPermissionsAsync(string userId, AssignMenuPermissionsRequestDto request)
    {
        var menus = await _context.Menus
            .AsNoTracking()
            .Where(x => request.MenuIds.Contains(x.IdMenu)
                     && x.IdSistema == request.SystemId
                     && x.Activo)
            .ToListAsync();

        var current = await _context.UserAccesses
            .Where(x => x.IdUsuario == userId
                     && x.IdEmpresa == request.CompanyId
                     && x.IdSistema == request.SystemId)
            .ToListAsync();

        _context.UserAccesses.RemoveRange(current);
        await _context.SaveChangesAsync();

        var newAccess = menus.Select(m => new UserAccess
        {
            IdUsuario = userId,
            IdEmpresa = request.CompanyId,
            IdSistema = request.SystemId,
            IdOpcion = m.Codigo,
            IdNomOpcion = m.Nombre,
            Accesos = "T"
        }).ToList();

        _context.UserAccesses.AddRange(newAccess);
        await _context.SaveChangesAsync();
    }
}