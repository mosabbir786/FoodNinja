using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;

#if ANDROID
using AndroidX.Core.Content;
using AndroidX.Core.App;
using Android.Content.PM;
using Android;
#endif

namespace FoodNinja.Services
{
    public class NotificationPermissionHelper:INotificationPermissionManager
    {
        private const string PermissionDeniedCountKey = "NotificationPermissionDeniedCount";

        public async Task<PermissionStatus> CheckNotificationPermissionAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
            return status;
        }

        public int GetDeniedCount()
        {
            return Preferences.Get(PermissionDeniedCountKey, 0);
        }

        public async Task<PermissionStatus> RequestNotificationPermissionAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.PostNotifications>();
                int deniedCount = Preferences.Get(PermissionDeniedCountKey, 0);
                if (status != PermissionStatus.Granted)
                {
                    deniedCount++;
                    Preferences.Set(PermissionDeniedCountKey, deniedCount);
                }
                else
                {
                    Preferences.Set(PermissionDeniedCountKey, 0);
                }
            }
            return status;
        }

        public void ResetDeniedCount()
        {
            Preferences.Set(PermissionDeniedCountKey, 0);
        }
    }
}
