using System;
 using System.Collections.Generic;

namespace SistemaDistribuidora.DTOs;

// Lista de ventas — una fila por venta
public record SaleSummaryDto(
    int IdSale,
    string CustomerName,
    string EmployeeName,
    DateTime Date,
    decimal TotalAmount
);

// Factura completa — encabezado + lineas
  public class SaleFullDetailDto
{
    public int IdSale { get; set; }
    public string CustomerName { get; set; } = "";
    public string EmployeeName { get; set; } = "";
    public DateTime Date { get; set; }
    public decimal TotalAmount { get; set; }
    public List<SaleDetailsDto> Details { get; set; } = new();
}

// Una linea de la factura
public record SaleDetailsDto(
    int IdSale,
    int IdSaleDetail,
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal
);

