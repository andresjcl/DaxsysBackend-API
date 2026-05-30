namespace Daxsys.Application.Auth.DTOs;

public class AuthContextPointOfSaleDto
{
    public string BranchCode { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? Name { get; set; }
}