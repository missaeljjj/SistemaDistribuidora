using System.Threading.Tasks;
using SistemaDistribuidora.DTOs;
using System.Collections.Generic;

namespace SistemaDistribuidora.Services.Interfaces;

public interface ICategoryService 
{
    Task CreateNewCategory(CategoryCreateDto dto);

    Task UpdateCategory(CategoryUpdateDto dto);

    Task DeleteCategory(int IdCategory);

    Task<IEnumerable<CategoryListDto>> GetAllCategories();

    Task<IEnumerable<CategoryDetailDto>> GetAllWithQuantityOfProducts();
}
