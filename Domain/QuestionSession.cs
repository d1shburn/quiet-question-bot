namespace QuietQuestion.Domain;

public class QuestionSession
{
    public long SenderId { get; }
    public long RecipientId { get; }

    public QuestionSession(long senderId, long recipientId)
    {
        SenderId = senderId;
        RecipientId = recipientId;
    }
}