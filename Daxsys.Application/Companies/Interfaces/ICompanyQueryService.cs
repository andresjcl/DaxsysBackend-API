using Daxsys.Application.Companies.DTOs;

namespace Daxsys.Application.Companies.Interfaces;

public interface ICompanyQueryService
{
    Task<List<CompanyListItemDto>> GetCompaniesAsync();
    Task<CompanyDetailDto?> GetCompanyByIdAsync(int companyId);
    Task<List<BranchDto>> GetBranchesAsync(int companyId);
    Task<List<WarehouseDto>> GetWarehousesAsync(int companyId, string branchCode);
    Task<List<PointOfSaleDto>> GetPointsOfSaleAsync(int companyId, string branchCode);
    Task<CompanyParameterDto?> GetParametersAsync(int companyId);
    Task<List<CompanyDatabaseDto>> GetDatabasesAsync(int companyId);
    Task<List<AvailableDocumentDto>> GetAvailableDocumentsAsync(int companyId, string archiveType);
}