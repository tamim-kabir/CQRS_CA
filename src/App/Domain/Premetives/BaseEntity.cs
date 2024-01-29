using System.ComponentModel.DataAnnotations;

namespace Domain.Premetives;
public abstract class BaseEntity<T> : AuditableEntity, IHasKey<T>
{
    [Key]
    public T Id { get; set; }
}
