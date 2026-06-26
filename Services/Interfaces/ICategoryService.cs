using System.Threading.Tasks;
using SistemaDistribuidora.DTOs;
using System.Collections.Generic;

namespace SistemaDistribuidora.Services.Interfaces;

public interface ICategoryService 
{
    Task CreateNewCategory(CustomerCreateDto customerCreateDto);

    Task UpdateCategory(CustomerUpdateDto customerUpdateDto);

    Task DeleteCategory(int IdCategory);

    Task<IEnumerable<CustomerSummaryDto>> GetAllCategories();

    Task<IEnumerable<CustomerDetailDto>> GetAllWithQuantityOfProducts();
}
