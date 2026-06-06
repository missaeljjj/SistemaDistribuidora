using SistemaDistribuidora.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Repositories.Interfaces;

public interface ISuplierRepository : IGenericRepository<Supplier>
{
    // Obtiene todos los proveedores junto con las compras que han realizado
    Task<IEnumerable<Supplier>> GetAllSuppliersWithPurchasesAsync();
    // Obtiene todos los proveedores que no han realizado compras
    Task<IEnumerable<Supplier>> GetAllSuppliersWithoutPurchasesAsync();

    // Obtiene todos los proveedores junto con los productos que han suministrado
    Task<IEnumerable<Supplier>> GetAllSuppliersWithProductsAsync();

    Task<IEnumerable<Supplier>> GetAllSuppliersWithoutProductsAsync();
}