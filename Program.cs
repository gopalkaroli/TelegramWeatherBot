using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramSupportBot;

var botToken = "7823158082:AAFs2HBNScSluoEQskoWDcH5mi6w8Q"; // Replace with your actual token
var botClient = new TelegramBotClient(botToken);

using var cts = new CancellationTokenSource();

// Setup update handler
var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = new[] { UpdateType.Message } // Only process messages
};

botClient.StartReceiving(
   updateHandler: HandleUpdateAsync,
   errorHandler: HandleErrorAsync, // Correct parameter name
   receiverOptions: receiverOptions,
   cancellationToken: cts.Token
);

var me = await botClient.GetMe();
Console.WriteLine($"🤖 Bot started: @{me.Username}");
Console.ReadLine();
cts.Cancel();

// ==== Update Handler ====
async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken token)
{


    if (update.Message is not { Type: MessageType.Text } message || message.Text == null)
        return;

    Console.WriteLine($"📩 Received from {message.Chat.Id}: {message.Text}");

    string reply;
    ReplyMarkup? keyboard = null; // Corrected type to ReplyMarkup

    switch (message.Text.ToLower())
    {
        case "/start":
            reply = "👋 Welcome to WeatherBot!\nType any city name to get live weather.\n\nOr click the button below.";    
            break;
        case "/help":
            reply = "🛠️ Available Commands:\n/help - List commands\n/start - Restart the bot\n\nType any city name to get weather.";
            break;

        case "check weather 🌤️":
            reply = "Please type your city name (e.g., *London* or *Delhi*) to get current weather.";
            break;

        default:
            reply = await WeatherService.GetWeatherAsync(message.Text);
            break;
    }

    await bot.SendMessage( // Corrected method name
        chatId: message.Chat.Id,
        text: reply,
        replyMarkup: keyboard,
        parseMode: ParseMode.Markdown,
        cancellationToken: token
    );
}
// ==== Error Handler ====

Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken token)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
    return Task.CompletedTask;
}
