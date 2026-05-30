namespace Daxsys.Application.Users.DTOs;

public class UserListItemDto
{
    public string Id { get; set; } = null!;
    public string? Code { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? PasswordChangeDate { get; set; }
    public int? PasswordDurationDays { get; set; }
    public bool IsAdmin { get; set; }
}