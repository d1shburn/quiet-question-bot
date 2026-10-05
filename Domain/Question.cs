namespace QuietQuestion.Domain;

public class Question
{
    public long RecipientId { get; }
    public string Text { get; }

    public Question(long recipientId, string text)
    {
        RecipientId = recipientId;
        Text = text;
    }
}