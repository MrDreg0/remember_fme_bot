using RememberFMeBot.WebApi.Settings;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

var botConfigSection = builder.Configuration.GetSection("BotConfig");
var botConfig = botConfigSection.Get<BotConfig>();

builder.Services
 .AddOptions()
 .Configure<BotConfig>(botConfigSection);

builder.Services.AddHttpClient("telegram_bot_client")
 .AddTypedClient<ITelegramBotClient>(httpClient =>
  {
    var options = new TelegramBotClientOptions(botConfig.BotToken);
    return new TelegramBotClient(options, httpClient);
  });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();