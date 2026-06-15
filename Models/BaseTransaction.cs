using SistemaDistribuidora.Exceptions;
using System;
using System.Collections.Generic;
namespace SistemaDistribuidora.Models;

public abstract class BaseTransaction
{
    private int _IdTransaction;
    private int _EmployeeId;
    private DateTime _Date = DateTime.Now;
    private decimal _TotalAmount;
    private string _Status = "";
    public IEnumerable<BaseTransactionDetail> Cart { get; protected set; } = new List<BaseTransactionDetail>();

    public int IdTransaction
    {
        get => _IdTransaction;
        set => _IdTransaction = value;
    }

    public string Status
    {
        get => _Status;
        protected set
        {
            string NewValue = value?.Trim() ?? "";
            if (NewValue != "Realizada" && NewValue != "Pendiente" && NewValue != "Cancelada")
                throw new ValidationException("El tipo de persona debe ser 'Natural' o 'Juridica'", nameof(Status));
            _Status = NewValue;
        }
    }
    public int EmployeeId
    {
        get => _EmployeeId;
        set => _EmployeeId = value;
    }
    public DateTime Date
    {
        get => _Date;
        set => _Date = value;
    }
    public decimal TotalAmount
    {
        get => _TotalAmount;
        set => _TotalAmount = value > 0 ? value : throw new ValidationException("El monto total debe ser mayor a cero", nameof(TotalAmount));
    }

  

    public BaseTransaction(int idTransaction, int employeeId, DateTime date, decimal totalAmount,string Status, IEnumerable<BaseTransactionDetail> cart)
    {
        this.IdTransaction = idTransaction;
        this.EmployeeId = employeeId;
        this.Date = date;
        this.TotalAmount = totalAmount;
        this.Status = Status;
        this.Cart = cart;
    }
}

public abstract class BaseTransactionDetail
{
    private int _IdTransactionDetail;
    private int _TransactionId;
    private int _ProductId;
    private int _Quantity;
    private decimal _UnitPrice;
    public int IdTransactionDetail
    {
        get => _IdTransactionDetail;
        protected set => _IdTransactionDetail = value;
    }
    public int TransactionId
    {
        get => _TransactionId;
        protected set => _TransactionId = value;
    }
    public int ProductId
    {
        get => _ProductId;
        protected set => _ProductId = value;
    }
    public int Quantity
    {
        get => _Quantity;
        protected set => _Quantity = value > 0 ? value : throw new ValidationException("La cantidad debe ser mayor a cero", nameof(Quantity));
    }
    public decimal UnitPrice
    {
        get => _UnitPrice;
        protected set => _UnitPrice = value > 0 ? value : throw new ValidationException("El precio unitario debe ser mayor a cero", nameof(UnitPrice));
    }
    public BaseTransactionDetail(int idTransactionDetail, int transactionId, int productId, int quantity, decimal unitPrice)
    {
        this.IdTransactionDetail = idTransactionDetail;
        this.TransactionId = transactionId;
        this.ProductId = productId;
        this.Quantity = quantity;
        this.UnitPrice = unitPrice;
    }
}