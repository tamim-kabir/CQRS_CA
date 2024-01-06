using Domain.Premetives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;
public class Product : BaseEntity<int>
{
    public string Name { get; set; }
    public decimal Quentity { get; set; }
    public decimal Size { get; set; }
}
