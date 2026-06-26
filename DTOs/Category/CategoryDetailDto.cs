namespace SistemaDistribuidora.DTOs;

/// <summary>
/// Se muestra el detalle de la categoria con sus productos asignados a esa categoria
/// </summary>
/// <param name="CategoryId"></param>
/// <param name="CategoryName"></param>
/// <param name="QuantityOfProducts"></param>

public readonly record struct CategoryDetailDto
(
    int CategoryId,
    string CategoryName,
    int QuantityOfProducts //dato calculado desde la base de datos (lo hara repositorio)

);

public readonly record struct CategoryListDto
(
    int CategoryId,
    string CategoryName
 );





