namespace Daxsys.Application.Companies.DTOs;

public class PointOfSaleCreateDto
{
    public string Code { get; set; } = null!;
    public string? Name { get; set; }
    public string? TributaryId { get; set; }
    public string? PointType { get; set; }
}