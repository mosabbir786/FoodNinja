using FoodNinja.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.Model
{
    public class SaveNotificationModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string ReceivedAt { get; set; }

        public string RelativeTime => TimeFormater.GetRelativeTime(ReceivedAt);

        public bool IsRead { get; set; }
    }
}
