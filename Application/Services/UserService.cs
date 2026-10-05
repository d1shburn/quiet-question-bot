using QuietQuestion.Application.Interfaces;
using QuietQuestion.Domain;

namespace QuietQuestion.Application.Services;

public class UserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository) =>
        _repository = repository;

    public User GetOrCreate(long id, string username)
    {
        var existingUser = _repository.GetById(id);

        if (existingUser is not null)
            return existingUser;

        var user = new User(
            id,
            username,
            Guid.NewGuid().ToString("N")
        );

        _repository.Add(user);
        return user;
    }

    public User? GetByToken(string token) =>
        _repository.GetByToken(token);

    public User? GetById(long id) =>
        _repository.GetById(id);
}