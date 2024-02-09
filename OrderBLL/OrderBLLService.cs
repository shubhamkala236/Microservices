using Common;
using MassTransit;
using MassTransit.Transports;
using Newtonsoft.Json;
using OrderBLL.Interfaces;
using OrderDAL.Interfaces;
using Polly;
using RestSharp;


namespace OrderBLL
{
    public class OrderBLLService : IOrderBLLService
    {
        private readonly IOrderRepository orderRepository;
        private readonly IPublishEndpoint publishEndpoint;
        public OrderBLLService(IOrderRepository orderRepository, IPublishEndpoint publishEndpoint)
        {
            this.orderRepository = orderRepository;
            this.publishEndpoint = publishEndpoint;
        }

        public async Task<Order> createMyOrder(int userId, int productId, string authToken)
        {
            //1. call cart service to get this product from cart (HTTP)
            //2. create Order
            //3. Remove from cart Service for this userId (EVENT) producer
            //4. Deduct quantiy from Product Service for this productID (EVENT) producer

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
            var response = await finalPolicy.ExecuteAsync(async () =>
            {
                var cartItemsList = await connectToCartService(authToken);
                
                var cartitem = cartItemsList.FirstOrDefault(item => item.ProductId==productId && item.UserId==userId);

                return cartitem;
            });

            //if not in cart
            if(response == null)
            {
                return null;
            }

            //if valid cart item --> create order
            Order order = new Order()
            {
                ProductId = response.ProductId,
                UserId = response.UserId,
                Name = response.Name,
                Quantity = response.Quantity,
                Description = response.Description,
                TotalPrice = response.Quantity * response.Price,
            };

            var orderCreated = orderRepository.createMyOrder(order);
            if(orderCreated == null)
            {
                return null;
            }
            //Event to remove from my cart and reduce product quantity from Product service (publish)
            await publishEndpoint.Publish<Order>(orderCreated);

            return orderCreated;

        }

        public List<Order> getMyOrders(int userId)
        {
            var myOrders = orderRepository.getMyOrders(userId);
            return myOrders;
        }

        private async Task<List<CartItem>> connectToCartService(string authToken)
        {
            //fetch all cartItems
            var url = $"http://cart-container:5003/api/Cart/myCart";

            var client = new RestClient();
            var request = new RestRequest(url, Method.Get);

            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"{authToken}");


            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                Console.WriteLine(response.ErrorMessage);
            }


            //if success message
            var data = response.Content;

            //deserialize data
            var myCartList = JsonConvert.DeserializeObject<List<CartItem>>(data);

            return myCartList;

        }
    }
}
