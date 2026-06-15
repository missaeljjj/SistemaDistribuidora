using System.Collections.Generic;
using System.Linq;

namespace SistemaDistribuidora.DTOs;
/// <summary>
/// DTO Creacion de venta
/// Simulamos carrito de venta del usuario 
/// </summary>
public class SaleCreateDto
{
    public int CustomerId { get; init; }

    public int EmployeeId { get; init; }

    public List<SaleDetailItemDto> Items { get; init; } = new();

    public string SaleStatus { get; init; } = "";

}

//Dto para representar los detalles de cada producto vendido en la venta
public readonly record struct SaleDetailItemDto
(
    int ProductId,
    int Quantity,
    decimal UnitPrice
);
