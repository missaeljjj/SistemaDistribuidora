using SistemaDistribuidora.Models;
using System.Collections.Generic;   
using System.Threading.Tasks;

namespace SistemaDistribuidora.Repositories.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    // Obtiene todos los productos en inventario    
    Task<IEnumerable<Product>> GetAllProductsInInventoryAsync();
    //Obtiene los productos que se han vendido más con la cantidad de ventas
    Task<IEnumerable<Product>> GetAllProductsWithQuantityOfSales();

}