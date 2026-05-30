using Daxsys.Application.Companies.DTOs;

namespace Daxsys.Application.Companies.Interfaces;

public interface ICompanyCommandService
{
    Task<CreateCompanyResponseDto> CreateCompanyAsync(CreateCompanyRequestDto request);
    Task<CompanyDetailDto> UpdateCompanyAsync(int companyId, UpdateCompanyRequestDto request);
    Task<BranchDto> CreateBranchAsync(int companyId, CreateBranchRequestDto request);
    Task<WarehouseDto> CreateWarehouseAsync(int companyId, string branchCode, CreateWarehouseRequestDto request);
    Task<PointOfSaleDto> CreatePointOfSaleAsync(int companyId, string branchCode, CreatePointOfSaleRequestDto request);
    Task<List<CompanyDatabaseDto>> UpdateCompanyDatabasesAsync(int companyId, UpdateCompanyDatabasesRequestDto request);

    Task<CompanyParameterDto> UpdateCompanyParametersAsync(int companyId, UpdateCompanyParametersRequestDto request);
    Task DeleteCompanyAsync(int companyId);
}