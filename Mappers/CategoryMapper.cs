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

    //CategorySummary utilizado en la lista 
    public static CategorySummaryDto ToSummaryDto(this Category category)
        => new CategorySummaryDto
        (
            Id: category.IdCategory,
            CategoryName: category.Name
        );

    public static IEnumerable<CategorySummaryDto> ToSummaryDtoList(this IEnumerable<Category> categories)
        => categories.Select(c => c.ToSummaryDto());
}