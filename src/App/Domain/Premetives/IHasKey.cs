namespace Domain.Premetives;
public interface IHasKey<T>
{
    T Id { get; set; }
}
