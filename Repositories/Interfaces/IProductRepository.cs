using SistemaDistribuidora.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Repositories.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    // Productos con nombre de proveedor y categoria (para inventario)
    Task<IEnumerable<(Product product, string SupplierName, string CategoryName)>> GetAllProductsInInventoryAsync();

    // Productos con cantidad de ventas
    Task<IEnumerable<(Product product, int QuantityOfSales)>> GetAllProductsWithQuantityOfSales();

    // Validaciones de nombre duplicado
    Task<bool> ExistsByNameAsync(string name);
    Task<bool> ExistsByNameExcludedAsync(string name, int idToExclude);
    // Verifica si hay productos asociados a una categoria antes de eliminarla
    Task<bool> ExistingProductWithCategory(int categoryId);

    Task<bool> UpdateProductPricesAsync(int productId, decimal newSalePrice);
}