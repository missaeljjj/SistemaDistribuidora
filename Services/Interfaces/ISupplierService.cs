
using SistemaDistribuidora.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Services.Interfaces;

public  interface ISupplierService
{
    Task CreateNewSupplier(SupplierCreateDto dto);

    Task UpdateSupplier(SupplierUpdateDto dto);

    Task DeleteSupplier(int SupplierId);

    Task<IEnumerable<SupplierListDto>> GetAllSuppliers();

    Task<IEnumerable<SupplierDetailDto>> GetEmployeesDetail();
}
