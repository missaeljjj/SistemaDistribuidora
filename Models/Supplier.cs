namespace SistemaDistribuidora.Models;
using System;

public class Supplier : Person
{
    private int _IdSupplier;
    public int IdSupplier
    {
        get => _IdSupplier;
        private set => _IdSupplier = value;
    }
    public Supplier(int idperson, string fullname, string typeofperson, string identitycard, string address,
                    string phone, DateTime registerdate, bool status, int idsupplier)

                    : base(idperson, fullname, typeofperson, identitycard, address,phone, registerdate, status)
    {
        this.IdSupplier = idsupplier;
    }

    public Supplier(string fullname, string typeofperson, string identitycard, string address,
                    string phone, DateTime registerdate, bool status, int idsupplier)

                    : base(fullname, typeofperson, identitycard, address, phone, registerdate, status)
    {
        this.IdSupplier = idsupplier;
    }
}