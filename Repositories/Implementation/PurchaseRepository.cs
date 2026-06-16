using SistemaDistribuidora.Models;
using SistemaDistribuidora.Repositories.DataBaseConnection;
using  SistemaDistribuidora.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace SistemaDistribuidora.Repositories.Implementation;

public class PurchaseRepository : IPurchaseRepository
{
    private readonly IDataBase _DataBase;

    public PurchaseRepository(IDataBase dataBase)
    {
        _DataBase = dataBase;
    }

    public async Task InsertAsync(Purchase purchase)
    {

    }

    public async Task UpdateAsync(Purchase purchase)
    {

    }

    public async Task DeleteAsync(int PurchaseId)
    {

    }

    public async Task<IEnumerable<Purchase>> GetAllAsync()
    {
        return null!;
    }

    public async Task<Purchase> GetByIdAsync(int PurchaseId)
    {
        return null!;
    }

    public async Task<IEnumerable<Purchase>> GetPurchasesBySupplierAsync(int idSupplier)
    {
        return null!;
    }

    // Obtener las compras gestionadas por un empleado específico
    public async Task<IEnumerable<Purchase>> GetPurchasesByEmployeeAsync(int idEmployee)
    {
        return null!;
    }


    public async Task<IEnumerable<Purchase>> GetPurchasesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return null!;
    }
}
