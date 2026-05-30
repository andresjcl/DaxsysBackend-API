namespace Daxsys.Application.Auth.DTOs;

public class MeResponseDto
{
    public string Id { get; set; } = null!;
    public bool IsAdmin { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaCaduca { get; set; }
}