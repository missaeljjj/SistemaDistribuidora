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
                    DateTime registerdate, bool status, int idsupplier)

                    : base(idperson, fullname, typeofperson, identitycard, address, registerdate, status)
    {
        this.IdSupplier = idsupplier;
    }
}