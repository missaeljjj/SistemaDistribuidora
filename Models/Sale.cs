using System;
using System.Collections.Generic;
namespace SistemaDistribuidora.Models;

public class Sale : BaseTransaction
{
    private int _IdSale;
    private int _CustomerId;
    public int IdSale
    {
        get => _IdSale;
        private set => _IdSale = value;
    }
    public int CustomerId
    {
        get => _CustomerId;
        private set => _CustomerId = value;
    }


    public Sale(int idSale, int customerId, int employeeId, DateTime date, decimal totalAmount,IEnumerable<SaleDetail> SaleCart) 
        : base(idSale, employeeId, date, totalAmount,SaleCart)
    {
        this.IdSale = idSale;
        this.CustomerId = customerId;
    }
}

public class SaleDetail : BaseTransactionDetail
{
    private int _IdSaleDetail;
    public int IdSaleDetail
    {
        get => _IdSaleDetail;
        private set => _IdSaleDetail = value;
    }
    public SaleDetail(int idSaleDetail, int transactionId, int productId, int quantity, decimal unitPrice)
        : base(idSaleDetail, transactionId, productId, quantity, unitPrice)
    {
        this.IdSaleDetail = idSaleDetail;
    }
}