using System;

namespace SistemaDistribuidora.Models;

public class Customer : Person
{
    private int _IdCustomer;

    public int IdCustomer
    {
        get => _IdCustomer;
        private set => _IdCustomer = value;
    }

    public Customer(int idperson, string fullname, string typeofperson, string identitycard, string address,
                    string phone,DateTime registerdate, bool status, int idcustomer)

                    : base(idperson, fullname, typeofperson, identitycard, address,phone,registerdate, status)
    {
        this.IdCustomer = idcustomer;
    }

    //Constructor para detalle de cliente    
    public Customer(string fullname, string typeofperson, string identitycard, string address,
                    string phone, DateTime registerdate, bool status, int idcustomer)

                    : base(fullname, typeofperson, identitycard, address, phone, registerdate, status)
    {
        this.IdCustomer = idcustomer;
    }

    
}
