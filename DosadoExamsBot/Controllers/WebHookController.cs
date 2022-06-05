using DosadoExamsBot.Services;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace DosadoExamsBot.Controllers;

public class WebHookController : ControllerBase
{
    // Telegram will use this method to deliver an update caused by user through the webhook
    [HttpPost]
    public async Task<IActionResult> PostUpdate([FromServices] HandleUpdateService handleUpdateService,
        [FromBody] Update update)
    {
        await handleUpdateService.EchoAsync(update);
        return Ok();
    }
}