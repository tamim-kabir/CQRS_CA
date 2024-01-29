using Application.Abstractions.Messaging.Command;

namespace Application.Products.Commands.CreateProduct;
public record CreateProductCommand(string Name, int Quentity, string Size) : ICommand;
