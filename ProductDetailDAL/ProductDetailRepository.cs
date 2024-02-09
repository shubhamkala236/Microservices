using Common;
using ProductDetailDAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductDetailDAL
{
    public class ProductDetailRepository : IProductDetailRepository
    {
        //inmemory database
        private List<ProductDetail> productDetailsList = new List<ProductDetail>();

        public ProductDetail AddProductDetails(ProductDetail productDetails)
        {
            productDetailsList.Add(productDetails);
            return productDetails;
        }

        public List<ProductDetail> GetAllProductDetails()
        {
            return productDetailsList;
        }

        public ProductDetail GetProductDetailsById(int id)
        {
            var productDetails = productDetailsList.FirstOrDefault(p => p.ProductId == id);
            if(productDetails == null)
            {
                return null;
            }

            return productDetails;
        }

        public ProductDetail RemoveProductDetails(int id)
        {
            var existingProductDetails = productDetailsList.FirstOrDefault(p => p.ProductId == id);
            if(existingProductDetails == null)
            {
                return null;
            }

            productDetailsList.Remove(existingProductDetails);

            return existingProductDetails;

        }

    }
}
