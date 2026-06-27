using System.Linq;
using SistemaDistribuidora.Exceptions;
using System.Threading.Tasks;
using SistemaDistribuidora.DTOs;
using SistemaDistribuidora.Repositories.Interfaces;
using SistemaDistribuidora.Services.Interfaces;
using SistemaDistribuidora.Mappers;

namespace SistemaDistribuidora.Services.Implementation;

    public class PurchasesService : IPurchaseService
    {
        private readonly IPurchaseRepository _PurchaseRepository;

        public PurchasesService(IPurchaseRepository purchaserepository)
        {
            _PurchaseRepository = purchaserepository;
        }

        public async Task CreateNewPurchase(PurchaseCreateDto dto)
        {
    
            if (!dto.Items.Any())
                throw new BussinessRulesException("CarritoVacio", "La compra debe tener al menos un producto.");

            if (dto.Items.Any(i => i.Quantity <= 0))
                throw new BussinessRulesException("CantidadInvalida", "Todos los productos deben tener una cantidad mayor a cero.");

            if(dto.Items.Any(i => i.UnitPrice <= 0))
                throw new BussinessRulesException("Precio invalido", "Todos los productos deben tener un precio mayor a 0");

            var purchase = dto.ToModel();
            await _PurchaseRepository.CreateNewPurchaseAsync(purchase);
        }
    }


    
