using SistemaDistribuidora.Models;
using System.Collections.Generic;   
using System.Threading.Tasks;

namespace SistemaDistribuidora.Repositories.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    // Obtiene todos los productos en inventario    
    Task<IEnumerable<Product>> GetAllProductsInInventoryAsync();
    //Obtiene los productos que se han vendido más
    Task<IEnumerable<Product>> GetAllProductsWithMoreSales();
    //Obtiene ls productos que se han vendido menos
    Task<IEnumerable<Product>> GetAllProductsWithLessSales();
    //Obtiene los productos que tienen un stock bajo
    Task<IEnumerable<Product>> GetAllProductsWithLowStock();

}