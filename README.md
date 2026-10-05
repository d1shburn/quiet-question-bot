# QuietQuestion

Simple anonymous questions Telegram bot built with C# and .NET.

Users can share their personal link and receive anonymous questions from other users.

## Features

- Anonymous questions
- Personal links
- Environment-based configuration

## Setup

Clone the repository and install dependencies:

```bash
git clone https://github.com/d1shburn/QuietQuestion.git
cd QuietQuestion
dotnet restore
```

Set environment variables:

```bash
export TELEGRAM_BOT_TOKEN="your_bot_token"
export TELEGRAM_BOT_USERNAME="your_bot_username"
```

Run the bot:

```bash
dotnet run
```

## Tech Stack

- C#
- .NET
- Telegram.Bot