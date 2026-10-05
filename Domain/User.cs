namespace QuietQuestion.Domain;

public class User
{
    public long Id { get; }
    public string Username { get; }
    public string LinkToken { get; }

    public User(long id, string username, string linkToken)
    {
        Id = id;
        Username = username;
        LinkToken = linkToken;
    }
}