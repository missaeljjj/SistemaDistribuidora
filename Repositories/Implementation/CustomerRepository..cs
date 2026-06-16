using Dapper;
using SistemaDistribuidora.DTOs;
using SistemaDistribuidora.Exceptions;
using SistemaDistribuidora.Mappers;
using SistemaDistribuidora.Models;
using SistemaDistribuidora.Repositories.DataBaseConnection;
using SistemaDistribuidora.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Repositories.Implementation;

public class CustomerRepository  : ICustomerRepository
{
    private readonly IDataBase _DateBase;

    //por implementar...
    public CustomerRepository(IDataBase database)
    {
        _DateBase = database;
    }

    #region REPOSITORY IMPLEMENTATION
    public async Task InsertAsync(Customer customer)
    {
        await using var connection = await _DateBase.GetConnectionAsync();

        try
        {
            await connection.ExecuteAsync(
               "sp_InsertCustomer",
               new
               {
                   FullName = customer.FullName,
                   TypeOfPerson = customer.TypeOfPerson,
                   IdentityCard = customer.IdentityCard,
                   Address = customer.Address,
                   Phone = customer.Phone
                   
               },
               commandType: CommandType.StoredProcedure
           );

        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("sp_InsertNewCustomer", "Error  al insertar cliente",ex);
        }
    }

    public async Task UpdateAsync(Customer customer)
    {
        await using var Connection = await _DateBase.GetConnectionAsync();

        try
        {
            await Connection.ExecuteAsync
                (
                "sp_UpdateCustomer",
                new
                {
                    FullName = customer.FullName,
                    TypeOfPerson = customer.TypeOfPerson,
                    IdentityCard = customer.IdentityCard,
                    Address = customer.Address,
                    Phone = customer.Phone
                },
                commandType: CommandType.StoredProcedure
                );
                
        }
        catch(Exception ex)
        {
            throw new DataBaseOperationException("sp_UpdateCustomer", "Error al actualizar", ex);
        }
    }

    public async Task DeleteAsync(int customerId)
    {
        await using var Connection = await _DateBase.GetConnectionAsync();

        try
        {
            await Connection.ExecuteAsync
                (
                    "sp_DeleteCustomer",
                    new
                    {
                        Id = customerId
                    },
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
        await using var Connection = await _DateBase.GetConnectionAsync();

        try
        {
            var row = await Connection.QuerySingleOrDefaultAsync<CustomerMap>(
                "sp_GetCustomerById",
                new { IdCustomer = customerId },
                commandType: CommandType.StoredProcedure
                );
            if (row == null)
                throw new EntityNotFoundException("Cliente", customerId);

            return row.ToCustomer();
        }
        catch(Exception ex ) 
        {
            throw new DataBaseOperationException("sp_GetCustomerById", "Error al buscar", ex);
        }
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        await using var Connection = await _DateBase.GetConnectionAsync();

        try
        {
            const string Sql = "SELECT * FROM vw_AllCustomers";
            var rows = await Connection.QueryAsync<CustomerDetailMap>(Sql);

            return rows.Select(r => r.ToCustomerDetail());
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("Select * vw_GetAll","Error al obtener", ex);
        }

    }

    public async Task<IEnumerable<CustomerDetailDto>> GetAllWithQuantityOfPurchases()
    {
        await using var Connection = await _DateBase.GetConnectionAsync();

        const string Sql = "SELECT * FROM vw_AllCustumerWithPurchases";

        var rows = await Connection.QueryAsync<CustomerWithCountMap>(Sql);

        return rows.Select(r => r.ToDetailDto());
    }
    #endregion

    #region Mapper privados
    private class CustomerMap
    {
        public int IdPerson { get; set; }
        public int IdCustomer { get; set; }
        public string FullName { get; set; } = "";
        public string TypeOfPerson { get; set; } = "";
        public string IdentityCard { get; set; } = "";
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public DateTime RegisterDate { get; set; }
        public bool Status { get; set; }

        public Customer ToCustomer() => new Customer(
            idperson: IdPerson,
            fullname: FullName,
            typeofperson: TypeOfPerson,
            identitycard: IdentityCard,
            address: Address,
            phone: Phone,
            registerdate: RegisterDate,
            status: Status,
            idcustomer: IdCustomer
        );
    }

    private class CustomerDetailMap
    {
        public int IdCustomer { get; set; }
        public string FullName { get; set; } = "";
        public string IdentityCard { get; set; } = "";
        public string TypeofPerson { get; set; } = "";  
        public string? Address { get; set; } 
        public string? Phone { get; set; } 
        public DateTime RegisterDate { get; set; }
        public bool Status { get; set; }

        public Customer ToCustomerDetail() => new Customer
            (
                fullname: FullName,
                typeofperson: TypeofPerson,
                identitycard: IdentityCard,
                address: Address!,
                phone: Phone!,
                registerdate: RegisterDate,
                status: Status,
                idcustomer: IdCustomer
            );
    }
    

    // Map extendido: agrega QuantityOfPurchases para sp_GetCustomersWithPurchaseCount
    // Hereda CustomerMap para no repetir las columnas base
    private class CustomerWithCountMap : CustomerDetailMap
    {
        public int QuantityOfPurchases { get; set; }

        // ToDetailDto usa CustomerMapper — el campo calculado se pasa como parámetro
        public CustomerDetailDto ToDetailDto() =>
            ToCustomerDetail().ToDetailDto(quantityofpurchases: QuantityOfPurchases);
    }
}

#endregion 


