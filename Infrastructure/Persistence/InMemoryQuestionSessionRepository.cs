using QuietQuestion.Application.Interfaces;
using QuietQuestion.Domain;

namespace QuietQuestion.Infrastructure.Persistence;

public class InMemoryQuestionSessionRepository : IQuestionSessionRepository
{
    private readonly List<QuestionSession> _sessions = [];

    public QuestionSession? GetBySenderId(long senderId) =>
        _sessions.FirstOrDefault(session => session.SenderId == senderId);

    public void Add(QuestionSession session)
    {
        Remove(session.SenderId);

        _sessions.Add(session);
    }

    public void Remove(long senderId)
    {
        var session = GetBySenderId(senderId);

        if (session is not null)
            _sessions.Remove(session);
    }
}