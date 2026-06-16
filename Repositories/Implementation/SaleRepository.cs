using SistemaDistribuidora.Models;
using SistemaDistribuidora.Repositories.DataBaseConnection;
using SistemaDistribuidora.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Repositories.Implementation;
public  class SaleRepository : ISaleRepository
{
    private readonly IDataBase _DataBase;

    public SaleRepository(IDataBase dataBase)
    {
        _DataBase = dataBase;
    }

    public async Task InsertAsync(Sale sale)
    {

    }

    // Futura implementacion
    public async Task UpdateAsync(Sale sale)
    {

    }

    // Futura implementacion
    public async Task DeleteAsync(int SaleId)
    {

    }

    public async Task<IEnumerable<Sale>> GetAllAsync()
    {
        return null!;
    }

    public async Task<Sale> GetByIdAsync(int SaeId)
    {
        return null!;
    }


    public async Task<IEnumerable<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return null!;
    }

    // Obtener el historial de ventas completo de un cliente específico
    public async Task<IEnumerable<Sale>> GetSalesByCustomerAsync(int idCustomer)
    {
        return null!;
    }

    // Obtener las ventas procesadas por un empleado
    public async Task<IEnumerable<Sale>> GetSalesByEmployeeAsync(int idEmployee)
    {
        return null!;
    }

    // Futura implementacion
    public async Task<bool> CancelSaleAsync(int idSale)
    {
        return false;
    }


}
