using System;
using System.Collections.Generic;

namespace SistemaDistribuidora.DTOs;

// <summary>
// DTO para representar los detalles de un cliente, incluyendo su información personal, estado y cantidad de compras realizadas. 
// </summary>
public record CustomerDetailDto(
    int Id, string FullName, string Address,string Phone,string IdentityCard, bool Status,DateTime RegisterDate,
    int QuantityOfPurchases
) : PersonDetailDto(Id, FullName, Address, Phone,IdentityCard,Status,RegisterDate);
