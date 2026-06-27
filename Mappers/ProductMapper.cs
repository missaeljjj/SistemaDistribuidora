using SistemaDistribuidora.DTOs;
using SistemaDistribuidora.Models;
using System.Collections.Generic;
using System.Linq;

namespace SistemaDistribuidora.Mappers;

public static class ProductMapper
{
    public static Product ToModel(this ProductCreateDto dto)
        => new Product
        (
            idProduct:     0,
            name:          dto.Name,
            stock:         0,
            purchasePrice: 0,
            salePrice:     0,
            SupplierId:    dto.SupplierId,
            categoryId:    dto.CategoryId,
            status:        true
        );

    public static Product ToModel(this ProductUpdateDto dto, Product existing)
        => new Product
        (
            idProduct:     dto.ProductId,
            name:          dto.Name          ?? existing.Name,
            stock:         existing.Stock,
            categoryId:    dto.CategoryId    ?? existing.CategoryId,
            SupplierId:    dto.SupplierId    ?? existing.SupplierId,
            salePrice:     existing.SalePrice,
            purchasePrice: existing.PurchasePrice,
            status:        dto.Status        ?? existing.Status
        );

    private static ProductDetailDto ToDetail(this Product product, string SupplierName, string CategoryName, int QuantityOfSales = 0)
        => new ProductDetailDto
        (
            ProductId:                   product.IdProduct,
            ProductName:                 product.Name,
            Stock:                       product.Stock,
            SupplierName:                SupplierName,
            CategoryName:                CategoryName,
            QuantityOfSaleOfThisProduct: QuantityOfSales
        );

    public static IEnumerable<InventoryDetailDto> ToInventoryList(
        this IEnumerable<(Product product, string SupplierName, string CategoryName)> items)
        => items.Select(i => i.product.ToDetail(i.SupplierName, i.CategoryName));

    public static IEnumerable<ProductDetailDto> ToDetailList(
        this IEnumerable<(Product product, int QuantityOfSales)> items)
        => items.Select(i => i.product.ToDetail("", "", i.QuantityOfSales));

    public static IEnumerable<ProductSummaryDto> ToSummaryList(this IEnumerable<Product> products)
        => products.Select(p => p.ToSummary());

    private static ProductSummaryDto ToSummary(this Product product)
        => new ProductSummaryDto
        (
            ProductId:   product.IdProduct,
            ProductName: product.Name,
            stock:       product.Stock
        );
}