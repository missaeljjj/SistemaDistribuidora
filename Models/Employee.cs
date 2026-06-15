using System;
using SistemaDistribuidora.Exceptions;
namespace SistemaDistribuidora.Models;

public class Employee : Person
{
    private int _IdEmployee;
    private string _EmployeePosition = "";

    public int IdEmployee
    {
        get => _IdEmployee;
        private set => _IdEmployee = value;
    }

    public string Position
    {
        get => _EmployeePosition;
        private set => _EmployeePosition = !string.IsNullOrWhiteSpace(value) ? value.Trim() : throw new ValidationException("El cargo no puede estar vacío",nameof(Position));
    }

    public Employee(int idperson, string fullname, string typeofperson, string identitycard,
        string address,string phone, DateTime registerdate, bool status, int idemployee, string employeeposition)

         : base(idperson, fullname, typeofperson, identitycard, address,phone, registerdate, status)
    {
        this.IdEmployee = idemployee;
        this.Position = employeeposition;
    }
}