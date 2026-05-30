namespace Daxsys.Application.Users.DTOs;

public class AssignDocumentsRequestDto
{
    public int CompanyId { get; set; }
    public bool GenerateDefaultAccesses { get; set; } = true;
    public List<DocumentPermissionItemDto> Documents { get; set; } = new();
}

public class DocumentPermissionItemDto
{
    public string? DocumentCode { get; set; }
    public string? Changes { get; set; }
}