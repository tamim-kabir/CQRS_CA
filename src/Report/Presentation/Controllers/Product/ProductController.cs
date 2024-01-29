﻿using Application.Products.Commands.CreateProduct;
using Application.Products.Queries.GetAllProducts;
using Application.Products.Queries.GetProductById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstractions;

namespace Presentation.Controllers.Product;
[Route("api/product")]
public sealed class ProductController(ISender sender) : ApiContoller(sender)
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id, CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        return Result(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetProductAllProduct(CancellationToken cancellationToken)
    {
        var query = new GetAllProductsQuery();
        var result = await _sender.Send(query, cancellationToken);

        return Result(result);
    }
}
