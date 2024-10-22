namespace MainPortfolio.Models;

public class Connection
{
    public Connection(Guid userId, string signalrId)
    {
        UserId = userId;
        SignalrId = signalrId;
        TimeStamp = DateTime.UtcNow;
    }
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string SignalrId { get; set; } = null!;
    public DateTime? TimeStamp { get; set; }
    public virtual User User { get; set; } = null!;
}
