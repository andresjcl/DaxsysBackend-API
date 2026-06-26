using System;
using System.Threading.Tasks;
using Daxsys.Domain.Entities.Factura;

namespace Daxsys.Domain.Interfaces;

public interface IAdcDocNumRepository
{
    /// <summary>
    /// Obtiene el siguiente número de documento desde la tabla AdcDocNum
    /// </summary>
    /// <param name="idLugar">Código de sucursal + punto de emisión (Ej: AV6007-001)</param>
    /// <param name="idDocumento">Tipo de documento (Ej: FAC, REM, N/C)</param>
    /// <returns>El siguiente número de documento</returns>
    Task<decimal> GetNextDocumentNumberAsync(string idLugar, string idDocumento);

    /// <summary>
    /// Actualiza el número de documento en la tabla AdcDocNum
    /// </summary>
    /// <param name="idLugar">Código de sucursal + punto de emisión</param>
    /// <param name="idDocumento">Tipo de documento</param>
    /// <param name="nuevoNumero">Nuevo número de documento</param>
    /// <param name="fecha">Fecha del documento</param>
    Task UpdateDocumentNumberAsync(string idLugar, string idDocumento, decimal nuevoNumero, DateTime fecha);

    /// <summary>
    /// Obtiene el registro completo de numeración para un documento
    /// </summary>
    Task<AdcDocNum?> GetDocumentNumberAsync(string idLugar, string idDocumento);

    /// <summary>
    /// Crea un nuevo registro de numeración para un documento
    /// </summary>
    Task<AdcDocNum> CreateDocumentNumberAsync(AdcDocNum docNum);

    /// <summary>
    /// Actualiza el número de documento (método genérico)
    /// </summary>
    Task UpdateAsync(AdcDocNum docNum);
}