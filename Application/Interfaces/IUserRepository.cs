using QuietQuestion.Domain;

namespace QuietQuestion.Application.Interfaces;

public interface IUserRepository
{
    User? GetById(long id);

    User? GetByToken(string token);
    
    void Add(User user);
}