using Common;
using Newtonsoft.Json;
using Polly;
using ProductBLL.Interfaces;
using ProductDAL.Interfaces;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductBLL
{
    public class ProductBLLService : IProductBLLService
    {
        private readonly IProductRepository productRepository;
        public ProductBLLService(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        public Product AddProduct(Product product)
        {
            return productRepository.AddProduct(product);
        }

        public List<Product> GetAllProducts()
        {
            return productRepository.GetAllProducts();
        }

        public Product GetProductById(int id)
        {
            return productRepository.GetProductById(id);
        }

        public async Task<ProductDetail> GetProductDetailById(int id)
        {
            //check if product exists
            var exists = productRepository.GetProductById(id);
            if(exists == null)
            {
                return null;
            }

            //circuit breaker-----------------------------------------
            var amountToPause = TimeSpan.FromSeconds(10);
            var retryWaitPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(5, i => amountToPause, onRetry: (exception, retryCount) =>
            {
                Console.WriteLine("Error:" + exception.Message + "...Retry Count" + retryCount);
            });

            var circuitBreakerPolicy = Policy.Handle<Exception>().CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));

            var finalPolicy = retryWaitPolicy.WrapAsync(circuitBreakerPolicy);
            //-----------------------------------------------------------
            //call productDetail api
            var data = await finalPolicy.ExecuteAsync(async () =>
            {
                var productDetail = await connectToApi(id);
                //if cannot add this details as product does not exists
                if (productDetail==null || productDetail.ProductId==0)
                {
                    return null;
                }

                return productDetail; 
            });

            return data;
        }

        private async Task<ProductDetail> connectToApi(int productId)
        {
            //URL to Product Service to fetch all products
            var url = $"http://productDetail-container:5002/api/ProductDetail/getProductDetailsById/{productId}";

            var client = new RestClient();
            var request = new RestRequest(url, Method.Get);

            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");

            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                Console.WriteLine(response.ErrorMessage);
            }


            //if success message
            var data = response.Content;

            //deserialize data
            var productDetails = JsonConvert.DeserializeObject<ProductDetail>(data);

            return productDetails;

        }

        //Fires Event
        public Product RemoveProduct(int id)
        {
            var removedProduct = productRepository.RemoveProduct(id);
            return removedProduct;
        }

        //called by event only
        public void DeductQuantity(int productId, int quantity)
        {
            productRepository.DeductQuantity(productId, quantity);
        }
    }
}
