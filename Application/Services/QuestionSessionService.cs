using QuietQuestion.Application.Interfaces;
using QuietQuestion.Domain;

namespace QuietQuestion.Application.Services;

public class QuestionSessionService
{
    private readonly IQuestionSessionRepository _repository;

    public QuestionSessionService(IQuestionSessionRepository repository) =>
        _repository = repository;

    public void StartSession(long senderId, long recipientId)
    {
        var session = new QuestionSession(
            senderId,
            recipientId
        );

        _repository.Add(session);
    }

    public QuestionSession? GetSession(long senderId) =>
        _repository.GetBySenderId(senderId);

    public void EndSession(long senderId) =>
        _repository.Remove(senderId);
}