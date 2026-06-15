using SistemaDistribuidora.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Repositories.Interfaces;

public interface ISuplierRepository : IGenericRepository<Supplier>
{
    // Obtiene todos los proveedores junto con las compras que han realizado y la cantidad de prodctos sumisntrados
    Task<IEnumerable<Supplier>> GetAllSuppliersSummary();

}