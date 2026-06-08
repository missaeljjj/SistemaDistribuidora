using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Repositories.DataBaseConnection;

public class DataBase
{
    private readonly string _connectionString = "";

    public DataBase()
    {
        // Detectamos el entorno (Por defecto en desarrollo lee 'Development')
        string environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

        // Construimos el lector apuntando a la carpeta donde corre la app de Avalonia
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory) // Busca en el directorio de ejecucion de la computadora
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Archivo general obligatorio
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true); // Archivo especifico opcional

        // Construimos la configuracion
        IConfiguration config = builder.Build();

        // Extraemos la cadena de conexion de Azure (Permitimos que sea un valor recuperable para validarlo abajo)
        _connectionString = config.GetConnectionString("AzureConnection") ?? string.Empty;

        // En caso de que no se haya podido cargar la cadena de conexion, lanzamos una excepcion critica
        if (string.IsNullOrEmpty(_connectionString))
        {
            throw new InvalidOperationException("Error crítico: No se pudo cargar la configuración de la Base de Datos desde los archivos JSON.");
        }
    }

    // Cambiado a GetConnectionAsync ya que retorna el objeto SqlConnection abierto
    protected async Task<SqlConnection> GetConnectionAsync()
    {
        // Creamos una nueva conexión utilizando la cadena de conexion cargada en el constructor
        var connection = new SqlConnection(_connectionString);

        // Abrimos la conexión de forma asincrona para mejorar el rendimiento y evitar bloqueos en la interfaz de usuario de Avalonia
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        return connection;
    }
}