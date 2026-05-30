namespace Daxsys.Application.Auth.DTOs;

public class AuthContextDocumentDto
{
    public string DocumentCode { get; set; } = null!;
    public string? Changes { get; set; }
}