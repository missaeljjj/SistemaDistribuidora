using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Repositories.DataBaseConnection;

public interface IDataBase
{
    Task<SqlConnection> GetConnectionAsync();
}
