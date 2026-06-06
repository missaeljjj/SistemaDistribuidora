using SistemaDistribuidora.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace SistemaDistribuidora.Repositories.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category>
{
    //Obtiene todas las categorías junto con sus productos asociados
    Task<IEnumerable<Category>> GetAllCategoriesWithProductsAsync();
}