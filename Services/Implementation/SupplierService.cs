
using SistemaDistribuidora.Services.Interfaces;
using SistemaDistribuidora.Repositories.Interfaces;
using System.Threading.Tasks;
using SistemaDistribuidora.DTOs;
using System.Collections.Generic;
using SistemaDistribuidora.Exceptions;
using SistemaDistribuidora.Mappers;

namespace SistemaDistribuidora.Services.Implementation;

public class SupplierService : ISupplierService
{
    private readonly ISuplierRepository _SuplierRepository;

    public SupplierService(ISuplierRepository suplierRepository)
    {
        _SuplierRepository = suplierRepository;
    }

    public async Task CreateNewSupplier(SupplierCreateDto dto)
    {
        bool duplicated = await _SuplierRepository.SupplierExisting(dto.Identity);

        if(duplicated)
            throw new BussinessRulesException("Identificacion duplicada", "Identificacion ya existente");

        var customer = dto.ToModel();

        await _SuplierRepository.InsertAsync(customer);
    }


    public async Task UpdateSupplier(SupplierUpdateDto dto)
    {
        var existing = await _SuplierRepository.GetByIdAsync(dto.SupplierId);

        bool Duplicated = await _SuplierRepository.SupplierExistingForUpdate(dto.Identity ?? existing.IdentityCard, dto.SupplierId);

        if(Duplicated)
            throw new BussinessRulesException("Identificacion duplicada", "Identificacion ya existente");

        var updated = dto.ToModel(existing);

        await _SuplierRepository.UpdateAsync(updated);
    }

    public async Task DeleteSupplier(int SupplierId)
    {
        //vacio por el momento
    }

    public async Task<IEnumerable<SupplierListDto>> GetAllSuppliers()
    {
        var SupplierList = await _SuplierRepository.GetAllAsync();

        return SupplierList.ToList();
    }

    public async Task<IEnumerable<SupplierDetailDto>> GetEmployeesDetail()
    {
        var SupplierDetail = await _SuplierRepository.GetAllSuppliersSummary();

        return SupplierDetail.ToDetailList();

    }



}
