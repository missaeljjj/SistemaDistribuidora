using SistemaDistribuidora.DTOs;
using SistemaDistribuidora.Models;
using System.Collections.Generic;
using System.Linq;

namespace SistemaDistribuidora.Mappers;

public static class ProductMapper
{
    //ProductCreateDto -> Producto
    public static Product ToModel(this ProductCreateDto dto)
        => new Product
        (
            idProduct: 0, //Asginado por la BD
            name: dto.Name,
            stock: 0,         //Definido en la primera compra
            purchasePrice: 0, //Definido en la primera compra que hacemos
            salePrice: 0,     //Definido en la primera venta
            SupplierId: dto.SupplierId,
            categoryId: dto.CategoryId,
            status: true

        );

    //ProductUpdateDto -> Product
    public static Product ToModel(this ProductUpdateDto dto, Product existing)
        => new Product
        (
            idProduct: dto.ProductId,
            name: dto.Name ?? existing.Name,
            stock: existing.Stock,
            categoryId: dto.CategoryId ?? existing.CategoryId,
            SupplierId: dto.SupplierId ?? existing.SupplierId,
            salePrice: existing.SalePrice,
            purchasePrice: existing.PurchasePrice,
            status: dto.Status ?? existing.Status

        );

    //Product -> ProductDetailDto
    public static ProductDetailDto ToDetailDto(this Product product, int QuantityOfSales, string SupplierName, string CategoryName)
        => new ProductDetailDto
        (
            ProductId:                   product.IdProduct,
            ProductName:                 product.Name,
            Stock:                       product.Stock,
            SupplierName:                SupplierName,   //Parametro que no es propio del objeto (DBPARAM)
            CategoryName:                CategoryName,   //DBPARAM
            QuantityOfSaleOfThisProduct: QuantityOfSales //DBPARAM 'COUNT' FROM DATABASE

        );

    //Detalle de inventario para metodo del repositorio
    //Product -> InventoryDetailDto
    public static InventoryDetailDto ToInventoryDetail(this Product product, string SupplierName, string CategoryName)
        => new InventoryDetailDto
        (
            ProductId:      product.IdProduct,
            ProductName:    product.Name,
            Stock:          product.Stock,
            SupplierName:   SupplierName,   //Parametro que no es propio del objeto (DBPARAM)
            CategoryName:   CategoryName
         );

    //Lista que obtiene los datos de ProductoSummaryDto para mostrar como catalogo
    public static IEnumerable<ProductSummaryDto> ToSummaryListDto(this IEnumerable<Product> product)
        => product.Select(p => p.ToSummary());

    //SUMMARY PARA LISTA
    public static ProductSummaryDto ToSummary(this Product product)
        => new ProductSummaryDto
        (
            ProductId:   product.IdProduct,
            ProductName: product.Name
        );
}