namespace Daxsys.Application.Users.DTOs;

public class UpdateUserRequestDto
{
    public string? Code { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? PasswordDurationDays { get; set; }
}