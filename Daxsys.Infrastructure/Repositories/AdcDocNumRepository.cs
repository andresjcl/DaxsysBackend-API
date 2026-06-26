using System;
using System.Threading.Tasks;
using Daxsys.Domain.Entities.Factura;
using Daxsys.Domain.Interfaces;
using Daxsys.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Daxsys.Infrastructure.Repositories;

public class AdcDocNumRepository : IAdcDocNumRepository
{
    private readonly AppDbContext _context;

    public AdcDocNumRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> GetNextDocumentNumberAsync(string idLugar, string idDocumento)
    {
        var docNum = await _context.Set<AdcDocNum>()
            .FirstOrDefaultAsync(d => d.Id_Lugar == idLugar && d.id_Documento == idDocumento);

        if (docNum == null)
        {
            docNum = new AdcDocNum
            {
                Id_Lugar = idLugar,
                id_Documento = idDocumento,
                UltimoNumero = 1,
                UltimaFecha = DateTime.Now
            };
            await _context.Set<AdcDocNum>().AddAsync(docNum);
            await _context.SaveChangesAsync();
            return 1;
        }

        var nextNumber = (docNum.UltimoNumero ?? 0) + 1;
        return nextNumber;
    }

    public async Task UpdateDocumentNumberAsync(string idLugar, string idDocumento, decimal nuevoNumero, DateTime fecha)
    {
        var docNum = await _context.Set<AdcDocNum>()
            .FirstOrDefaultAsync(d => d.Id_Lugar == idLugar && d.id_Documento == idDocumento);

        if (docNum != null)
        {
            docNum.UltimoNumero = nuevoNumero;
            docNum.UltimaFecha = fecha;
            _context.Set<AdcDocNum>().Update(docNum);
            await _context.SaveChangesAsync();
        }
    }

    // ✅ NUEVO: Obtener un registro de numeración
    public async Task<AdcDocNum?> GetDocumentNumberAsync(string idLugar, string idDocumento)
    {
        return await _context.Set<AdcDocNum>()
            .FirstOrDefaultAsync(d => d.Id_Lugar == idLugar && d.id_Documento == idDocumento);
    }

    // ✅ NUEVO: Crear un registro de numeración
    public async Task<AdcDocNum> CreateDocumentNumberAsync(AdcDocNum docNum)
    {
        await _context.Set<AdcDocNum>().AddAsync(docNum);
        await _context.SaveChangesAsync();
        return docNum;
    }

    // ✅ NUEVO: Actualizar un registro de numeración
    public async Task UpdateAsync(AdcDocNum docNum)
    {
        _context.Set<AdcDocNum>().Update(docNum);
        await _context.SaveChangesAsync();
    }
}