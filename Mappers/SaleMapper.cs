using SistemaDistribuidora.DTOs;
using SistemaDistribuidora.Models;
using System.Collections.Generic;
using System.Linq;

namespace SistemaDistribuidora.Mappers;

public static class SaleMapper
{
    //Creamos el detalle de venta 
    private static SaleDetail ToSaleDetail(this SaleDetailItemDto dto, int SaleId)
        => new SaleDetail
        (
            idSaleDetail: 0,
            transactionId: SaleId,
            productId: dto.ProductId,
            quantity: dto.Quantity,
            unitPrice: dto.UnitPrice
        );

    // Convierte toda la lista del carrito a SaleDetail
    private static IEnumerable<SaleDetail> ToSaleDetaiList(this IEnumerable<SaleDetailItemDto> items, int SaleId)
        => items.Select(item => item.ToSaleDetail(SaleId));


    //SaleCreateDto -> Sale
    public static Sale ToModel(this SaleCreateDto dto)
    {
        const int SaleId = 0;

        //Asignamos la  lista aqui a cart para ponerla en el constructor
        var Cart = dto.Items.ToSaleDetaiList(SaleId);

        return new Sale
        (

             idSale: SaleId ,
             customerId: dto.CustomerId,
             employeeId: dto.EmployeeId,
             date: System.DateTime.Now,
             totalAmount: dto.TotalAmount,
             SaleCart: Cart     

                
        );

            
    }

    public static IEnumerable<SaleDetailDto> ToSaleDetailDtoList(
        this Sale sale,
        IReadOnlyDictionary<int, string> productNames,
        IReadOnlyDictionary<int, string> categoryNames)
        => sale.Cart
            .Cast<SaleDetail>()
            .Select(detail => new SaleDetailDto(
                SaleId: sale.IdSale,
                SaleDetailId: detail.IdSaleDetail,
                ProductId: detail.ProductId,
                ProductName: productNames.GetValueOrDefault(detail.ProductId, "Desconocido"),
                CategoryName: categoryNames.GetValueOrDefault(detail.ProductId, "Sin categoría"),
                Quantity: detail.Quantity,
                UnitPrice: detail.UnitPrice,
                Subtotal: detail.Quantity * detail.UnitPrice
            ));






}
