using Application.Abstractions.Messaging.Command;
using MediatR;

namespace Application.Products.Commands.CreateProduct;
public record CreateProductCommand(string Name, int Quentity, string Size) : ICommand;
