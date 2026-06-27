using SistemaDistribuidora.Repositories.Interfaces;
using SistemaDistribuidora.Services.Interfaces;
using SistemaDistribuidora.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using SistemaDistribuidora.Exceptions;
using SistemaDistribuidora.Mappers;

namespace SistemaDistribuidora.Services.Implementation;

public class ProductService : IProductService
{
    private readonly IProductRepository _ProductRepository;

    public ProductService(IProductRepository productRepository)
    {
        _ProductRepository   = productRepository;   
    }

    public async Task CreateNewProduct(ProductCreateDto dto)
    {
        bool existing = await _ProductRepository.ExistsByNameAsync(dto.Name);

        if(existing)
            throw new BussinessRulesException("Producto Existente", $"Producto: {dto.Name} ya existe");

        var product = dto.ToModel();

        await _ProductRepository.InsertAsync(product);
    }

    public async Task UpdateProduct(ProductUpdateDto dto)
    {
        var existing = await _ProductRepository.GetByIdAsync(dto.ProductId);

        bool duplicated = await _ProductRepository.ExistsByNameExcludedAsync(dto.Name ?? existing.Name, dto.ProductId);
        
        if(duplicated)
            throw new BussinessRulesException("Producto Existente", $"Producto con nombre: {dto.Name} ya existe");
        
        var update = dto.ToModel(existing);

        await _ProductRepository.UpdateAsync(update);
    }

    public async Task DeleteProduct(int ProductId)
    {
        //No implementation yet
    }

    public async Task<IEnumerable<InventoryDetailDto>> GetAllProducts()
    {
        var InventoryDetail = await _ProductRepository.GetAllProductsInInventoryAsync();

        return InventoryDetail.ToInventoryList();       
    }

    public async Task<IEnumerable<ProductDetailDto>> GetProductsDetail()
    {
        var ProductDetail = await _ProductRepository.GetAllProductsWithQuantityOfSales();

        return ProductDetail.ToDetailList();
    }
}
