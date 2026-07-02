using Dapper;
using Microsoft.Data.SqlClient;
using SistemaDistribuidora.Exceptions;
using SistemaDistribuidora.Models;
using SistemaDistribuidora.Repositories.DataBaseConnection;
using SistemaDistribuidora.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

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
                    CategoryId  = product.CategoryId,
                    SupplierId  = product.SupplierId,
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
        await using var connection = await _DataBase.GetConnectionAsync();

        try
        {
            await connection.ExecuteAsync(
                "sp_UpdateProduct",
                new
                {
                    ProductID   = product.IdProduct,
                    ProductName = product.Name,
                    CategoryId  = product.CategoryId,
                    SupplierId  = product.SupplierId,
                    Status      = product.Status,
                },
                commandType: CommandType.StoredProcedure
            );
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("sp_UpdateProduct", "Error al actualizar el producto", ex);
        }

   
    }

    public async Task DeleteAsync(int productId)
    {
        await using var connection = await _DataBase.GetConnectionAsync();

        try
        {
            await connection.ExecuteAsync(
                "sp_DeleteProduct",
                new { Id = productId },
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
        DbConnection connection;
        try
        {
            connection = await _DataBase.GetConnectionAsync();
        }
        catch (Exception ex)
        {
            throw new ConnectionException("Error en conexion base de datos", ex);
        }

        try
        {
            const string sql = "SELECT * FROM vw_AllProducts";
            var rows = await connection.QueryAsync<ProductMap>(sql);
            return rows.Select(r => r.ToProduct());
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("vw_AllProducts", "Error al obtener la lista de productos", ex);
        }
    }

    public async Task<Product> GetByIdAsync(int productId)
    {
        DbConnection connection;
        try
        {
            connection = await _DataBase.GetConnectionAsync();
        }
        catch (Exception ex)
        {
            throw new ConnectionException("Error en conexion base de datos", ex);
        }

        try
        {
            var row = await connection.QuerySingleOrDefaultAsync<ProductMap>(
                "sp_GetProductById",
                new { ProductId = productId },
                commandType: CommandType.StoredProcedure
            );

            if (row == null)
                throw new EntityNotFoundException("Producto", productId);

            return row.ToProduct();
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("sp_GetProductById", "Error al buscar el producto", ex);
        }
    }

    public async Task<IEnumerable<(Product product, string SupplierName, string CategoryName)>> GetAllProductsInInventoryAsync()
    {
        DbConnection connection;
        try
        {
            connection = await _DataBase.GetConnectionAsync();
        }
        catch (Exception ex)
        {
            throw new ConnectionException("Error en conexion base de datos", ex);
        }

        try
        {
            const string sql = "SELECT * FROM vw_AllProductsInInventory";
            var rows = await connection.QueryAsync<InventoryMap>(sql);
            return rows.Select(r => r.ToTuple());
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("vw_AllProductsInInventory", "Error al obtener productos en inventario", ex);
        }
    }

    public async Task<IEnumerable<(Product product, int QuantityOfSales)>> GetAllProductsWithQuantityOfSales()
    {
        DbConnection connection;
        try
        {
            connection = await _DataBase.GetConnectionAsync();
        }
        catch (Exception ex)
        {
            throw new ConnectionException("Error en conexion base de datos", ex);
        }

        try
        {
            const string sql = "SELECT * FROM vw_ProductDetail";
            var rows = await connection.QueryAsync<ProductWithSalesMap>(sql);
            return rows.Select(r => r.ToTuple());
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("vw_ProductDetail", "Error al obtener productos con cantidad de ventas", ex);
        }
    }

    public async Task<bool> ExistingProductWithCategory(int categoryId)
    {
        DbConnection connection;
        try
        {
            connection = await _DataBase.GetConnectionAsync();
        }
        catch (Exception ex)
        {
            throw new ConnectionException("Error en conexion base de datos", ex);
        }

        try
        {
            const string sql = @"
                SELECT CASE 
                    WHEN EXISTS (SELECT 1 FROM Product WHERE CategoryId = @CategoryId) 
                    THEN 1 ELSE 0 
                END";

            var result = await connection.ExecuteScalarAsync<int>(sql, new { CategoryId = categoryId });
            return result == 1;
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("Products", $"Error al verificar productos de la categoría {categoryId}", ex);
        }
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        DbConnection connection;
        try
        {
            connection = await _DataBase.GetConnectionAsync();
        }
        catch (Exception ex)
        {
            throw new ConnectionException("Error en conexion base de datos", ex);
        }

        try
        {
            const string sql = @"
                SELECT CASE 
                    WHEN EXISTS (SELECT 1 FROM Product WHERE LOWER(TRIM(Name)) = LOWER(TRIM(@Name))) 
                    THEN 1 ELSE 0 
                END";

            var result = await connection.ExecuteScalarAsync<int>(sql, new { Name = name });
            return result == 1;
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("Product", "Error al verificar nombre de producto", ex);
        }
    }

    public async Task<bool> ExistsByNameExcludedAsync(string name, int idToExclude)
    {
        DbConnection connection;
        try
        {
            connection = await _DataBase.GetConnectionAsync();
        }
        catch (Exception ex)
        {
            throw new ConnectionException("Error en conexion base de datos", ex);
        }

        try
        {
            const string sql = @"
                SELECT CASE 
                    WHEN EXISTS (SELECT 1 FROM Product WHERE LOWER(TRIM(Name)) = LOWER(TRIM(@Name)) AND ProductId <> @IdToExclude) 
                    THEN 1 ELSE 0 
                END";

            var result = await connection.ExecuteScalarAsync<int>(sql, new { Name = name, IdToExclude = idToExclude });
            return result == 1;
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("Product", "Error al verificar nombre de producto", ex);
        }
    }

    public async Task<bool> UpdateProductPricesAsync(int productId, decimal newSalePrice)
    {
        DbConnection connection;
        try
        {
            connection = await _DataBase.GetConnectionAsync();
        }
        catch (Exception ex)
        {
            throw new ConnectionException("Error en conexion base de datos", ex);
        }

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ProductId", productId, DbType.Int32);
            parameters.Add("@NewSalePrice", newSalePrice, DbType.Decimal, precision: 10, scale: 2);

            int rowsAffected = await connection.ExecuteAsync(
                "sp_UpdateProductPrices",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("Product", "Error al actualizar", ex);
        }
    }

    #endregion

    #region PRIVATE MAPPERS

    // Mapper base — usado por GetAllAsync y GetByIdAsync
    private class ProductMap
    {
        public int ID     { get; set; }
        public string ProductName { get; set; } = "";
        public int Stock          { get; set; }
        public int CategoryId     { get; set; }
        public int SupplierId     { get; set; }
        public decimal SalePrice  { get; set; }
        public decimal PurchasePrice { get; set; }
        public bool Status        { get; set; }

        public Product ToProduct() => new Product(
            idProduct:     ID,
            name:          ProductName,
            stock:         Stock,
            categoryId:    CategoryId,
            SupplierId:    SupplierId,
            salePrice:     SalePrice,
            purchasePrice: PurchasePrice,
            status:        Status
        );
    }

    // Hereda ProductMap y agrega cantidad de ventas — usado por GetAllProductsWithQuantityOfSales
    private class ProductWithSalesMap : ProductMap
    {
        public int QuantityOfSale { get; set; }

        public (Product product, int QuantityOfSales) ToTuple() =>
            (ToProduct(), QuantityOfSale);
    }

    // Hereda ProductMap y agrega nombres de proveedor y categoría — usado por GetAllProductsInInventoryAsync
    private class InventoryMap : ProductMap
    {
        public string SupplierName { get; set; } = "";
        public string CategoryName { get; set; } = "";

        public (Product product, string SupplierName, string CategoryName) ToTuple() =>
            (ToProduct(), SupplierName, CategoryName);
    }

    #endregion
}