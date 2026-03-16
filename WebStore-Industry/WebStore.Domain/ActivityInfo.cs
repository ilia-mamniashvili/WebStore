namespace WebStore.Domain;

public class ActivityInfo
{
    public bool IsActibe { get; set; } = true;
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdateDate { get; set; }
    public bool IsActive { get; set; }
}