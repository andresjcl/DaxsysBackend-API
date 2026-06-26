namespace Daxsys.Application.Users.DTOs;

public class AssignableDocumentDto
{
    public int EmpCodigo { get; set; }
    public string DocumentCode { get; set; } = null!;
    public string? DocumentName { get; set; }
    public string? DocumentType { get; set; }
    public bool HasAccess { get; set; }
}