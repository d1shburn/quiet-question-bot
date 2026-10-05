using QuietQuestion.Application.Interfaces;
using QuietQuestion.Domain;

namespace QuietQuestion.Infrastructure.Persistence;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = [];

    public User? GetById(long id) =>
        _users.FirstOrDefault(user => user.Id == id);

    public User? GetByToken(string token) =>
        _users.FirstOrDefault(user => user.LinkToken == token);

    public void Add(User user) =>
        _users.Add(user);
}