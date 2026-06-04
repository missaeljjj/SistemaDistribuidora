using System;
namespace SistemaDistribuidora.Models;

class Producto 
{
    private int _IdProduct;
    private string _Name = "";
    private int _Stock;
    private int _CategoryId;
    private decimal _SalePrice;
    private decimal _PurchasePrice;
    private bool _Status = true;

    public int IdProduct
    {
        get => _IdProduct;
        private set => _IdProduct = value;
    }
    public string Name 
    {
        get => _Name;
        private set => _Name =!string.IsNullOrWhiteSpace(value) ? value.Trim() : throw new ArgumentException("El nombre del producto no puede estar vacío");
    }

    public int Stock
    {
        get => _Stock;
        private set => _Stock = value > 0 ? value : throw new ArgumentException("El stock no puede ser negativo");
    }

    public int CategoryId
    {
        get => _CategoryId;
        private set => _CategoryId = value;
    }

    public decimal SalePrice
    {
        get => _SalePrice;
        private set => _SalePrice =  value > 0 ? value : throw new ArgumentException("El precio de venta debe ser mayor a cero") ;
    }

    public decimal PurchasePrice
    {
        get => _PurchasePrice;
        private set => _PurchasePrice = value > 0 ? value : throw new ArgumentException("El precio de compra debe ser mayor a cero");
    }

    public bool Status 
    {
        get => _Status;
        private set => _Status = value;
    }

    public Producto(int idProduct, string name,int stock, int categoryId, decimal salePrice, decimal purchasePrice, bool status)
    {
        this.IdProduct = idProduct;
        this.Name = name;
        this.Stock = stock;
        this.CategoryId = categoryId;
        this.SalePrice = salePrice;
        this.PurchasePrice = purchasePrice;
        this.Status = status;
    }
}