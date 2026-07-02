using SistemaDistribuidora.Exceptions;
namespace SistemaDistribuidora.Models;

public class User
{
    private int _UserId;
    private int _EmployeeId;
    private string _Username = "";
    private string _Role = "";

    public int UserId
    {
        get => _UserId;
        private set => _UserId = value;
    } 

    public int EmployeeId
    {
        get => _EmployeeId;
        private set => _EmployeeId = value;
    }

    public string Username
    {
        get => _Username;
        private set => _Username = !string.IsNullOrWhiteSpace(value) ? value.Trim() : throw new ValidationException("Usuario no puede estar vacio", nameof(Username));
    }

    public string Role
    {
        get => _Role;
        private set => _Role = value == "Administrador" || value == "Vendedor"
            ? value
            : throw new ValidationException("El rol debe ser 'Administrados' o 'Vendedor'", nameof(Role));
    }

    public User(int userId, int employeeId, string username, string role)
    {
        UserId = userId;
        EmployeeId = employeeId;
        Username = username;
        Role = role;
    }

    public bool IsAdmin() => Role == "Administrador";
    public bool IsSeller() => Role == "Vendedor";
}


