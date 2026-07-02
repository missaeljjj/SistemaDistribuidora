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

public class CustomerRepository : ICustomerRepository
{
    private readonly IDataBase _DataBase;

    public CustomerRepository(IDataBase database)
    {
        _DataBase = database;
    }

    #region REPOSITORY IMPLEMENTATION
    public async Task InsertAsync(Customer customer)
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
               "sp_CreateNewCustomer",
               new
               {
                   FullName = customer.FullName,
                   Identification = customer.IdentityCard,
                   TypePerson = customer.TypeOfPerson,
                   Address = customer.Address,
                   Phone = customer.Phone
               },
               commandType: CommandType.StoredProcedure
           );
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("sp_CreateNewCustomer", "Error al insertar cliente", ex);
        }
    }

    public async Task UpdateAsync(Customer customer)
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
                "sp_UpdateCustomer",
                new
                {
                    CustomerId = customer.IdCustomer, 
                    FullName = customer.FullName,
                    Identification = customer.IdentityCard,
                    TypePerson = customer.TypeOfPerson,
                    Address = customer.Address,
                    Phone = customer.Phone,
                    Status = customer.Status
                },
                commandType: CommandType.StoredProcedure
            );
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("sp_UpdateCustomer", "Error al actualizar", ex);
        }
    }

    public async Task DeleteAsync(int customerId)
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
                "sp_DeleteCustomer",
                new { CustomerId = customerId },
                commandType: CommandType.StoredProcedure
            );
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("sp_DeleteCustomer", "Error al eliminar", ex);
        }
    }

    public async Task<Customer> GetByIdAsync(int customerId)
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
            var row = await Connection.QuerySingleOrDefaultAsync<CustomerMap>(
                "sp_GetCustomerByID",
                new { CustomerId = customerId },
                commandType: CommandType.StoredProcedure
            );

            if (row == null)
                throw new EntityNotFoundException("Cliente", customerId);

            return row.ToCustomer();
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("sp_GetCustomerByID", "Error al buscar", ex);
        }
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
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
            const string sql = "SELECT * FROM vw_AllCustomers";
            var rows = await Connection.QueryAsync<CustomerDetailMap>(sql);

            return rows.Select(r => r.ToCustomerDetail());
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("vw_AllCustomers", "Error al obtener", ex);
        }
    }

    public async Task<IEnumerable<(Customer customer, int QuantityOfPurchases)>> GetAllWithQuantityOfPurchases()
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
            const string sql = "SELECT * FROM vw_AllCustomerWithPurchases";
            var rows = await Connection.QueryAsync<CustomerWithCountMap>(sql);

            return rows.Select(r => r.ToTupple());
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("vw_AllCustomerWithPurchases", "Error al obtener", ex);
        }
    }

    public async Task<bool> CustomerExisting(string Identity)
    {
        DbConnection connection;
        try
        {
            connection = await _DataBase.GetConnectionAsync();
        }
        catch (Exception e)
        {
            throw new ConnectionException("Error en conexion base de datos", e);
        }

        try
        {
            const string Sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM Customer C INNER JOIN Person P ON C.PersonId = P.PersonId WHERE P.Identification = @Identification) THEN 1 ELSE 0 END AS result";

            var result = await connection.ExecuteScalarAsync<int>(Sql, new { Identification = Identity });

            return result == 1;
        }
        catch (Exception e)
        {
            throw new DataBaseOperationException("Command", "Error al obtener", e);
        }
    }

    public async Task<bool> CustomerExistingForUpdate(string Name, int id)
    {
        DbConnection connection;
        try
        {
            connection = await _DataBase.GetConnectionAsync();
        }
        catch (Exception e)
        {
            throw new ConnectionException("Error en conexion base de datos", e);
        }

        try
        {
           
            const string Sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM Customer C INNER JOIN Person P ON C.PersonId = P.PersonId WHERE P.Identification = @Identification AND C.CustomerId != @CustomerId) THEN 1 ELSE 0 END AS result";

            var result = await connection.ExecuteScalarAsync<int>(Sql, new { Identification = Name, CustomerId = id });

            return result == 1;
        }
        catch (Exception e)
        {
            throw new DataBaseOperationException("Command", "Error al obtener", e);
        }
    }
    #endregion

    #region Mapper privados
    private class CustomerMap
    {
       
        public int CustomerId { get; set; }
        public string FullName { get; set; } = "";
        public string TypeOfPerson { get; set; } = "";
        public string Identification { get; set; } = "";
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public DateTime RegisterDate { get; set; }
        public string Status { get; set; } = "";

        public Customer ToCustomer() => new Customer(
            idcustomer: CustomerId,
            fullname: FullName,
            typeofperson: TypeOfPerson,
            identitycard: Identification,
            address: Address,
            phone: Phone,
            registerdate: RegisterDate,
            status: Status == "Activo"
        );
    }

    private class CustomerDetailMap
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; } = "";
        public string Identification { get; set; } = "";
        public string TypeOfPerson { get; set; } = "";
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public DateTime RegisterDate { get; set; }
        public string Status { get; set; } = "";

        public Customer ToCustomerDetail() => new Customer(
            idcustomer: CustomerId,
            fullname: FullName,
            typeofperson: TypeOfPerson,
            identitycard: Identification,
            address: Address!,
            phone: Phone!,
            registerdate: RegisterDate,
            status: Status == "Activo"
        );
    }
    private class CustomerWithCountMap : CustomerDetailMap
    {
        public int QuantityOfPurchases { get; set; }

        public (Customer customer, int QuantityOfPurchases) ToTupple() =>
            (ToCustomerDetail(), QuantityOfPurchases);
    }
    #endregion
}