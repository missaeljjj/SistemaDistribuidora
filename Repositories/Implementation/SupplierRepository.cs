using Dapper;
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

namespace SistemaDistribuidora.Repositories.Implementation;

public class SupplierRepository : ISuplierRepository
{
    private readonly IDataBase _DataBase;

    public SupplierRepository(IDataBase dataBase)
    {
        _DataBase = dataBase;
    }

    #region REPOSITORY IMPLEMENTATION

    public async Task InsertAsync(Supplier supplier)
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
            await Connection.ExecuteAsync(
                "sp_CreateNewSupplier",
                new
                {
                    FullName = supplier.FullName,
                    Identification = supplier.IdentityCard,
                    TypePerson = supplier.TypeOfPerson,
                    Address = supplier.Address,
                    Phone = supplier.Phone
                },
                commandType: CommandType.StoredProcedure
            );
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("sp_CreateNewSupplier", "Error al insertar el nuevo proveedor", ex);
        }
    }

    public async Task UpdateAsync(Supplier supplier)
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
            await Connection.ExecuteAsync(
                "sp_UpdateSupplier",
                new
                {
                    SupplierId = supplier.Id,
                    FullName = supplier.FullName,
                    Identification = supplier.IdentityCard,
                    TypePerson = supplier.TypeOfPerson,
                    Address = supplier.Address,
                    Phone = supplier.Phone,
                    Status = supplier.Status
                },
                commandType: CommandType.StoredProcedure
            );
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("sp_UpdateSupplier", "Error al actualizar proveedor", ex);
        }
    }

    public async Task DeleteAsync(int SupplierId)
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
            await Connection.ExecuteAsync(
                "sp_DeleteSupplier",
                new { SupplierId = SupplierId },
                commandType: CommandType.StoredProcedure
            );
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("sp_DeleteSupplier", "Error al eliminar proveedor", ex);
        }
    }

    public async Task<Supplier> GetByIdAsync(int SupplierId)
    {
        await using var Connection = await _DataBase.GetConnectionAsync();

        try
        {
            var row = await Connection.QuerySingleOrDefaultAsync<SupplierMap>(
                "sp_GetSupplierByID",
                new { SupplierId = SupplierId },
                commandType: CommandType.StoredProcedure
            );

            if (row == null)
                throw new EntityNotFoundException("Proveedor", SupplierId);

            return row.ToSupplier();
        }
        catch (EntityNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("sp_GetSupplierByID", "Error al obtener proveedor", ex);
        }
    }

    public async Task<IEnumerable<Supplier>> GetAllAsync()
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
            const string Sql = "SELECT * FROM vw_AllSuppliers";
            var rows = await Connection.QueryAsync<SupplierDetailMap>(Sql);

            return rows.Select(r => r.ToSupplierDetail());
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("vw_AllSuppliers", "Error al obtener proveedores", ex);
        }
    }

    public async Task<IEnumerable<(Supplier supplier,int QuantityOfPurchases,int QuantityOfProdcutsProvide)>> GetAllSuppliersSummary()
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
            const string Sql = "SELECT * FROM vw_SupplierPurchaseSummary";
            var rows = await Connection.QueryAsync<SupplierFullDetailMap>(Sql);

            return rows.Select(r => r.ToTupple());
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("vw_SupplierPurchaseSummary", "Error al obtener el resumen de compra del proveedor", ex);
        }
    }

        public async Task<bool> SupplierExisting(string Name)
    {
        DbConnection connection;
        try
        {
            connection = await _DataBase.GetConnectionAsync();
        }    
        catch(Exception e)
        {
            throw new ConnectionException("Error en conexion base de datos", e);
        }    

        try
        {
            const string Sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM Supplier S INNER JOIN Person P ON S.PersonId = P.PersonId WHERE p.Identification = @Identification) THEN 1 ELSE 0 END AS result";

            var result = await connection.ExecuteScalarAsync<int>(Sql, new { Identification = Name }); 

            return result == 1;


        }
        catch(Exception e)
        {
            throw new DataBaseOperationException("Command", "Error al obtener", e);
        }
    }

        public async Task<bool> SupplierExistingForUpdate(string Name,int id)
    {
        DbConnection connection;
        try
        {
            connection = await _DataBase.GetConnectionAsync();
        }    
        catch(Exception e)
        {
            throw new ConnectionException("Error en conexion base de datos", e);
        }    

        try
        {
            const string Sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM Supplier S INNER JOIN Person P ON P.PersonId = S.PersonId WHERE p.Identification = @Identification AND S.SupplierId != @SupplierId) THEN 1 ELSE 0 END AS result";

            var result = await connection.ExecuteScalarAsync<int>(Sql, new { Identification = Name, SupplierId = id });

            return result == 1;


        }
        catch(Exception e)
        {
            throw new DataBaseOperationException("Command", "Error al obtener", e);
        }
    }

    #endregion

    #region Mappers Privados

    private class SupplierMap
    {
        public int SupplierId { get; set; }
        public string FullName { get; set; } = "";
        public string TypeOfPerson { get; set; } = "";
        public string Identification { get; set; } = "";
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public DateTime RegisterDate { get; set; }
        public bool Status { get; set; } 

        public Supplier ToSupplier() => new Supplier
            (
            idsupplier: SupplierId,
            fullname: FullName,
            typeofperson: TypeOfPerson,
            identitycard: Identification,
            address: Address,
            phone: Phone,
            registerdate: RegisterDate,
            status: Status
            );
    }

    private class SupplierDetailMap
    {
        public int SupplierId { get; set; }
        public string FullName { get; set; } = "";
        public string TypeOfPerson { get; set; } = "";
        public string Identification { get; set; } = "";
        public string? Address { get; set; } = "";
        public string? Phone { get; set; } = "";
        public DateTime RegisterDate { get; set; }
        public string Status { get; set; } = "";

        public Supplier ToSupplierDetail() => new Supplier
            (
            idsupplier: SupplierId,
            fullname: FullName,
            typeofperson: TypeOfPerson,
            identitycard: Identification,
            address: Address!,
            phone: Phone!,
            registerdate: RegisterDate,
            status: Status == "Activo"
            );
    }

    private class SupplierFullDetailMap : SupplierDetailMap
    {
        public int QuantityOfPurchases { get; set; }

        public int QuantityOfProdcutsProvide { get; set; }

        public (Supplier supplier, int QuantityOfPurchases, int QuantityOfProdcutsProvide) ToTupple()
            => (ToSupplierDetail(), QuantityOfPurchases, QuantityOfProdcutsProvide);
    }


    #endregion

}