using Daxsys.Application.Users.DTOs;

namespace Daxsys.Application.Users.Interfaces;

public interface IUserManagementService
{
    Task<List<UserListItemDto>> GetUsersAsync();
    Task<UserDetailDto?> GetUserByIdAsync(string userId);
    Task<UserDetailDto> CreateUserAsync(CreateUserRequestDto request);
    Task<UserDetailDto> UpdateUserAsync(string userId, UpdateUserRequestDto request);
    Task<UserDetailDto> ChangePasswordAsync(string userId, ChangePasswordRequestDto request);

    Task AssignBranchesAsync(string userId, AssignBranchesRequestDto request);
    Task AssignWarehousesAsync(string userId, AssignWarehousesRequestDto request);
    Task AssignPointsOfSaleAsync(string userId, AssignPointsOfSaleRequestDto request);
    Task<UserPermissionContextDto?> GetPermissionContextAsync(string userId, int companyId);
    Task AssignAccessesAsync(string userId, AssignAccessesRequestDto request);
    Task<List<UserAccessDto>> GetAccessesAsync(string userId, int companyId);

    Task AssignDocumentsAsync(string userId, AssignDocumentsRequestDto request);
    Task<List<UserDocumentDto>> GetDocumentsAsync(string userId, int companyId);

    Task AssignDocumentAccessesAsync(string userId, AssignDocumentAccessesRequestDto request);
    Task<List<UserDocumentAccessDto>> GetDocumentAccessesAsync(string userId, int companyId, string documentCode);
    Task<List<AssignableBranchDto>> GetAssignableBranchesAsync(string userId, int companyId);
    Task<List<AssignableWarehouseDto>> GetAssignableWarehousesAsync(string userId, int companyId, string branchId);
    Task<List<AssignablePointOfSaleDto>> GetAssignablePointsOfSaleAsync(string userId, int companyId, string branchId);

    Task<List<AssignableDocumentDto>> GetAssignableDocumentsAsync(string userId, int companyId, string archiveType);
}