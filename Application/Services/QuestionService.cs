using QuietQuestion.Domain;

namespace QuietQuestion.Application.Services;

public class QuestionService
{
    public Question Create(long recipientId, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Question cannot be empty.", nameof(text));

        return new Question(recipientId, text);
    }
}