using Dapper;
using SistemaDistribuidora.Exceptions;
using SistemaDistribuidora.Models;
using SistemaDistribuidora.Repositories.DataBaseConnection;
using SistemaDistribuidora.Repositories.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Repositories.Implementation;

public class ProductRepository : IProductRepository
{
    private readonly IDataBase _DataBase;

    public ProductRepository(IDataBase dataBase)
    {
        _DataBase = dataBase;
    }

    #region REPOSITORY IMPLEMENTATION
    public async Task InsertAsync(Product product)
    {
        await using var connection = await _DataBase.GetConnectionAsync();

        try
        {
            await connection.ExecuteAsync(
                "sp_CreateNewProduct",
                new
                {
                    ProductName = product.Name,
                    CategoryId = product.CategoryId,
                    SupplierId = product.SupplierId,
                },
                commandType: CommandType.StoredProcedure
            );
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("sp_CreateNewProduct", "Error al ingresar un nuevo producto", ex);
        }
    }

    public async Task UpdateAsync(Product product)
    {
        await using var Connection = await _DataBase.GetConnectionAsync();

        try
        {
            await Connection.ExecuteAsync(
                "sp_UpdateProduct",
                new
                {
                    ProductId = product.IdProduct,
                    ProductName = product.Name,
                    CategoryId = product.CategoryId,
                    SupplierId = product.SupplierId,
                    Status = product.Status,
                },
                commandType: CommandType.StoredProcedure
            );
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("sp_UpdateProduct", "Ha ocurrido un error al actualizar", ex);
        }

        try
        {
            await Connection.ExecuteAsync(
                "sp_UpdateProductPrices",
                new
                {
                    ProductId = product.IdProduct,
                    NewSalePrice = product.SalePrice,
                },
                commandType: CommandType.StoredProcedure
            );
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("sp_UpdateProductPrices", "Error al ingresar el nuevo valor", ex);
        }
    }

    public async Task DeleteAsync(int ProductId)
    {
        await using var Connection = await _DataBase.GetConnectionAsync();
        try
        {
            await Connection.ExecuteAsync(
                "sp_DeleteProduct",
                new
                {
                    Id = ProductId
                },
                commandType: CommandType.StoredProcedure
            );
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("sp_DeleteProduct", "Error al eliminar producto", ex);
        }
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        DbConnection Connection;
        try
        {
            Connection = await _DataBase.GetConnectionAsync();
        }
        catch (Exception ex)
        {
            throw new ConnectionException("Error en conexion base de datos", ex);
        }

        try
        {
            const string Sql = "SELECT * FROM vw_AllProducts";
            var rows = await Connection.QueryAsync<ProductDetailMap>(Sql);

            return rows.Select(r => r.ToProductDetail());
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("vw_AllProducts", "Error al obtener la lista de productos", ex);
        }
    }

    public async Task<Product> GetByIdAsync(int ProductId)
    {
        DbConnection Connection;
        try
        {
            Connection = await _DataBase.GetConnectionAsync();
        }
        catch (Exception ex)
        {
            throw new ConnectionException("Error en conexion base de datos", ex);
        }

        try
        {
            var row = await Connection.QuerySingleOrDefaultAsync<ProductMap>(
                "sp_GetProductById",
                new { IdProduct = ProductId },
                commandType: CommandType.StoredProcedure
            );

            if (row == null)
                throw new EntityNotFoundException("Producto", ProductId);

            return row.ToProduct();
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("sp_GetProductById", "Error al buscar el producto", ex);
        }
    }

    public async Task<IEnumerable<Product>> GetAllProductsInInventoryAsync()
    {
        DbConnection Connection;
        try
        {
            Connection = await _DataBase.GetConnectionAsync();
        }
        catch (Exception ex)
        {
            throw new ConnectionException("Error en conexion base de datos", ex);
        }

        try
        {
            const string Sql = "SELECT * FROM vw_AllProductsInInventory";
            var rows = await Connection.QueryAsync<ProductMap>(Sql);

            return rows.Select(r => r.ToProduct());
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("vw_AllProductsInInventory", "Error al obtener los productos en inventario", ex);
        }
    }

    public async Task<IEnumerable<Product>> GetAllProductsWithQuantityOfSales()
    {
        DbConnection Connection;
        try
        {
            Connection = await _DataBase.GetConnectionAsync();
        }
        catch (Exception ex)
        {
            throw new ConnectionException("Error en conexion base de datos", ex);
        }

        try
        {
            const string Sql = "SELECT * FROM vw_ProductDetail";
            var rows = await Connection.QueryAsync<ProductMap>(Sql);

            return rows.Select(r => r.ToProduct());
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("vw_ProductDetail", "Error al obtener productos con cantidad de ventas", ex);
        }
    }
    #endregion

    #region PRIVATE MAPPERS
    private class ProductMap
    {
        public int IdProduct { get; set; }
        public int Stock { get; set; }
        public string ProductName { get; set; } = "";
        public int CategoryId { get; set; }
        public int SupplierId { get; set; }
        public bool Status { get; set; }
        public decimal SalePrice { get; set; }

        public decimal PurchasePrice { get; set; }

        public Product ToProduct() => new Product(
            idProduct: IdProduct,
            stock: Stock,
            name: ProductName,
            categoryId: CategoryId,
            SupplierId: SupplierId,
            status: Status,
            salePrice: SalePrice,
            purchasePrice: PurchasePrice

        );
    }

    private class ProductDetailMap
    {
        public int IdProduct { get; set; }
        public int Stock { get; set; }
        public string ProductName { get; set; } = "";
        public int CategoryId { get; set; }
        public int SupplierId { get; set; }
        public bool Status { get; set; }
        public decimal SalePrice { get; set; }
        public decimal PurchasePrice { get; set; }

        public Product ToProductDetail() => new Product(
            idProduct: IdProduct,
            stock: Stock,
            name: ProductName,
            categoryId: CategoryId,
            SupplierId: SupplierId,
            status: Status,
            salePrice: SalePrice,
            purchasePrice: PurchasePrice
        );
    }

    private class ProductWithCountMap : ProductDetailMap
    {
        public int QuantityOfSale { get; set; }

        public (Product product, int quantityofsales) ToDetailDto() =>
            (ToProductDetail(),quantityofsales: QuantityOfSale);
    }
    #endregion
}