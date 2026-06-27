using System.Collections.Generic;
using System.Linq;
using SistemaDistribuidora.DTOs;
using SistemaDistribuidora.Models;

namespace SistemaDistribuidora.Mappers;

public static class CategoryMapper
{
    //CategoryCreateDto -> Category
    public static Category ToModel(this CategoryCreateDto dto)
        => new Category
        (
            idcategory: 0,
            name: dto.CategoryName
        );

    //CategortUpdate -> Category
    public static Category ToModel(this CategoryUpdateDto dto, Category existing)
        => new Category
        (
            idcategory: dto.CategoryId,
            name: dto.CategoryName ?? existing.Name
        );

    private static CategoryListDto ToList(this Category category)
        => new CategoryListDto
        (
                CategoryId: category.IdCategory,
                CategoryName: category.Name
        );

    public static IEnumerable<CategoryListDto> ToCategoryListDto(this IEnumerable<Category> categories)
        => categories.Select(c => c.ToList());

    public static IEnumerable<CategoryDetailDto> ToDetailDtoList(
     this IEnumerable<(Category category, int QuantityOfProducts)> items)
     => items.Select(i => i.category.ToDetail(i.QuantityOfProducts));


    private static CategoryDetailDto ToDetail(this Category category, int quantityOfProducts)
        => new CategoryDetailDto
        (
            CategoryId: category.IdCategory,
            CategoryName: category.Name,
            QuantityOfProducts: quantityOfProducts
        );

    
}