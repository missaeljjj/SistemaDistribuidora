using System; 

namespace SistemaDistribuidora.Models;

class Customer : Person
{
    private int _IdCustomer;

    public int IdCustomer
    {
        get => _IdCustomer;
        private set => _IdCustomer = value;
    }

    public Customer(int idperson, string fullname, string typeofperson, string identitycard, string address, 
                    DateTime registerdate, bool status,int idcustomer)
                     
                    : base(idperson, fullname, typeofperson, identitycard , address, registerdate, status)
    {
        this.IdCustomer = idcustomer;
    }
}
