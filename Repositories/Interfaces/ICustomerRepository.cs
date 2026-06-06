using SistemaDistribuidora.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Repositories.Interfaces;

public interface ICustomerRepository : IGenericRepository<Customer> 
{
    // Obtiene clientes que tienen facturas registradas
    Task<IEnumerable<Customer>> GetAllWithPurchases();
    // Obtiene los clientes que no han comprado nada todavía
    Task<IEnumerable<Customer>> GetAllWithoutPurchases();
    //Obtiene clientes con la cantidad de compras que han realizado
    Task<IEnumerable<Customer>> GetAllWithQuantityOfPurchases();
}