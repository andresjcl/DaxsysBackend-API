using Daxsys.Application.Companies.DTOs;

namespace Daxsys.Application.Companies.Interfaces;

public interface ICompanyQueryService
{
    Task<List<CompanyListItemDto>> GetCompaniesAsync();
    Task<CompanyDetailDto?> GetCompanyByIdAsync(int EmpCodigo);
    Task<List<BranchDto>> GetBranchesAsync(int EmpCodigo);
    Task<List<WarehouseDto>> GetWarehousesAsync(int EmpCodigo, string branchCode);
    Task<List<PointOfSaleDto>> GetPointsOfSaleAsync(int EmpCodigo, string branchCode);
    Task<CompanyParameterDto?> GetParametersAsync(int EmpCodigo);
    Task<List<CompanyDatabaseDto>> GetDatabasesAsync(int EmpCodigo);
    Task<List<AvailableDocumentDto>> GetAvailableDocumentsAsync(int EmpCodigo, string archiveType);
}