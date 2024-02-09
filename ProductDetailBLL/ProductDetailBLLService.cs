using Common;
using MassTransit;
using MassTransit.Transports;
using Newtonsoft.Json;
using Polly;
using ProductDetailBLL.Interfaces;
using ProductDetailDAL.Interfaces;
using RestSharp;



namespace ProductDetailBLL
{
    public class ProductDetailBLLService : IProductDetailBLLService
    {
        private readonly IProductDetailRepository productDetailRepository;
        private readonly IPublishEndpoint publishEndpoint;

        public ProductDetailBLLService(IProductDetailRepository productDetailRepository, IPublishEndpoint publishEndpoint)
        {
            this.productDetailRepository = productDetailRepository;
            this.publishEndpoint = publishEndpoint;
        }
        public async Task<ProductDetail> AddProductDetails(ProductDetail productDetails)
        {
            //throw new NotImplementedException();
            int productId = productDetails.ProductId;

            var amountToPause = TimeSpan.FromSeconds(15);
            var retryWaitPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(5, i => amountToPause, onRetry: (exception, retryCount) =>
            {
                Console.WriteLine("Error:" + exception.Message + "...Retry Count" + retryCount);
            });

            var circuitBreakerPolicy = Policy.Handle<Exception>().CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));

            var finalPolicy = retryWaitPolicy.WrapAsync(circuitBreakerPolicy);
            //-------------------------------------------

            var data = await finalPolicy.ExecuteAsync(async() =>
            {
                var canAdd = await connectToApi(productId);
                //if cannot add this details as product does not exists
                if (!canAdd)
                {
                    return null;
                }

                //if already added details not exist then 
                var alreadyAddedDetails = productDetailRepository.GetProductDetailsById(productDetails.ProductId);
                if (alreadyAddedDetails != null)
                {
                    return null;
                }

                var addingDetails = productDetailRepository.AddProductDetails(productDetails);
                return addingDetails;
            });

            return data;
        }

        public List<ProductDetail> GetAllProductDetails()
        {
            return productDetailRepository.GetAllProductDetails();
        }

        public ProductDetail GetProductDetailsById(int id)
        {
            return productDetailRepository.GetProductDetailsById(id);
        }

        public async Task<ProductDetail> RemoveProductDetails(int id)
        {
            var deleted = productDetailRepository.RemoveProductDetails(id);
            if(deleted != null)
            {
                //deleted success generate event to delete from cart also
                //publish event to delete from cart
                await publishEndpoint.Publish<ProductDetail>(deleted);
                return deleted;
            }
            return null;
        }

        //check whether product with this id is in inventory or not 
        private async Task<bool> connectToApi(int productId)
        {
            //URL to Product Service to fetch all products
            var url = "http://product-container:5001/api/Product/allProducts";

            var client = new RestClient();
            var request = new RestRequest(url,Method.Get);

            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");

            var response = await client.ExecuteAsync(request);

            if(!response.IsSuccessful)
            {
                Console.WriteLine(response.ErrorMessage);
            }


            //if success message
            var data =  response.Content;

            //deserialize data
            var productsList = JsonConvert.DeserializeObject<List<Product>>(data);

            var isValidProduct = productsList.Any(p => p.Id == productId);

            return isValidProduct;

        }
    }
}
