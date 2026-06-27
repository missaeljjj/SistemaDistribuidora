namespace SistemaDistribuidora.DTOs;
//Detalles de productos relavantes las propiedades necesarias para mapear
public record InventoryDetailDto
(
    int ProductId,
    string ProductName,
    int Stock,
    string SupplierName,
    string CategoryName
);

// Extiende InventoryDetailDto agregando solo el campo calculado
public record ProductDetailDto
(
    int ProductId,
    string ProductName,
    int Stock,
    string SupplierName,
    string CategoryName,
    int QuantityOfSaleOfThisProduct
) : InventoryDetailDto(ProductId, ProductName, Stock, SupplierName, CategoryName);


