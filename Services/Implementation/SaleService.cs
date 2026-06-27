using System;
using System.Linq;
using SistemaDistribuidora.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using SistemaDistribuidora.DTOs;
using SistemaDistribuidora.Repositories.Interfaces;
using SistemaDistribuidora.Services.Interfaces;
using SistemaDistribuidora.Mappers;

namespace SistemaDistribuidora.Services.Implementation
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _SaleRepository;

        public SaleService(ISaleRepository saleRepository)
        {
            _SaleRepository = saleRepository;
        }

        public async Task CreateNewSale(SaleCreateDto dto)
        {

            if (!dto.Items.Any())
                throw new BussinessRulesException("CarritoVacio", "La venta debe tener al menos un producto.");

            if (dto.Items.Any(i => i.Quantity <= 0))
                throw new BussinessRulesException("CantidadInvalida", "Todos los productos deben tener una cantidad mayor a cero.");

            if(dto.Items.Any(i => i.UnitPrice <= 0))
                throw new BussinessRulesException("Precio invalido", "Todos los productos deben tener un precio mayor a 0");

            var Sale = dto.ToModel();
            await _SaleRepository.CreateNewSaleAsync(Sale);
        }
    }
}

    
