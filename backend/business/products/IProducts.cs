using backend.config;
using backend.dto;

namespace backend.business.products
{
    public interface IProducts
    {
        Task<List<ProductsDTO>> GetAll();
        Task<ProductsDTO> GetByCode(int code);
        Task<ProductsDTO> GetByCodeProduct(string code);
        Task<ProductsDTO> Insert(ProductsDTO product);
        Task<ProductsDTO> Update(ProductsDTO product);
        Task<bool> Delete(int code);
    }
}
