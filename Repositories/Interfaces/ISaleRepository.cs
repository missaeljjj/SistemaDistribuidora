using SistemaDistribuidora.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Repositories.Interfaces;

public interface ISaleRepository : IGenericRepository<Sale>
{
    // Obtener ventas realizadas en un rango de fechas específico
    Task<IEnumerable<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);

    // Obtener el historial de ventas completo de un cliente específico
    Task<IEnumerable<Sale>> GetSalesByCustomerAsync(int idCustomer);

    // Obtener las ventas procesadas por un empleado
    Task<IEnumerable<Sale>> GetSalesByEmployeeAsync(int idEmployee);

    // Método especial para anular una venta (cambiar estado y regresar los productos al inventario)
    // No se va implementar por el momento por la complejidad que requiere este tipo de transaccion
    Task<bool>? CancelSaleAsync(int idSale);
}