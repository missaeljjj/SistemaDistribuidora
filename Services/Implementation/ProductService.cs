using SistemaDistribuidora.DTOs;
using SistemaDistribuidora.Exceptions;
using SistemaDistribuidora.Mappers;
using SistemaDistribuidora.Repositories.Implementation;
using SistemaDistribuidora.Repositories.Interfaces;
using SistemaDistribuidora.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

    public async Task<bool> UpdateProductPrice(int productId, decimal newSalePrice)
    {
   
        if (productId <= 0)
            throw new BussinessRulesException("Id no existente", $"Producto con Id: {productId} no existe"); 

        if (newSalePrice <= 0)
            throw new BussinessRulesException("Precio no válido", "El precio de venta debe ser mayor a cero.");

        return await _ProductRepository.UpdateProductPricesAsync(productId, newSalePrice);
    }
}
