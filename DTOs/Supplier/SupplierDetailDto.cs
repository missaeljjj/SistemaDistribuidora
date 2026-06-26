using System;
namespace SistemaDistribuidora.DTOs;

public record SupplierDetailDto
(
    int Id, string FullName, string Address, string Phone,string IdentityCard,string TypeOfPerson, bool Status,
    DateTime RegisterDate, int QuantityOfPurchases, int QuantityOfProducts
) : PersonDetailDto(Id, FullName, Address, Phone,IdentityCard, TypeOfPerson, Status, RegisterDate);

public record SupplierListDto
(
    int Id, string FullName, string Address, string Phone, string IdentityCard, string TypeOfPerson, bool Status,
    DateTime RegisterDate, int QuantityOfPurchases
) : PersonDetailDto(Id, FullName, Address, Phone, IdentityCard, TypeOfPerson, Status, RegisterDate);