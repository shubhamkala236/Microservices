using Common;
using MassTransit;
using ProductBLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductBLL
{
    public class OrderPlacedConsumer : IConsumer<CartItem>
    {
        private readonly IProductBLLService productBLLService;
        public OrderPlacedConsumer(IProductBLLService productBLLService)
        {
            this.productBLLService = productBLLService;
        }

        public Task Consume(ConsumeContext<CartItem> context)
        {
            var orderReceived = context.Message;
            var productId = orderReceived.ProductId;
            var orderedQuantity = orderReceived.Quantity;

            //deduct product quantity
            productBLLService.DeductQuantity(productId, orderedQuantity);

            return Task.CompletedTask;
        }

    }
}
