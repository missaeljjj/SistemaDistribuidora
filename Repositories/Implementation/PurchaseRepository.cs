using Dapper;
using SistemaDistribuidora.Exceptions;
using SistemaDistribuidora.Models;
using SistemaDistribuidora.Repositories.DataBaseConnection;
using SistemaDistribuidora.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
namespace SistemaDistribuidora.Repositories.Implementation;

public class PurchaseRepository : IPurchaseRepository
{
    private readonly IDataBase _DataBase;

    public PurchaseRepository(IDataBase dataBase)
    {
        _DataBase = dataBase;
    }


    #region RepositoryImplementation
    public async Task CreateNewPurchaseAsync(Purchase purchase)
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
            CartTable.Columns.Add("PurchaseUnitPrice", typeof(int));

            foreach (var item in purchase.Cart)
            {
                CartTable.Rows.Add(item.ProductId, item.Quantity, item.UnitPrice);
            }

            var parameters = new DynamicParameters();
            parameters.Add("@EmployeeId", purchase.EmployeeId);
            parameters.Add("@SupplierID", purchase.SupplierId);

            parameters.Add("@Cart", CartTable.AsTableValuedParameter("PurchaseCart"));

            await Connection.ExecuteAsync(
                "sp_CreatePurchase",
                parameters,
                commandType: CommandType.StoredProcedure
            );

        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("sp_CreatePurchase", " Error al crear una compra nueva", ex);
        }

    }

    

#endregion

    #region PRIVATE MAPPERS

    private class PurchaseMap
    {
        public int PurchaseId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = "";
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = "";
        public decimal Total { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string PurchaseStatus { get; set; } = "";

        public IEnumerable<PurchaseDetail> PurchaseCart { get; set; } = new List<PurchaseDetail>();

        public Purchase ToPurchase() => new Purchase(
            idPurchase: PurchaseId,
            employeeId: EmployeeId,
            supplierId: SupplierId,
            totalAmount: Total,
            date: PurchaseDate,
            status: PurchaseStatus,
            PurchaseCart: PurchaseCart

        );
    }

    #endregion
}