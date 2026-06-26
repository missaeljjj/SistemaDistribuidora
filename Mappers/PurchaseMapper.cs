using System;
using System.Collections.Generic;
using System.Linq;
using SistemaDistribuidora.DTOs;
using SistemaDistribuidora.Models;

namespace SistemaDistribuidora.Mappers;

public static class PurchaseMapper
{
    // PurchaseCreateDto → Purchase 

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
            status: dto.PurchaseStatus,
            totalAmount: 0,
            PurchaseCart: cart
        );
    }
    
}