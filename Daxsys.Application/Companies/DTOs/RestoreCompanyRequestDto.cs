namespace Daxsys.Application.Companies.DTOs;

public class RestoreCompanyRequestDto
{
    // ==================== DATOS OBLIGATORIOS ====================
    public string NewCompanyName { get; set; } = null!;
    public string NewRuc { get; set; } = null!;
    public string NewInitials { get; set; } = null!; // Ej: "DGC"
    public int SourceCompanyId { get; set; } = 1; // Empresa base (Emp_Defecto = 1)

    // ==================== DATOS DE SUCURSAL ====================
    public string BranchCode { get; set; } = null!;
    public string BranchName { get; set; } = null!;

    // ==================== DATOS DE BODEGA ====================
    public string WarehouseCode { get; set; } = null!;
    public string WarehouseName { get; set; } = null!;

    // ==================== RUTA DE BACKUP ====================
    public string BackupPath { get; set; } = null!; // Ej: "C:\\DGC\\Backups\\"

    // ==================== DATOS DE LA EMPRESA ====================
    public string? Pais { get; set; }
    public string? Provincia { get; set; }
    public string? Ciudad { get; set; }
    public string? Canton { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono1 { get; set; }
    public string? Telefono2 { get; set; }
    public string? Email { get; set; }
    public string? TipoBase { get; set; }
}