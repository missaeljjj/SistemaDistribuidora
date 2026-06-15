using System;
using System.Collections.Generic;
namespace SistemaDistribuidora.Models;

public class Purchase : BaseTransaction
{
    private int _IdPurchase;
    private int _SupplierId;

    public int IdPurchase
    {
        get => _IdPurchase;
        private set => _IdPurchase = value;
    }
    public int SupplierId
    {
        get => _SupplierId;
        private set => _SupplierId = value;
    }


    public Purchase(int idPurchase, int supplierId, int employeeId, DateTime date, decimal totalAmount, IEnumerable<PurchaseDetail> PurchaseCart)
        : base(idPurchase, employeeId, date, totalAmount, PurchaseCart)
    {
        this.IdPurchase = idPurchase;
        this.SupplierId = supplierId;

    }
}

public class PurchaseDetail : BaseTransactionDetail
{
    private int _IdPurchaseDetail;
    public int IdPurchaseDetail
    {
        get => _IdPurchaseDetail;
        private set => _IdPurchaseDetail = value;
    }
    public PurchaseDetail(int idPurchaseDetail, int transactionId, int productId, int quantity, decimal unitPrice)
        : base(idPurchaseDetail, transactionId, productId, quantity, unitPrice)
    {
        this.IdPurchaseDetail = idPurchaseDetail;
    }
}