namespace Daxsys.Application.Companies.DTOs;

public class AvailableDocumentDto
{
    public string DocumentCode { get; set; } = null!;
    public string? DocumentName { get; set; }
    public string? DocumentType { get; set; }
}