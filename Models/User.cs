namespace MainPortfolio.Models;

public class User
{
    public Guid Id { get; set; }
    public sbyte Role { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public virtual ICollection<Connection> Connections { get; set; } = new List<Connection>();
    public User(sbyte role, string name, string email, string passwordHash)
    {
        Role = role;
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
    }
}

public class UserAuthDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class UserRegistrDto : UserAuthDto
{
    public string Name { get; set; } = null!;
}

public class UserSignalrDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string SignalrId { get; set; }
    public UserSignalrDto(Guid id, string name, string signalrId)
    {
        Id = id; Name = name; SignalrId = signalrId;
    }
}