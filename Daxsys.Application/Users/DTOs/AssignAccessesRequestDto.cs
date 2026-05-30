namespace Daxsys.Application.Users.DTOs;

public class AssignAccessesRequestDto
{
    public int CompanyId { get; set; }
    public string? SystemId { get; set; }
    public List<AccessItemDto> Accesses { get; set; } = new();
}

public class AccessItemDto
{
    public string? OptionId { get; set; }
    public string? OptionName { get; set; }
    public string? AccessValue { get; set; }
}