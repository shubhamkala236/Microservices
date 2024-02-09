using Common;

namespace ProductDetailBLL.Interfaces
{
    public interface IProductDetailBLLService
    {
        ProductDetail GetProductDetailsById(int id);
        Task<ProductDetail> AddProductDetails(ProductDetail productDetails);
        Task<ProductDetail> RemoveProductDetails(int id);
        List<ProductDetail> GetAllProductDetails();
    }
}
