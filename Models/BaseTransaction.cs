using System;
namespace SistemaDistribuidora.Models;

public abstract class BaseTransaction
{
    private int _IdTransaction;
    private int _EmployeeId;
    private DateTime _Date = DateTime.Now;
    private decimal _TotalAmount;
    public int IdTransaction
    {
        get => _IdTransaction;
        set => _IdTransaction = value;
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
        set => _TotalAmount = value > 0 ? value : throw new ArgumentException("El monto total debe ser mayor a cero");
    }

    public BaseTransaction(int idTransaction, int employeeId, DateTime date, decimal totalAmount)
    {
        this.IdTransaction = idTransaction;
        this.EmployeeId = employeeId;
        this.Date = date;
        this.TotalAmount = totalAmount;
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
        protected set => _Quantity = value > 0 ? value : throw new ArgumentException("La cantidad debe ser mayor a cero");
    }
    public decimal UnitPrice
    {
        get => _UnitPrice;
        protected set => _UnitPrice = value > 0 ? value : throw new ArgumentException("El precio unitario debe ser mayor a cero");
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