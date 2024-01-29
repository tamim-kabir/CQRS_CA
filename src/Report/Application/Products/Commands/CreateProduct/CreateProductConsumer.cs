using Contracts;
using MassTransit;

namespace Application.Products.Commands.CreateProduct;
public class CreateProductConsumer : IConsumer<ProductCreateMessagesEvent>
{
    public Task Consume(ConsumeContext<ProductCreateMessagesEvent> context)
    {
        var con = context;
        return Task.CompletedTask;
    }
}
