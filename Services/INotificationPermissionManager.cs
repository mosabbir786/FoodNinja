using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.Services
{
    public interface INotificationPermissionManager
    {
        Task<PermissionStatus> CheckNotificationPermissionAsync();
        Task<PermissionStatus> RequestNotificationPermissionAsync();
        int GetDeniedCount(); 
        void ResetDeniedCount();
    }
}
