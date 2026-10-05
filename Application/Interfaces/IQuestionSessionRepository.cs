using QuietQuestion.Domain;

namespace QuietQuestion.Application.Interfaces;

public interface IQuestionSessionRepository
{
    QuestionSession? GetBySenderId(long senderId);

    void Add(QuestionSession session);

    void Remove(long senderId);
}