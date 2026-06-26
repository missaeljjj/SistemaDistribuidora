using SistemaDistribuidora.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Services.Interfaces;

public interface IProductService
{
    Task CreateNewProduct(ProductCreateDto productCreate);

    Task UpdateProduct(ProductUpdateDto productUpdate);

    Task DeleteProduct(int ProductId);

    Task<IEnumerable<InventoryDetailDto>> GetAllProducts();

    Task<IEnumerable<ProductDetailDto>> GetProductsDetail();

}
