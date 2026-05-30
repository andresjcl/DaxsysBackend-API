namespace Daxsys.Application.Auth.DTOs;

public class AuthContextWarehouseDto
{
    public string BranchCode { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? Name { get; set; }
}