using Application.Abstractions.Messaging.Command;
using Contracts;
using Domain.Repositories.IProduct;
using Domain.Shared;
using MassTransit;

namespace Application.Products.Commands.CreateProduct;
internal sealed class CreateProductCommandHandler : ICommandHandler<CreateProductCommand>
{
    private readonly IProductRepository _repository;
    private readonly IPublishEndpoint _endpoint;
    public CreateProductCommandHandler(IProductRepository repository, IBus bus)
    {
        _repository = repository;
        _endpoint = bus;
    }

    public async Task<Result> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        await _repository.CreateProduct();
        await _endpoint.Publish(new ProductCreateMessagesEvent
        {
            Id = 1,
            ProductName = request.Name,
            Quentity = request.Quentity,
        }, cancellationToken);
        return Result.Success();
    }
}
