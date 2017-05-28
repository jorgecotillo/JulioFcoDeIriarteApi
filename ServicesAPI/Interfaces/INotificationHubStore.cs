using ServicesAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicesAPI.Interfaces
{
    public interface INotificationHubStore
    {
        Task SendNotificationHub(NotificationModel notification);
    }
}
