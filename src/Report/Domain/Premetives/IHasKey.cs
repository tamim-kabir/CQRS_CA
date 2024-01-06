using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Premetives;
public interface IHasKey<T>
{
    T Id { get; set; }
}
