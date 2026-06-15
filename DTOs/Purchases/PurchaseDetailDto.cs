namespace SistemaDistribuidora.DTOs;

/// <summary>
/// Detalles relavantes de ventas
/// Simulacion de una especie de factura 
/// </summary>
/// <param name="PurchaseId",></param>
/// <param name="PurchaseDetailId"></param>
/// <param name="ProductId"></param>
/// <param name="ProductName"></param>
/// <param name="Quantity"></param>
/// <param name="UnitPrice"></param>
public readonly record struct PurchaseDetailDto
(
    int PurchaseId,
    int PurchaseDetailId,
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice
);
    

