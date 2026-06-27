using SistemaDistribuidora.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Repositories.Interfaces;

public interface ICustomerRepository : IGenericRepository<Customer> 
{
    Task<IEnumerable<(Customer customer, int QuantityOfPurchases)>> GetAllWithQuantityOfPurchases();

    Task<bool> ExistsCustomerIdentificationUpdateAsync(string identity);

    Task<bool> CustomerExistingForUpdate(string Name, int id);
}