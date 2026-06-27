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
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Repositories.Implementation;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly IDataBase _DataBase;

    public EmployeeRepository(IDataBase database)
    {
        _DataBase = database;
    }

    #region REPOSITORY IMPLEMENTATION
    public async Task InsertAsync(Employee employee)
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
            await connection.ExecuteAsync(
               "sp_CreateNewEmployee",
               new
               {
                   FullName = employee.FullName,
                   TypeOfPerson = employee.TypeOfPerson,
                   Identification = employee.IdentityCard,
                   Address = employee.Address,
                   Phone = employee.Phone,
               },
               commandType: CommandType.StoredProcedure
           );

        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("sp_CreateNewEmployee", "Error al insertar un nuevo empleado", ex);
        }
    }

    public async Task UpdateAsync(Employee employee)
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
            await Connection.ExecuteAsync
                (
                "sp_UpdateEmployee",
                new
                {
                    PersonId = employee.Id,
                    FullName = employee.FullName,
                    TypeOfPerson = employee.TypeOfPerson,
                    Identification = employee.IdentityCard,
                    Address = employee.Address,
                    Phone = employee.Phone,
                    PersonStatus = employee.Status
                },
                commandType: CommandType.StoredProcedure
                );

        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("sp_UpdateEmployee", "Error al actualizar", ex);
        }
    }

    public async Task DeleteAsync(int employeeId)
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
            await Connection.ExecuteAsync
                (
                    "sp_DeleteEmployee",
                    new
                    {
                        Id = employeeId
                    },
                    commandType: CommandType.StoredProcedure
                );
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("sp_DeleteEmployee", "Error al eliminar", ex);
        }
    }

    public async Task<Employee> GetByIdAsync(int employeeId)
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
            var row = await Connection.QuerySingleOrDefaultAsync<EmployeeMap>(
                "sp_GetEmployeeById",
                new { IdEmployee = employeeId },
                commandType: CommandType.StoredProcedure
                );
            if (row == null)
                throw new EntityNotFoundException("Empleado", employeeId);

            return row.ToEmployee();
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("sp_GetEmployeeById", "Error al buscar", ex);
        }
    }

    public async Task<IEnumerable<Employee>> GetAllAsync()
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
            const string Sql = "SELECT * FROM vw_AllEmployees";
            var rows = await Connection.QueryAsync<EmployeeDetailMap>(Sql);

            return rows.Select(r => r.ToEmployeeDetail());
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("Select * vw_GetAll", "Error al obtener", ex);
        }

    }

    public async Task<IEnumerable<(Employee employee, int QuantityOfSales)>> GetAllEmployeesWithQuantityofSaleAsync()
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
            const string Sql = "SELECT * FROM vw_AllEmployeeWithSales";
            var rows = await Connection.QueryAsync<EmployeeWithCountMap>(Sql);
            return rows.Select(r => r.ToTupple());
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("vw_AllEmployeeWithSales", "Error al obtener empleados con cantidad de ventas", ex);
        }
    }

        public async Task<bool> EmployeeExisting(string Name)
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
            const string Sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM Employee E INNER JOIN Person P ON C.PersonId = E.PersonId WHERE p.Identification = @Identification) THEN 1 ELSE 0 END AS EXISTS";

            var result = await connection.ExecuteScalarAsync<int>(Sql, new { Identification = Name }); 

            return result == 1;


        }
        catch(Exception e)
        {
            throw new DataBaseOperationException("Command", "Error al obtener", e);
        }
    }

        public async Task<bool> EmployeeExistingForUpdate(string Name,int id)
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
            const string Sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM Employee E INNER JOIN Person P ON E.PersonId = C.PersonId WHERE P.Identification = @Identification AND E.EmployeeId != @CustomerId) THEN 1 ELSE 0 END AS EXISTS";

            var result = await connection.ExecuteScalarAsync<int>(Sql, new { Identification = Name,EmployeeId = id }); 

            return result == 1;


        }
        catch(Exception e)
        {
            throw new DataBaseOperationException("Command", "Error al obtener", e);
        }
    }

    #endregion

    #region Mapper privados
    private class EmployeeMap
    {
        public int IdPerson { get; set; }
        public int IdEmployee { get; set; }
        public string FullName { get; set; } = "";
        public string TypeOfPerson { get; set; } = "";
        public string Identification { get; set; } = "";
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public DateTime RegisterDate { get; set; }
        public bool Status { get; set; }
        public string EmployeePosition { get; set; } = "";


        public Employee ToEmployee() => new Employee(
            idperson: IdPerson,
            fullname: FullName,
            typeofperson: TypeOfPerson,
            identitycard: Identification,
            address: Address,
            phone: Phone,
            registerdate: RegisterDate,
            status: Status,
            idemployee: IdEmployee,
            employeeposition: EmployeePosition
        );
    }

    private class EmployeeDetailMap
    {
        public int IdEmployee { get; set; }
        public string FullName { get; set; } = "";
        public string Identification { get; set; } = "";
        public string TypeofPerson { get; set; } = "";
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public DateTime RegisterDate { get; set; }
        public bool Status { get; set; }
        public string EmployeePosition { get; set; } = "";

        public Employee ToEmployeeDetail() => new Employee
            (
                fullname: FullName,
                typeofperson: TypeofPerson,
                identitycard: Identification,
                address: Address!,
                phone: Phone!,
                registerdate: RegisterDate,
                status: Status,
                idemployee: IdEmployee,
                employeeposition: EmployeePosition
            );
    }


    private class EmployeeWithCountMap : EmployeeDetailMap
    {
        public int QuantityOfSales { get; set; }

        public (Employee Employee, int quantityofsales) ToTupple() =>
            (ToEmployeeDetail(),quantityofsales : QuantityOfSales); 
    }
}

    #endregion