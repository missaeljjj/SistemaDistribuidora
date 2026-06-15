using SistemaDistribuidora.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Repositories.Interfaces;

public interface IPurchaseRepository : IGenericRepository<Purchase>
{
    // Obtener las compras de abastecimiento hechas a un proveedor específico
    Task<IEnumerable<Purchase>>? GetPurchasesBySupplierAsync(int idSupplier);

    // Consultar el dinero invertido en compras en un rango de fechas
    Task<IEnumerable<Purchase>> GetPurchasesByDateRangeAsync(DateTime startDate, DateTime endDate);
}