using System.Threading.Tasks;
using Daxsys.Domain.Entities.Factura;

namespace Daxsys.Domain.Interfaces
{
    public interface IProductRepository
    {
        // Para productos (AdcArt)
        Task<AdcArt?> GetProductByCodeAsync(string code);
        Task UpdateProductAsync(AdcArt product);

        // Para servicios (AdcServ)
        Task<AdcServ?> GetServiceByCodeAsync(string code);
        Task UpdateServiceAsync(AdcServ service);

        // Para stock
        Task<decimal> GetStockAsync(string codigoArticulo, string codigoBodega);
        Task UpdateStockAsync(string codigoArticulo, string codigoBodega, decimal cantidad);

        // Método genérico que acepta 2 argumentos (para compatibilidad)
        Task<object?> GetByCodeAsync(string code, string sucursal);
    }
}