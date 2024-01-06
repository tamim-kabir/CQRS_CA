using Application.Abstractions.Messaging.Command;
using Domain.Repositories.IProduct;
using Domain.Shared;

namespace Application.Products.Commands.CreateProduct;
internal sealed class CreateProductCommandHandler : ICommandHandler<CreateProductCommand>
{
    private readonly IProductRepository _repository;
    public CreateProductCommandHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        await _repository.CreateProduct();

        return Result.Success();
    }
}
