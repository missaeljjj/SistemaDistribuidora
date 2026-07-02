namespace SistemaDistribuidora.DTOs;
//Detalles de productos relavantes las propiedades necesarias para mapear
public record InventoryDetailDto
(
    int ProductId,
    string ProductName,
    int Stock,
    string SupplierName,
    string CategoryName,
    decimal SalePrice,
    decimal PurchasePrice
);

// Extiende InventoryDetailDto agregando solo el campo calculado
public record ProductDetailDto
(
    int ProductId,
    string ProductName,
    int Stock,
    string SupplierName,
    string CategoryName,
    int QuantityOfSaleOfThisProduct,
    decimal SalePrice,
    decimal PurchasePrice
) : InventoryDetailDto(ProductId, ProductName, Stock, SupplierName, CategoryName, SalePrice, PurchasePrice);


