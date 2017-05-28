using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.NotificationHubs;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using ServicesAPI.Models;
using MongoDB.Bson;
using ServicesAPI.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ServicesAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    //[Authorize]
    public class ManageController : Controller
    {
        readonly IConfigurationRoot _configuration;
        readonly INotificationStore _notificationStore;
        readonly INotificationHubStore _notificationHubStore;

        public ManageController(
            IConfigurationRoot configuration, 
            INotificationStore notificationStore,
            INotificationHubStore notificationHubStore)
        {
            _configuration = configuration;
            _notificationStore = notificationStore;
            _notificationHubStore = notificationHubStore;
        }

        [Route("notification")]
        [HttpPost]
        public async Task<JsonResult> SendNotification([FromBody]NotificationModel notification)
        {
            if (ModelState.IsValid)
            {
                await _notificationStore
                    .SaveNotification(notification: notification);

                await _notificationHubStore
                    .SendNotificationHub(notification);
                
                return Json(new { message = "Ok" });
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
                return Json(new { message = "Invalid values received" });
            }
        }

        [Route("notification")]
        [HttpGet]
        public async Task<IEnumerable<NotificationModel>> Get()
        {
            return 
                await _notificationStore.GetAllNotifications();
        }
    }
}
