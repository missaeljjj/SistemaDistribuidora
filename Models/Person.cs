using System;
using SistemaDistribuidora.Exceptions;
namespace SistemaDistribuidora.Models;

public abstract class Person
{
    private int _IdPerson;
    private string _FullName = "";
    private string _TypeOfPerson = "";
    private string _IdentityCard = "";
    private string _Address = "";
    private DateTime _RegisterDate = DateTime.Now;
    private bool _Status = true;

    public int Id
    {
        get => _IdPerson;
        protected set => _IdPerson = value;
    }

    // Uso de operador ternario para validar que el nombre no sea nulo o esté compuesto solo por espacios en blanco
    // funciona como un if-else: si el valor es nulo o solo espacios, se asigna el valor recortado (sin espacios al inicio y al final), de lo contrario, se lanza una excepción
    public string FullName
    {
        get => _FullName;
        protected set => _FullName = !string.IsNullOrWhiteSpace(value) ? value.Trim() : throw new ValidationException("El nombre no puede estar vacio", nameof(FullName));
    }

    public string TypeOfPerson
    {
        get => _TypeOfPerson;
        protected set
        {
            string NewValue = value?.Trim() ?? "";
            if (NewValue != "Natural" && NewValue != "Juridica")
                throw new ValidationException("El tipo de persona debe ser 'Natural' o 'Juridica'",nameof(TypeOfPerson));
            _TypeOfPerson = NewValue;
        }
    }

    public string IdentityCard
    {
        get => _IdentityCard;
        protected set => _IdentityCard = !string.IsNullOrWhiteSpace(value) ? value.Trim() : throw new ValidationException("La identificacion no puede estar vacía", nameof(IdentityCard));
    }

    public string Address
    {
        get => _Address;
        protected set => _Address = !string.IsNullOrWhiteSpace(value) ? value.Trim() : throw new ValidationException("La dirección no puede estar vacía", nameof(Address));
    }

    public DateTime RegisterDate
    {
        get => _RegisterDate;
        protected set => _RegisterDate = value;
    }

    public bool Status
    {
        get => _Status;
        protected set => _Status = value;
    }

    public Person(int idperson, string fullname, string typeofperson, string identitycard, string address, DateTime registerdate, bool status)
    {
        this.Id = idperson;
        this.FullName = fullname;
        this.TypeOfPerson = typeofperson;
        this.IdentityCard = identitycard;
        this.Address = address;
        this.RegisterDate = registerdate;
        this.Status = status;
    }

}