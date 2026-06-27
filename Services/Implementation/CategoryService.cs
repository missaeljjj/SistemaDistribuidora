using SistemaDistribuidora.Services.Interfaces;
using SistemaDistribuidora.Repositories.Interfaces;
using SistemaDistribuidora.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using SistemaDistribuidora.Mappers;
using System.Linq;
using SistemaDistribuidora.Exceptions;

namespace SistemaDistribuidora.Services.Implementation;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _ICategoryRepository;
    private readonly IProductRepository _IProductRepository;

    public CategoryService(ICategoryRepository IcategoryRepository, IProductRepository IProductRepository)
    {
        _ICategoryRepository = IcategoryRepository;  
        _IProductRepository =   IProductRepository;   
    }

    public async Task CreateNewCategory(CategoryCreateDto dto)
    {

        bool duplicated = await _ICategoryRepository.ExistsByNameAsync(dto.CategoryName);

        //si hubo una categoria con el nombre de pone una excepcion de negocio
        if(duplicated)
            throw new BussinessRulesException("Categoria existente",$"Ya existe una categoria con el nombre {dto.CategoryName}");

        //Covertimos el dto a modelo 
        var category = dto.ToModel();
        //hacemos la operacion desde el repositorio
        await _ICategoryRepository.InsertAsync(category);
    }

    public async Task UpdateCategory(CategoryUpdateDto dto)
    {
        var existing = await _ICategoryRepository.GetByIdAsync(dto.CategoryId);
        
        if(!string.IsNullOrWhiteSpace(dto.CategoryName))
        {
            bool duplicated = await _ICategoryRepository.ExistsByNameExcludedAsync(dto.CategoryName,dto.CategoryId);

            if(duplicated)
                throw new BussinessRulesException("Categoria duplicada", $"Ya existe una categoria con el nombre {dto.CategoryName}");
        }
    
        var update = dto.ToModel(existing);
        await _ICategoryRepository.UpdateAsync(update);
    }

    public async Task DeleteCategory(int CategoryId)
    {
        // Verifica que existe
        await _ICategoryRepository.GetByIdAsync(CategoryId);

        // Regla de negocio: no se puede eliminar si tiene productos asociados
        bool hasProducts = await _IProductRepository.ExistingProductWithCategory(CategoryId);

        if (hasProducts)
            throw new BussinessRulesException(
                "CategoriaConProductos",
                "No se puede eliminar la categoría porque tiene productos asociados."
            );

        await _ICategoryRepository.DeleteAsync(CategoryId);
    }

    public async Task<IEnumerable<CategoryListDto>> GetAllCategories()
    {
        var CategoryList = await _ICategoryRepository.GetAllAsync();
        return CategoryList.ToCategoryListDto();
    }

    public async Task<IEnumerable<CategoryDetailDto>> GetAllWithQuantityOfProducts()
    {
        var CategoryList = await _ICategoryRepository.GetAllCategoriesWithQuantityOfProductsAsync();
        return CategoryList.ToDetailDtoList();
    } 
}
