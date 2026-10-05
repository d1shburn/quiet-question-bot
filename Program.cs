using QuietQuestion.Application.Services;
using QuietQuestion.Configuration;
using QuietQuestion.Infrastructure.Persistence;
using QuietQuestion.Infrastructure.Telegram;

var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
var username = Environment.GetEnvironmentVariable("TELEGRAM_BOT_USERNAME");

if (string.IsNullOrWhiteSpace(token))
    throw new InvalidOperationException("TELEGRAM_BOT_TOKEN is not set.");

if (string.IsNullOrWhiteSpace(username))
    throw new InvalidOperationException("TELEGRAM_BOT_USERNAME is not set.");

var options = new BotOptions
{
    Token = token,
    Username = username
};

var userRepository = new InMemoryUserRepository();
var questionSessionRepository = new InMemoryQuestionSessionRepository();

var userService = new UserService(userRepository);
var questionService = new QuestionService();

var questionSessionService = new QuestionSessionService(
    questionSessionRepository
);

var bot = new TelegramBot(
    options,
    userService,
    questionService,
    questionSessionService
);

using var cts = new CancellationTokenSource();

Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cts.Cancel();
};

await bot.StartAsync(cts.Token);
await Task.Delay(Timeout.Infinite, cts.Token);