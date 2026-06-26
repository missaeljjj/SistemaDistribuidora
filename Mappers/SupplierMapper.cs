using System.Collections.Generic;
using System.Linq;
using SistemaDistribuidora.DTOs;
using SistemaDistribuidora.Models;

namespace SistemaDistribuidora.Mappers;

public static class SupplierMapper
{
    //Supplier -> SupplierDetailDto
    private static SupplierDetailDto ToDetailDto(this Supplier supplier,int quantityofpurchases,int quantityofproduct)
        => new SupplierDetailDto
        (
            Id: supplier.IdSupplier,
            FullName: supplier.FullName,
            Address: supplier.Address,
            Phone: supplier.Phone,
            IdentityCard: supplier.IdentityCard,
            TypeOfPerson: supplier.TypeOfPerson,
            Status: supplier.Status,
            RegisterDate: supplier.RegisterDate,
            QuantityOfPurchases: quantityofpurchases,
            QuantityOfProducts: quantityofproduct
        );


    public static IEnumerable<SupplierDetailDto> ToDetailList(this IEnumerable<(Supplier supplier, int quantityofpurchases, int quantityofproduct)> supplier)
        => supplier.Select(s => s.supplier.ToDetailDto(s.quantityofpurchases, s.quantityofproduct));

    //SupplierCreateDto -> Supplier
    public static Supplier ToModel(this SupplierCreateDto dto)
        => new Supplier
        (
            idperson: 0,
            fullname: dto.FullName,
            typeofperson: dto.PersonType,
            identitycard: dto.Identity,
            address: dto.Address,
            phone: dto.Phone,
            registerdate: System.DateTime.Now,
            status: true,
            idsupplier: 0 
        );

    //SupplierUpdateDto -> Supplier
    public static Supplier ToModel(this SupplierUpdateDto dto, Supplier existing)
        => new Supplier
        (
            idperson: dto.IdPerson,
            fullname: dto.FullName ?? existing.FullName,
            typeofperson: dto.PersonType ?? existing.TypeOfPerson,
            identitycard: dto.Identity ?? existing.IdentityCard,
            address: dto.Address ?? existing.Address,
            phone: dto.Phone ?? existing.Phone,
            registerdate: existing.RegisterDate,
            status: dto.Status ?? existing.Status,
            idsupplier: dto.SupplierId
        );

  
    public static IEnumerable<SupplierSummaryDto> ToSummaryDtoList(this IEnumerable<Supplier> suppliers)
        => suppliers.Select(s => s.ToSummaryDto());

    public static SupplierSummaryDto ToSummaryDto(this Supplier supplier)
        => new SupplierSummaryDto
        (
            Id: supplier.IdSupplier,
            Name: supplier.FullName
        );
    
}
