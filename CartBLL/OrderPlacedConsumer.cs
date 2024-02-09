using CartBLL.Interfaces;
using Common;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartBLL
{
    public class OrderPlacedConsumer : IConsumer<Order>
    {
        private readonly ICartBLLService cartBLLService;
        private readonly IPublishEndpoint publishEndpoint;
        public OrderPlacedConsumer(ICartBLLService cartBLLService, IPublishEndpoint publishEndpoint)
        {
            this.cartBLLService = cartBLLService;
            this.publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<Order> context)
        {
            var orderReceived = context.Message;
            var productId = orderReceived.ProductId;
            var myUserId = orderReceived.UserId;

            //removed products for this ID from my cart
            var removedFromCart = cartBLLService.RemovefromMyCart(myUserId, productId);

            //Publish event to deduct quantity from product service as order is placed
            await publishEndpoint.Publish<CartItem>(removedFromCart);

        }
    }
}
