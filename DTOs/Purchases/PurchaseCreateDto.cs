using System.Collections.Generic;
using System.Linq;

namespace SistemaDistribuidora.DTOs;
/// <summary>
/// DTO creacion de compra 
/// necesitamos una lista que simula ser un carrito de compras
/// </summary>
public class PurchaseCreateDto
{
    public int SupplierId { get; init; }
    public int EmployeeId { get; init; }
    public IEnumerable<PurchaseDetailItemDto> Items { get; init; } = new List<PurchaseDetailItemDto>();
    public string PurchaseStatus { get; init; } = "";
}

/// <summary>
/// estrcutura que simula el carrito de venta con los datos
/// respectivos necesarios
/// </summary>
/// <param name="ProductId"></param>
/// <param name="Quantity"></param>
/// <param name="UnitPrice"></param>
public readonly record struct PurchaseDetailItemDto
(
    int ProductId,
    int Quantity,
    decimal UnitPrice
);



