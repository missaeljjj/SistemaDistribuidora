using System;
using System.Collections.Generic;
using System.Linq;
using SistemaDistribuidora.DTOs;
using SistemaDistribuidora.Models;

namespace SistemaDistribuidora.Mappers;

public static class PurchaseMapper
{
    // PurchaseCreateDto → Purchase 

    // PurchaseDetailItemDto (carrito) → PurchaseDetail 
    private static PurchaseDetail ToPurchaseDetail(this PurchaseDetailItemDto item, int purchaseId)
        => new PurchaseDetail(
            idPurchaseDetail: 0,
            transactionId: purchaseId,
            productId: item.ProductId,
            quantity: item.Quantity,
            unitPrice: item.UnitPrice
        );

    private static IEnumerable<PurchaseDetail> ToPurchaseDetailList(this IEnumerable<PurchaseDetailItemDto> items, int purchaseId)
        => items.Select(item => item.ToPurchaseDetail(purchaseId));

    // PurchaseCreateDto → Purchase (modelo completo con su carrito de detalles)
    public static Purchase ToModel(this PurchaseCreateDto dto)
    {
        const int idPurchase = 0;

        var cart = dto.Items.ToPurchaseDetailList(idPurchase);

        return new Purchase(
            idPurchase: idPurchase,
            supplierId: dto.SupplierId,
            employeeId: dto.EmployeeId,
            date: DateTime.Now,
            totalAmount: dto.TotalAmount,
            PurchaseCart: cart
        );
    }

    // Purchase → PurchaseDetailDto (para mostrar una compra como factura)

    public static IEnumerable<PurchaseDetailDto> ToPurchaseDetailDtoList(
        this Purchase purchase,
        IReadOnlyDictionary<int, string> productNames)
        => purchase.Cart
            .Cast<PurchaseDetail>()
            .Select(detail => new PurchaseDetailDto(
                PurchaseId: purchase.IdPurchase,
                PurchaseDetailId: detail.IdPurchaseDetail,
                ProductId: detail.ProductId,
                ProductName: productNames.GetValueOrDefault(detail.ProductId, "Desconocido"),
                Quantity: detail.Quantity,
                UnitPrice: detail.UnitPrice
            ));


}