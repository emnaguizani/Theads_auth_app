
public class User
{
    public GUID int Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }= DateTime.UtcNow;

    public User()
    {
        CreatedAt = DateTime.Now;
    }

    public override string ToString()
    {
        return $"{Name} ({Email}) - Created at: {CreatedAt}";
    }
}