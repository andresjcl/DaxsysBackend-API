namespace Daxsys.Application.Users.DTOs;

public class CreateUserRequestDto
{
    public string Id { get; set; } = null!;
    public string? Code { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Password { get; set; } = null!;
    public DateTime? PasswordChangeDate { get; set; }
    public int? PasswordDurationDays { get; set; }
}