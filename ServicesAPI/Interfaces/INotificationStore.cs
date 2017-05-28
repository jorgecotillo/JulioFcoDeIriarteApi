using ServicesAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicesAPI.Interfaces
{
    public interface INotificationStore
    {
        Task SaveNotification(NotificationModel notification);
        Task<IEnumerable<NotificationModel>> GetAllNotifications(
            DateTime? from = null,
            int numberOfRecords = 10);
    }
}
