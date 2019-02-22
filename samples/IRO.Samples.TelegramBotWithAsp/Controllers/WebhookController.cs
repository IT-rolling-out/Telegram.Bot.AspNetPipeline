using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IRO.Samples.TelegramBotWithAsp.Controllers
{
    [Route("")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        // GET: api/Webhook
        [HttpGet]
        public string Get()
        {
            return "Webhook controller.";
        }
    }
}
