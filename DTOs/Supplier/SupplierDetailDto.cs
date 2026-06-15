using System;
namespace SistemaDistribuidora.DTOs;

public record SupplierDetailDto
(
    int Id, string FullName, string Address, string Phone,string IdentityCard, bool Status,
    DateTime RegisterDate, int QuantityOfPurchases, int QuantityOfProducts
) : PersonDetailDto(Id, FullName, Address, Phone,IdentityCard, Status,RegisterDate);
