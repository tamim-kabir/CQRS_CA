namespace Contracts;

public record ProductCreateMessagesEvent
{
    public int Id { get; set; }
    public string? ProductName { get; set; }
    public decimal Quentity { get; set; }
}
