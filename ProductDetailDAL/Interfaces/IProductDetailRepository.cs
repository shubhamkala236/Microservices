using Common;

namespace ProductDetailDAL.Interfaces
{
    public interface IProductDetailRepository
    {
        ProductDetail GetProductDetailsById(int id);
        ProductDetail AddProductDetails(ProductDetail productDetails);
        ProductDetail RemoveProductDetails(int id);
        List<ProductDetail> GetAllProductDetails();
    }
}
