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
    public class ProductDetailDeleteConsumer : IConsumer<ProductDetail>
    {
        private readonly ICartBLLService cartBLLService;
        public ProductDetailDeleteConsumer(ICartBLLService cartBLLService)
        {
            this.cartBLLService = cartBLLService;
        }

        public Task Consume(ConsumeContext<ProductDetail> context)
        {
            var productReceived = context.Message;
            var productId = productReceived.ProductId;

            //removed all products for this ID from all carts
            cartBLLService.RemoveFromAllCarts(productId);

            return Task.CompletedTask;
        }
    }
}
