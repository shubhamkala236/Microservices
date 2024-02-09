using Common;
using MassTransit;
using Microsoft.Extensions.Logging;
using ProductDetailBLL.Interfaces;
using ProductDetailDAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductDetailBLL
{
    public class DeleteProductConsumer : IConsumer<Product>
    {
        private readonly IProductDetailBLLService productDetailBLLService;
        public DeleteProductConsumer(IProductDetailBLLService productDetailBLLService)
        {
            this.productDetailBLLService = productDetailBLLService;
        }
        public Task Consume(ConsumeContext<Product> context)
        {
            var productReceived = context.Message;
            var productId = productReceived.Id;

            //removed product
            var deletedProduct = productDetailBLLService.RemoveProductDetails(productId);
            if(deletedProduct == null)
            {
                Console.WriteLine("Product Details to delete not found");
            }
            Console.Write("Product details deleted");

            return Task.CompletedTask;
        }
    }
}
