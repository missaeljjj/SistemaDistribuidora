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
                    Identification = employee.IdentityCard,
                    TypePerson = employee.TypeOfPerson,
                    Address = employee.Address,
                    Phone = employee.Phone,
                    Position = employee.Position ?? "Operativo"
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
                "sp_UpdateEmployee",
                new
                {
                    EmployeeId = employee.Id,
                    FullName = employee.FullName,
                    Identification = employee.IdentityCard,
                    Position = employee.Position,
                    Address = employee.Address,
                    TypePerson = employee.TypeOfPerson,
                    Phone = employee.Phone,
                    Status = employee.Status
                },
                commandType: CommandType.StoredProcedure
            );
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("sp_UpdateEmployee", "Error al actualizar el empleado", ex);
        }
    }

    public async Task DeleteAsync(int employeeId)
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
                "sp_DeleteEmployee",
                new { EmployeeId = employeeId },
                commandType: CommandType.StoredProcedure
            );
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("sp_DeleteEmployee", "Error al eliminar el empleado", ex);
        }
    }

    public async Task<Employee> GetByIdAsync(int employeeId)
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
            var row = await connection.QuerySingleOrDefaultAsync<EmployeeMap>(
                "sp_GetEmployeeByID",
                new { EmployeeId = employeeId },
                commandType: CommandType.StoredProcedure
            );

            if (row == null)
                throw new EntityNotFoundException("Empleado", employeeId);

            return row.ToEmployee();
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("sp_GetEmployeeById", "Error al buscar el empleado por ID", ex);
        }
    }

    public async Task<IEnumerable<Employee>> GetAllAsync()
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
            const string sql = "SELECT * FROM vw_AllEmployees";
            var rows = await connection.QueryAsync<EmployeeDetailMap>(sql);

            return rows.Select(r => r.ToEmployeeDetail());
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("SELECT * FROM vw_AllEmployees", "Error al obtener la lista de empleados", ex);
        }
    }

    public async Task<IEnumerable<(Employee employee, int QuantityOfSales)>> GetAllEmployeesWithQuantityofSaleAsync()
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
            const string sql = "SELECT * FROM vw_EmployeeSalesSummary";
            var rows = await connection.QueryAsync<EmployeeWithCountMap>(sql);
            return rows.Select(r => r.ToTuple());
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("vw_EmployeeSalesSummary", "Error al obtener empleados con cantidad de ventas", ex);
        }
    }

    public async Task<bool> EmployeeExisting(string identification)
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
            const string sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM Employee E INNER JOIN Person P ON P.PersonId = E.PersonId WHERE P.Identification = @Identification) THEN 1 ELSE 0 END AS result";
            var result = await connection.ExecuteScalarAsync<int>(sql, new { Identification = identification });

            return result == 1;
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("EmployeeExisting", "Error al verificar la existencia del empleado", ex);
        }
    }

    public async Task<bool> EmployeeExistingForUpdate(string identification, int id)
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
            const string sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM Employee E INNER JOIN Person P ON E.PersonId = P.PersonId WHERE P.Identification = @Identification AND E.EmployeeId != @EmployeeId) THEN 1 ELSE 0 END AS [EXISTS]";
            var result = await connection.ExecuteScalarAsync<int>(sql, new { Identification = identification, EmployeeId = id });

            return result == 1;
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException("EmployeeExistingForUpdate", "Error al verificar existencia para actualización", ex);
        }
    }

    #endregion

    #region Mappers Privados

    private class EmployeeMap
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = "";
        public string TypeOfPerson { get; set; } = "";
        public string Identification { get; set; } = "";
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public DateTime RegisterDate { get; set; }
        public string Status { get; set; } = "";
        public string Position { get; set; } = "";

        public Employee ToEmployee() => new Employee(
            idemployee: EmployeeId,
            fullname: FullName,
            typeofperson: TypeOfPerson,
            identitycard: Identification,
            address: Address,
            phone: Phone,
            registerdate: RegisterDate,
            status: Status == "Activo",
            employeeposition: Position
        );
    }

    private class EmployeeDetailMap
    {
        public int Id { get; set; } 
        public string FullName { get; set; } = "";
        public string IdentityCard { get; set; } = ""; 
        public string TypeOfPerson { get; set; } = "Natural";
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public DateTime RegisterDate { get; set; }
        public string Status { get; set; } = ""; 
        public string Position { get; set; } = ""; 

        public Employee ToEmployeeDetail() => new Employee(
            fullname: FullName,
            typeofperson: TypeOfPerson,
            identitycard: IdentityCard,
            address: Address ?? string.Empty,
            phone: Phone ?? string.Empty,
            registerdate: RegisterDate,
            status: Status == "Activo",
            idemployee: Id,
            employeeposition: Position
        );
    }

    private class EmployeeWithCountMap : EmployeeDetailMap
    {
        
        public int QuantityOfSales { get; set; }

        public (Employee Employee, int QuantityOfSales) ToTuple() =>
            (ToEmployeeDetail(), QuantityOfSales);
    }
    #endregion
}