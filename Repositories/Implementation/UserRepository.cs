using Dapper;
using SistemaDistribuidora.Exceptions;
using SistemaDistribuidora.Models;
using SistemaDistribuidora.Repositories.DataBaseConnection;
using SistemaDistribuidora.Repositories.Interfaces;
using System;
using System.Data;
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
        await using var connection = await _dataBase.GetConnectionAsync();

        try
        {
            //Dapper
            var row = await connection.QueryFirstOrDefaultAsync<UserMap>(
                "sp_UserLogin",
                new { Username = username, Password = password },
                commandType: CommandType.StoredProcedure
            );

            return row?.ToUser();
        }
        catch (Exception ex)
        {
            throw new DataBaseOperationException(
                "usp_Login",
                "Error al autenticar el usuario.",
                ex
            );
        }
    }

    private class UserMap
    {
        public int UserId { get; set; }
        public int EmployeeId { get; set; }
        public string Username { get; set; } = "";
        public string Role { get; set; } = "";

        public User ToUser() => new User(UserId, EmployeeId, Username, Role);
    }
}