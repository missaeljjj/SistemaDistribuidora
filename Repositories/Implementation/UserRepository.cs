using Dapper;
using SistemaDistribuidora.Exceptions;
using SistemaDistribuidora.Models;
using SistemaDistribuidora.Repositories.DataBaseConnection;
using SistemaDistribuidora.Repositories.Interfaces;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Repositories.Implementation;

public class UserRepository : IUserRepository
{
    private readonly IDataBase _dataBase;

    public UserRepository(IDataBase dataBase)
    {
        _dataBase = dataBase;
    }

    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        //Conexion a la base de datos
        DbConnection connection;
        try
        {
            connection = await _dataBase.GetConnectionAsync();
        }
        catch (Exception e)
        {
            
            throw new ConnectionException("Error al conectar a la base de datos", e);
        }
       
        try
        {
            //Dapper
            var parameters = new DynamicParameters();
            parameters.Add("@UserName", username, DbType.String);
            parameters.Add("@Password", password, DbType.String);

            // Ejecutamos pasándole el contenedor de parámetros
            var row = await connection.QueryFirstOrDefaultAsync<UserMap>(
                "sp_UserLogin",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return row?.ToUser();
        }
        catch (Exception ex)
        {
            throw ex;
            //throw new DataBaseOperationException(
               // "sp_UserLogin",
               // "Error al autenticar el usuario.",
               // ex));
            
        }
    }

    private class UserMap
    {
        public int UserId { get; set; }
        public int EmployeeId { get; set; }
        public string UserName { get; set; } = "";
        public string Role { get; set; } = "";

        public User ToUser() => new User(UserId, EmployeeId, UserName, Role);
    }
}