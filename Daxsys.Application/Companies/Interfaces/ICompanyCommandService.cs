using Daxsys.Application.Companies.DTOs;

namespace Daxsys.Application.Companies.Interfaces;

public interface ICompanyCommandService
{
    Task<CreateCompanyResponseDto> CreateCompanyAsync(CreateCompanyRequestDto request);
    Task<CompanyDetailDto> UpdateCompanyAsync(int EmpCodigo, UpdateCompanyRequestDto request);
    Task<BranchDto> CreateBranchAsync(int EmpCodigo, CreateBranchRequestDto request);
    Task<WarehouseDto> CreateWarehouseAsync(int EmpCodigo, string branchCode, CreateWarehouseRequestDto request);
    Task<PointOfSaleDto> CreatePointOfSaleAsync(int EmpCodigo, string branchCode, CreatePointOfSaleRequestDto request);
    Task<List<CompanyDatabaseDto>> UpdateCompanyDatabasesAsync(int EmpCodigo, UpdateCompanyDatabasesRequestDto request);

    Task<CompanyParameterDto> UpdateCompanyParametersAsync(int EmpCodigo, UpdateCompanyParametersRequestDto request);
    Task DeleteCompanyAsync(int EmpCodigo);
}