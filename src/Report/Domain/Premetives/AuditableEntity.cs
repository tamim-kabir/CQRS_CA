namespace Domain.Premetives;
public class AuditableEntity
{
    public string CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedIPAddress { get; set; }
    public string LastModifiedBy { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public string ModifiedIPAddress { get; set; }
    public bool Deleted { get; set; }
    public string DeletedBy { get; set; }
    public DateTime? DeletedTime { get; set; }
    public string DeletedIPAddress { get; set; }
}
