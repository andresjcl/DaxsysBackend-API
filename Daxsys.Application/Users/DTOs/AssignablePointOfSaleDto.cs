namespace Daxsys.Application.Users.DTOs;

public class AssignablePointOfSaleDto
{
    public int EmpCodigo { get; set; }
    public string BranchId { get; set; } = null!;
    public string PointOfSaleId { get; set; } = null!;
    public string? PointOfSaleName { get; set; }
    public bool HasAccess { get; set; }
}