using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.NotificationHubs;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ServicesAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    //[Authorize]
    public class ManageController : Controller
    {
        public async Task<JsonResult> SendNotification(string messageToBroadcast)
        {
            NotificationHubClient hub =
                NotificationHubClient
                .CreateClientFromConnectionString("<connection string with full access>", "<hub name>");

            var message = new
            {
                data = new
                {
                    message = messageToBroadcast
                }
            };

            await hub.SendGcmNativeNotificationAsync(JsonConvert.SerializeObject(message));

            return Json(new { message = "Ok" });
        }
    }
}
