
using SistemaDistribuidora.Services.Interfaces;
using SistemaDistribuidora.Repositories.Interfaces;
using System.Threading.Tasks;
using SistemaDistribuidora.DTOs;
using System.Collections.Generic;
using SistemaDistribuidora.Exceptions;
using SistemaDistribuidora.Mappers;
using System.Linq;

namespace SistemaDistribuidora.Services.Implementation;

public class SupplierService : ISupplierService
{
    private readonly ISuplierRepository _SuplierRepository;
    private readonly IAppCache _Cache;

    public SupplierService(ISuplierRepository suplierRepository,IAppCache cache)
    {
        _SuplierRepository = suplierRepository;
        _Cache = cache;
    }

    public async Task CreateNewSupplier(SupplierCreateDto dto)
    {

        var customer = dto.ToModel();

        await _SuplierRepository.InsertAsync(customer);
        await _Cache.ReloadSuppliersAsync();
    }


    public async Task UpdateSupplier(SupplierUpdateDto dto)
    {
        var existing = await _SuplierRepository.GetByIdAsync(dto.SupplierId);

        bool Duplicated = await _SuplierRepository.SupplierExistingForUpdate(dto.Identity ?? existing.IdentityCard, dto.SupplierId);

        if(Duplicated)
            throw new BussinessRulesException("Identificacion duplicada", "Identificacion ya existente");

        var updated = dto.ToModel(existing);

        await _SuplierRepository.UpdateAsync(updated);
        await _Cache.ReloadSuppliersAsync();
    }

    public async Task DeleteSupplier(int SupplierId)
    {
        //No implementation for this version
    }

    public async Task<IEnumerable<SupplierListDto>> GetAllSuppliers()
    {
        if (_Cache.Suppliers == null || !_Cache.Suppliers.Any())
        {
            await _Cache.ReloadSuppliersAsync();
        }

        return _Cache.Suppliers ?? new List<SupplierListDto>();

    }

    public async Task<IEnumerable<SupplierDetailDto>> GetSuppliersDetail()
    {
        var SupplierDetail = await _SuplierRepository.GetAllSuppliersSummary();

        return SupplierDetail.ToDetailList();

    }

}
