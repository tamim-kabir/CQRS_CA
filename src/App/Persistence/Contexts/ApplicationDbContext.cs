using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence.Abstractions.Context;

namespace Persistence.Contexts;
public class ApplicationDbContext : DbContextExtention
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options, httpContextAccessor)
    {
    }
    public DbSet<Product> Product { get; set; }    
}
