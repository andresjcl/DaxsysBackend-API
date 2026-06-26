namespace Daxsys.Application.Users.DTOs;

public class AssignPointsOfSaleRequestDto
{
    public int EmpCodigo { get; set; }
    public string BranchId { get; set; } = null!;
    public List<PointOfSalePermissionItemDto> PointsOfSale { get; set; } = new();
}

public class PointOfSalePermissionItemDto
{
    public string PointOfSaleId { get; set; } = null!;
    public bool Authorized { get; set; }
}