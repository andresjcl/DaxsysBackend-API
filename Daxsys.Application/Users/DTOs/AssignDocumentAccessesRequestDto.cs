namespace Daxsys.Application.Users.DTOs;

public class AssignDocumentAccessesRequestDto
{
    public int EmpCodigo { get; set; }
    public string DocumentCode { get; set; } = null!;
    public List<DocumentAccessOptionDto> Options { get; set; } = new();
}