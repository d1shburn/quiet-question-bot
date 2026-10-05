using QuietQuestion.Application.Services;
using QuietQuestion.Configuration;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace QuietQuestion.Infrastructure.Telegram;

public class TelegramBot
{
    private readonly TelegramBotClient _bot;
    private readonly BotOptions _options;
    private readonly UserService _userService;
    private readonly QuestionService _questionService;
    private readonly QuestionSessionService _questionSessionService;

    public TelegramBot(
        BotOptions options,
        UserService userService,
        QuestionService questionService,
        QuestionSessionService questionSessionService)
    {
        _options = options;
        _bot = new TelegramBotClient(options.Token);
        _userService = userService;
        _questionService = questionService;
        _questionSessionService = questionSessionService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        _bot.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );

        var me = await _bot.GetMe(cancellationToken);

        Console.WriteLine($"@{me.Username} started.");
    }

    private async Task HandleUpdateAsync(
        ITelegramBotClient bot,
        Update update,
        CancellationToken cancellationToken)
    {
        if (update.Message?.Text is not { } text)
            return;

        var message = update.Message;
        var telegramUser = message.From;

        if (telegramUser is null)
            return;

        if (text.StartsWith("/start"))
        {
            await HandleStartAsync(
                message,
                text,
                cancellationToken
            );

            return;
        }

        if (text.Equals("/cancel", StringComparison.OrdinalIgnoreCase))
        {
            await HandleCancelAsync(
                message,
                cancellationToken
            );
            
            return;
        }

        await HandleQuestionAsync(
            message,
            text,
            cancellationToken
        );
    }

    private async Task HandleStartAsync(
        Message message,
        string text,
        CancellationToken cancellationToken)
    {
        var telegramUser = message.From!;

        var user = _userService.GetOrCreate(
            telegramUser.Id,
            telegramUser.Username ?? telegramUser.FirstName
        );

        var parts = text.Split(
            ' ',
            StringSplitOptions.RemoveEmptyEntries
        );

        if (parts.Length == 1)
        {
            await SendPersonalLinkAsync(
                message.Chat.Id,
                user.LinkToken,
                cancellationToken
            );

            return;
        }

        var token = parts[1];

        var recipient = _userService.GetByToken(token);

        if (recipient is null)
        {
            await _bot.SendMessage(
                message.Chat.Id,
                "User not found.",
                cancellationToken: cancellationToken
            );

            return;
        }

        if (recipient.Id == telegramUser.Id)
        {
            await _bot.SendMessage(
                message.Chat.Id,
                "You can't send a question to yourself.",
                cancellationToken: cancellationToken
            );

            return;
        }

        _questionSessionService.StartSession(
            telegramUser.Id,
            recipient.Id
        );

        await _bot.SendMessage(
            message.Chat.Id,
            "Send your anonymous question in one message.\n\n" +
            "Use /cancel to cancel.",
            cancellationToken: cancellationToken
        );
    }

    private async Task HandleQuestionAsync(
        Message message,
        string text,
        CancellationToken cancellationToken)
    {
        var senderId = message.From!.Id;

        var session = _questionSessionService.GetSession(senderId);

        if (session is null)
        {
            await SendPersonalLinkAsync(
                message.Chat.Id,
                _userService.GetById(senderId)!.LinkToken,
                cancellationToken
            );

            return;
        }

        var question = _questionService.Create(
            session.RecipientId,
            text
        );

        await _bot.SendMessage(
            question.RecipientId,
            $"📩 Anonymous question:\n\n{question.Text}",
            cancellationToken: cancellationToken
        );

        _questionSessionService.EndSession(senderId);

        await _bot.SendMessage(
            message.Chat.Id,
            "✨ Your question was sent anonymously.",
            cancellationToken: cancellationToken
        );
    }

    private async Task HandleCancelAsync(
        Message message,
        CancellationToken cancellationToken)
    {
        var senderId = message.From!.Id;

        var session = _questionSessionService.GetSession(senderId);

        if (session is null)
        {
            await _bot.SendMessage(
                message.Chat.Id,
                "You don't have an active question.",
                cancellationToken: cancellationToken
            );

            return;
        }

        _questionSessionService.EndSession(senderId);

        await _bot.SendMessage(
            message.Chat.Id,
            "🚫 Question sending cancelled.",
            cancellationToken: cancellationToken
        );
    }

    private async Task SendPersonalLinkAsync(
        long chatId,
        string token,
        CancellationToken cancellationToken)
    {
        var link = $"https://t.me/{_options.Username}?start={token}";

        await _bot.SendMessage(
            chatId,
            $"🔗 Your personal link:\n\n{link}\n\n" +
            "Share it with someone so they can send you an anonymous question.",
            cancellationToken: cancellationToken
        );
    }

    private Task HandleErrorAsync(
        ITelegramBotClient bot,
        Exception exception,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Telegram error: {exception}");
        return Task.CompletedTask;
    }
}