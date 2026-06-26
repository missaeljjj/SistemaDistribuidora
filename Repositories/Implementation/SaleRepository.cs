using Dapper;
using SistemaDistribuidora.Exceptions;
using SistemaDistribuidora.Models;
using SistemaDistribuidora.Repositories.DataBaseConnection;
using SistemaDistribuidora.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;



namespace SistemaDistribuidora.Repositories.Implementation;

public class SaleRepository : ISaleRepository

{

    private readonly IDataBase _DataBase;

    public SaleRepository(IDataBase dataBase)

    {
        _DataBase = dataBase;
    }

    #region Implementation
    public async Task CreateNewSaleAsync(Sale sale)

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
            var CartTable = new DataTable();
            CartTable.Columns.Add("ProductId", typeof(int));
            CartTable.Columns.Add("Quantity", typeof(int));
            CartTable.Columns.Add("SaleUnitPrice", typeof(decimal));

            foreach (var item in sale.Cart)
            {
                CartTable.Rows.Add(item.ProductId, item.Quantity, item.UnitPrice);
            }

            var parameters = new DynamicParameters();
            parameters.Add("@EmployeeId", sale.EmployeeId);
            parameters.Add("@CustomerId", sale.CustomerId);

            parameters.Add("@Cart", CartTable.AsTableValuedParameter("SalesCart"));

            await Connection.ExecuteAsync(
                "sp_CreateSale",
                parameters,
                commandType: CommandType.StoredProcedure
            );

        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("sp_CreateSale", " Error al crear una venta nueva", ex);
        }
    }


#endregion

    #region PRIVATE MAPPERS

    private class SaleMap
    {
        public int SaleId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = "";
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = "";
        public decimal Total { get; set; }
        public DateTime SaleDate { get; set; }
        public string SaleStatus { get; set; } = "";

        public IEnumerable<SaleDetail> SaleCart = new List<SaleDetail>();

        public Sale ToSale() => new Sale(
            idSale: SaleId,
            employeeId: EmployeeId,
            customerId: CustomerId,
            totalAmount: Total,
            date: SaleDate,
            Status: SaleStatus,
            SaleCart: SaleCart

        );
    }
    #endregion

}