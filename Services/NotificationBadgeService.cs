using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.Services
{
    public class NotificationBadgeService
    {
        public int NotificationCount { get; private set; }

        public void SetNotificationCount(int count)
        {
            NotificationCount = count;
        }

        public int GetNotificationCount()
        {
            return NotificationCount;
        }
    }
}
