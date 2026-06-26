using System;
using System.Collections.Generic;

namespace SistemaDistribuidora.DTOs;

public record PurchaseDetailDto
{
    public int IdPurchase { get; set; }
    public string SupplierName { get; set; } = "";
    public string EmployeeName { get; set; } = "";
    public DateTime Date { get; set; }
    public decimal TotalAmount { get; set; }
    public List<PurchaseDetailDto> Details { get; set; } = new();
}

public readonly record struct PurchaseDetailsDto
(
    int PurchaseId,
    int PurchaseDetailId,
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal
);
    

