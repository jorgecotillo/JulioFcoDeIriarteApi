using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ServicesAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicesAPI.Models
{
    public class NotificationHubStoreModel : INotificationHubStore
    {
        readonly IConfigurationRoot _configuration;

        public NotificationHubStoreModel(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        public async Task SendNotificationHub(NotificationModel notification)
        {
            var connString = _configuration["Hub:ConnectionString"];
            var name = _configuration["Hub:Name"];

            NotificationHubClient hub =
                    NotificationHubClient
                    .CreateClientFromConnectionString(connString, name);

            var message = new
            {
                data = new
                {
                    message = notification.Title
                }
            };

            await hub
                .SendGcmNativeNotificationAsync(JsonConvert.SerializeObject(message));
        }
    }
}
