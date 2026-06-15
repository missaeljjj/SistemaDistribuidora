using SistemaDistribuidora.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Repositories.Interfaces;

public interface ICustomerRepository : IGenericRepository<Customer> 
{
    Task<IEnumerable<Customer>> GetAllWithQuantityOfPurchases();
}