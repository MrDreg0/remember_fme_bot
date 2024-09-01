using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RememberFMeBot.WebApi.Settings;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RememberFMeBot.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TelegramBotController(
  ITelegramBotClient botClient,
  IOptions<BotConfig> botConfig,
  ILogger<TelegramBotController> logger)
  : ControllerBase
{
  private readonly BotConfig _botConfig = botConfig.Value;

  [HttpPost]
  public async Task<IActionResult> Post([FromBody] Update update)
  {
    if (Request.Headers["X-Telegram-Bot-Api-Secret-Token"] != _botConfig.SecretToken)
    {
      logger.LogWarning(
        "Несанкционированный запрос с ip {IP} и данными '{RequestMessage}'.",
        Request.HttpContext.Connection.RemoteIpAddress,
        update?.Message?.Text ?? JsonSerializer.Serialize(update));

      return NoContent();
    }

    return Ok();
  }

  [HttpPost("webhook")]
  public async Task<IActionResult> SetWebhook([FromBody] string webhookUrl, [FromQuery]bool needDropPendingUpdates = false)
  {
    await botClient.SetWebhookAsync(webhookUrl, secretToken: _botConfig.SecretToken, dropPendingUpdates: needDropPendingUpdates);

    return Ok($"Webhook set to '{webhookUrl}'");
  }

  [HttpGet("webhook")]
  public async Task<ActionResult<WebhookInfo>> GetWebhookInfo()
  {
    var webhookInfo = await botClient.GetWebhookInfoAsync();

    return Ok(webhookInfo);
  }

  [HttpDelete("webhook")]
  public async Task<IActionResult> DeleteWebhook()
  {
    await botClient.DeleteWebhookAsync();

    return Ok("Webhook deleted");
  }
}