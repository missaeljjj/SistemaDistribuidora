namespace SistemaDistribuidora.DTOs;
//Detalles de productos relavantes las propiedades necesarias para mapear
public record ProductDetailDto
(
    int ProductId,
    string ProductName,
    int Stock,
    string SupplierName,
    string CategoryName,
    int QuantityOfSaleOfThisProduct  //Dato calculado en la base de datos que nos tiene que devolver un numero entero de todas las ventas que participo este producto

);

public record InventoryDetailDto
(
    int ProductId,
    string ProductName,
    int Stock,
    string SupplierName,
    string CategoryName

 );


