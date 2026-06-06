using System;
namespace SistemaDistribuidora.Models;

public class Employee : Person
{
    private int _IdEmployee;
    private string EmployeePosition = "";

    public int IdEmployee
    {
        get => _IdEmployee;
        private set => _IdEmployee = value;
    }

    public string Position
    {
        get => EmployeePosition;
        private set => EmployeePosition = !string.IsNullOrWhiteSpace(value) ? value.Trim() : throw new ArgumentException("El cargo no puede estar vacío");
    }

    public Employee(int idperson, string fullname, string typeofperson, string identitycard,
        string address, DateTime registerdate, bool status, int idemployee, string employeeposition)

         : base(idperson, fullname, typeofperson, identitycard, address, registerdate, status)
    {
        this.IdEmployee = idemployee;
        this.Position = employeeposition;
    }
}