using System.Threading.Tasks;
using Daxsys.Domain.Entities;
using Daxsys.Domain.Entities.Factura;
using Daxsys.Domain.Interfaces;
using Daxsys.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Daxsys.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AdcArt?> GetProductByCodeAsync(string code)
        {
            return await _context.Set<AdcArt>()
                .FirstOrDefaultAsync(p => p.Art_codigo == code);
        }

        public async Task<AdcServ?> GetServiceByCodeAsync(string code)
        {
            return await _context.Set<AdcServ>()
                .FirstOrDefaultAsync(s => s.Sev_codigo == code);
        }

        // Este es el método que acepta 2 argumentos
        public async Task<object?> GetByCodeAsync(string code, string sucursal)
        {
            var product = await GetProductByCodeAsync(code);
            if (product != null) return product;

            var service = await GetServiceByCodeAsync(code);
            return service;
        }

        public async Task UpdateProductAsync(AdcArt product)
        {
            _context.Set<AdcArt>().Update(product);
            await Task.CompletedTask;
        }

        public async Task UpdateServiceAsync(AdcServ service)
        {
            _context.Set<AdcServ>().Update(service);
            await Task.CompletedTask;
        }

        public async Task<decimal> GetStockAsync(string codigoArticulo, string codigoBodega)
        {
            var stock = await _context.Set<AdcStk>()
                .Where(s => s.Art_codigo == codigoArticulo && s.Bod_codigo == codigoBodega)
                .SumAsync(s => (decimal?)s.Stk_cantidad) ?? 0;
            return stock;
        }

        public async Task UpdateStockAsync(string codigoArticulo, string codigoBodega, decimal cantidad)
        {
            var stock = await _context.Set<AdcStk>()
                .FirstOrDefaultAsync(s => s.Art_codigo == codigoArticulo && s.Bod_codigo == codigoBodega);

            if (stock != null)
            {
                stock.Stk_cantidad += cantidad;
                _context.Set<AdcStk>().Update(stock);
            }
            await Task.CompletedTask;
        }
    }
}