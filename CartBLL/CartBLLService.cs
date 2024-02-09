using CartBLL.Interfaces;
using CartDAL.Interfaces;
using Common;
using Newtonsoft.Json;
using Polly;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartBLL
{
    public class CartBLLService : ICartBLLService
    {
        public readonly ICartRepository cartRepository;
        public CartBLLService(ICartRepository cartRepository)
        {
            this.cartRepository = cartRepository;
        }

        public async Task<CartItem> addToMyCart(int userId, int productId, int quantity)
        {
            //call productDetail and product service to create CartItem
            //1 take product details from Pdetails service
            //2 take product name from product service and check quantity
            //3 then create cartIem and add it


            //circuit breaker-----------------------------------------
            var amountToPause = TimeSpan.FromSeconds(10);
            var retryWaitPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(5, i => amountToPause, onRetry: (exception, retryCount) =>
            {
                Console.WriteLine("Error:" + exception.Message + "...Retry Count" + retryCount);
            });

            var circuitBreakerPolicy = Policy.Handle<Exception>().CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));

            var finalPolicy = retryWaitPolicy.WrapAsync(circuitBreakerPolicy);
            //circuit breaker-----------------------------------------

            //call productDetail api
            var productDetailData = await finalPolicy.ExecuteAsync(async () =>
            {
                var productDetail = await getProductDetailsApi(productId);
                //cannot add this details as product does not exists
                if (productDetail == null || productDetail.ProductId == 0)
                {
                    return null;
                }

                return productDetail;
            });

            //call product api
            var productData = await finalPolicy.ExecuteAsync(async () =>
            {
                var product = await getProductApi(productId);
               
                //check quantity validity
                if(product.Quantity <= 0 || product.Quantity <quantity)
                {
                    return null;
                }

                return product;
            });

            //if any of (detail or quantiy case is null cannot add product to cart)
            if(productDetailData == null || productData ==null)
            {
                return null;
            }

            //create cart item
            CartItem cartItem = new CartItem()
            {
                UserId = userId,
                Name = productData.Name,
                Quantity = quantity,
                ProductId = productData.Id,
                Price = productDetailData.Price,
                Description = productDetailData.Description,
            };

            var addCartItem = cartRepository.addToMyCart(cartItem);

            return addCartItem;

        }

        public List<CartItem> getMyCartItems(int userId)
        {
            var cartItems = cartRepository.getMyCartItems(userId);
            return cartItems;
        }

        //Event based
        public void RemoveFromAllCarts(int productId)
        {
            cartRepository.RemoveFromAllCarts(productId);
        }

        public CartItem RemovefromMyCart(int userId, int productId)
        {
            var removedItem = cartRepository.RemovefromMyCart(userId, productId);
            //event launch to deduct product quantity from product service 

            return removedItem;
        }

        //api to get product details
        private async Task<ProductDetail> getProductDetailsApi(int productId)
        {
            //URL to Product Detail Service to fetch
            //var url = $"http://localhost:5002/api/ProductDetail/getProductDetailsById/{productId}";
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

        private async Task<Product> getProductApi(int productId)
        {
            //URL to Product Service to fetch
            //var url = $"http://localhost:5001/api/Product/getProductById/{productId}";
            var url = $"http://product-container:5001/api/Product/getProductById/{productId}";

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
            var product = JsonConvert.DeserializeObject<Product>(data);

            return product;

        }

    }
}
