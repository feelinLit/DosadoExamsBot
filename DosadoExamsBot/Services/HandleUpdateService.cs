using DosadoExamsBot.Exams;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DosadoExamsBot.Services;

public class HandleUpdateService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<HandleUpdateService> _logger;
    private Exam? _exam;

    public HandleUpdateService(ITelegramBotClient botClient, ILogger<HandleUpdateService> logger)
    {
        _botClient = botClient;
        _logger = logger;
    }

    public async Task EchoAsync(Update update)
    {
        var handler = update.Type switch
        {
            UpdateType.Message            => BotOnMessageReceived(update.Message!),
            UpdateType.CallbackQuery      => BotOnCallbackQueryReceived(update.CallbackQuery!),
            _                             => UnknownUpdateHandlerAsync(update)
        };

        try
        {
            await handler;
        }
        catch (Exception exception)
        {
            await HandleErrorAsync(exception);
        }
    }

    private async Task BotOnMessageReceived(Message message)
    {
        _logger.LogInformation("Receive message type: {messageType}", message.Type);
        if (message.Type != MessageType.Text)
            return;

        var action = message.Text!.Split(' ')[0] switch
        {
            "/MathStat"  => RequestMathStatQuestion(_botClient, message),
            "/DataBases"  => RequestDataBasesQuestion(_botClient, message),
            _           => Usage(_botClient, message)
        };
        Message sentMessage = await action;
        _logger.LogInformation("The message was sent with id: {sentMessageId}", sentMessage.MessageId);
        
        static async Task<Message> RequestMathStatQuestion(ITelegramBotClient bot, Message message)
        {
            await bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Следующий вопрос 🈯", "MathStat"),
                    InlineKeyboardButton.WithCallbackData("Закончить лютую ботку 🚬", "stop"),
                });

            return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                                                  text: "Okay let's go",
                                                  replyMarkup: inlineKeyboard);
        }
        
        static async Task<Message> RequestDataBasesQuestion(ITelegramBotClient bot, Message message)
        {
            await bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Следующий вопрос 🈯", "DataBases"),
                    InlineKeyboardButton.WithCallbackData("Закончить лютую ботку 🚬", "stop"),
                });

            return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                                                  text: "Okay let's go",
                                                  replyMarkup: inlineKeyboard);
        }

        static async Task<Message> Usage(ITelegramBotClient bot, Message message)
        {
            const string usage = "Usage:\n" +
                                 "/DataBases - случайный вопрос из экзамаена по БД\n" +
                                 "/MathStat  - случайный вопрос из экзамена по МатСтату";

            return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                                                  text: usage,
                                                  replyMarkup: new ReplyKeyboardRemove());
        }
    }

    // Process Inline Keyboard callback data
    private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery)
    {
        if (callbackQuery.Data == "MathStat")
        {
            if (_exam?.ExamName is not ExamName.MathematicalStatistics)
            {
                _exam = new Exam(ExamName.MathematicalStatistics, "MathematicalStatisticsExam.docx");
            }

            await _botClient.EditMessageTextAsync(callbackQuery.Message.Chat.Id,
                                                    callbackQuery.Message.MessageId,
                                                    _exam.GetRandomQuestion(),
                                                    replyMarkup: callbackQuery.Message.ReplyMarkup);
        }
        
        if (callbackQuery.Data == "DataBases")
        {
            if (_exam?.ExamName is not ExamName.DataBases)
            {
                _exam = new Exam(ExamName.DataBases, "DataBasesExam.docx");
            }
            
            await _botClient.EditMessageTextAsync(callbackQuery.Message.Chat.Id, 
                                                    callbackQuery.Message.MessageId, 
                                                    _exam.GetRandomQuestion(),
                                                    replyMarkup: callbackQuery.Message.ReplyMarkup);
        }
        
        if (callbackQuery.Data == "stop")
        {
            _exam = null;
            var getUsageMessage = new Message();
            getUsageMessage.Text = "Obvious bicycle";
            await BotOnMessageReceived(new Message());
        }
    }

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        _logger.LogInformation("Unknown update type: {updateType}", update.Type);
        return Task.CompletedTask;
    }

    public Task HandleErrorAsync(Exception exception)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);
        return Task.CompletedTask;
    }
}