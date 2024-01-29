using Domain.Premetives;

namespace Domain.Entities;
public class Product : BaseEntity<int>
{
    public string Name { get; set; }
    public decimal Quentity { get; set; }
    public decimal Size { get; set; }
}
