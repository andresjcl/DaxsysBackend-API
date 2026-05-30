namespace Daxsys.Application.Users.DTOs;

public class UserAccessDto
{
    public int CompanyId { get; set; }
    public string? SystemId { get; set; }
    public string? OptionId { get; set; }
    public string? OptionName { get; set; }
    public string? AccessValue { get; set; }
}