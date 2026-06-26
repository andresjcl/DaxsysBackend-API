using Daxsys.Domain.Entities;

namespace Daxsys.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user, bool isAdmin, DateTime expiresAt);

    string GenerateContextToken(
    string userId,
    bool isAdmin,
    int EmpCodigo,
    string branchId,
    string? warehouseId,
    string? pointOfSaleId,
    DateTime expiresAt);
}