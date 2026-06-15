using SistemaDistribuidora.Repositories.DataBaseConnection;
using SistemaDistribuidora.Repositories.Interfaces;
using SistemaDistribuidora.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SistemaDistribuidora.Repositories.Implementation;

public class CustomerRepository  : ICustomerRepository
{
    private readonly IDataBase _DateBase;

    //por implementar...
    public CustomerRepository(IDataBase database)
    {
        _DateBase = database;
    }

    public async Task InsertAsync(Customer customer)
    {

    }

    public async Task UpdateAsync(Customer customer)
    {

    }

    public async Task DeleteAsync(int customerId)
    {

    }

    public async Task<Customer> GetByIdAsync(int customerId)
    {
        return null;
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return null;
    }


}