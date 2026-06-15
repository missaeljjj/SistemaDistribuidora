using SistemaDistribuidora.Repositories.Interfaces;
using SistemaDistribuidora.Repositories.DataBaseConnection;
using System.Threading.Tasks;
using SistemaDistribuidora.Models;
using System.Collections.Generic;

namespace SistemaDistribuidora.Repositories.Implementation;


public class CategoryRepository : ICategoryRepository
{
    private readonly IDataBase _DataBase;

    public CategoryRepository(IDataBase database)
    {
        _DataBase = database;
    }

    //METODOS GENERICOS
    //FALTA IMPLEMENTAR

    public async Task InsertAsync(Category category)
    {

    }

    public async Task UpdateAsync(Category category)
    {

    }

    public async Task DeleteAsync(int CategoryId)
    {

    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return null;
    }

    public async Task<Category> GetByIdAsync(int CategoryId)
    {
        return null;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesWithQuantityOfProductsAsync() 
    {
        return null;
    }
}

    

