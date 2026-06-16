using SistemaDistribuidora.Models;
using SistemaDistribuidora.Repositories.DataBaseConnection;
using SistemaDistribuidora.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Repositories.Implementation;
public class ProductRepository : IProductRepository
{
    private readonly IDataBase _DataBase;

    public ProductRepository(IDataBase dataBase )
    {
        _DataBase = dataBase;
    }

    public async Task InsertAsync(Product product)
    {

    }

    public async Task UpdateAsync(Product product)
    {

    }

    public async Task DeleteAsync(int ProductId)
    {

    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return null!;
    }

    public async Task<Product> GetByIdAsync(int ProductId)
    {
        return null!;
    }

    public async Task<IEnumerable<Product>> GetAllProductsInInventoryAsync() 
    {
        return null!;
    }

    public async Task<IEnumerable<Product>> GetAllProductsWithQuantityOfSales()
    {
        return null!;
    }


}


