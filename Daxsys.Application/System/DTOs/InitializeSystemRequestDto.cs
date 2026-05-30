namespace Daxsys.Application.System.DTOs;

public class InitializeSystemRequestDto
{
    public string ConfirmationText { get; set; } = null!;

    public int CompanyId { get; set; } = 1;
    public string CompanyName { get; set; } = "EMPRESA PRUEBAS";
    public string? Ruc { get; set; }

    public string BranchCode { get; set; } = "PRI";
    public string BranchName { get; set; } = "PRINCIPAL";

    public string WarehouseCode { get; set; } = "PRI";
    public string WarehouseName { get; set; } = "PRINCIPAL";

    public string AdminUserId { get; set; } = "administrador";
    public string AdminPassword { get; set; } = "1";

    public string TransactionalDatabase { get; set; } = null!;
    public string? SriDatabase { get; set; }
}