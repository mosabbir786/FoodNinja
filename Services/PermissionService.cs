using Microsoft.Maui.Controls.PlatformConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
#if ANDROID
using Android.App;
using Android.Content.PM;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Microsoft.Maui.ApplicationModel;
#endif
#if IOS
using Photos;
#endif

using Microsoft.Maui.Controls;

namespace FoodNinja.Services
{
    public class PermissionService 
    {
        private const string PermissionDeniedCountKey = "PermissionDeniedCount";

        public async Task<bool> CheckPhotoPermission()
       {
#if ANDROID
            var currentActivity = Platform.CurrentActivity;
            if(currentActivity == null)
                return false;

            Permission status;
            if(DeviceInfo.Version.Major >= 13)
            {
                status = ContextCompat.CheckSelfPermission(currentActivity, Android.Manifest.Permission.ReadMediaImages);
            }
            else
            {
                status = ContextCompat.CheckSelfPermission(currentActivity, Android.Manifest.Permission.ReadExternalStorage);
            }
            return status == Permission.Granted;
#elif IOS
            var status = PHPhotoLibrary.AuthorizationStatus;
            return status == PHAuthorizationStatus.Authorized;
#else
            return false;
#endif
        }

        public async Task<bool> RequestPhotoPermission()
        {
#if ANDROID
            var currentActivity = Platform.CurrentActivity;
            if (currentActivity == null)
                return false;

            Permission status;

            if (DeviceInfo.Version.Major >= 13)
            {
                status = ContextCompat.CheckSelfPermission(currentActivity, Android.Manifest.Permission.ReadMediaImages);
                if (status != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(currentActivity, new string[] { Android.Manifest.Permission.ReadMediaImages }, 100);
                }
            }
            else
            {
                status = ContextCompat.CheckSelfPermission(currentActivity, Android.Manifest.Permission.ReadExternalStorage);
                if (status != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(currentActivity, new string[] { Android.Manifest.Permission.ReadExternalStorage }, 100);
                }
            }

            status = DeviceInfo.Version.Major >= 13 ?
            ContextCompat.CheckSelfPermission(currentActivity, Android.Manifest.Permission.ReadMediaImages) :
            ContextCompat.CheckSelfPermission(currentActivity, Android.Manifest.Permission.ReadExternalStorage);

            if (status != Permission.Granted)
            {
                IncrementPermissionDeniedCount();
            }
            return status == Permission.Granted;

#elif IOS
            var status = PHPhotoLibrary.AuthorizationStatus;
            if (status != PHAuthorizationStatus.Authorized)
            {
                var result = await PHPhotoLibrary.RequestAuthorizationAsync();
                return result == PHAuthorizationStatus.Authorized;
            }
            return true;
#else
            return false;
#endif
        }

        private void IncrementPermissionDeniedCount()
        {
            int currentCount = GetPermissionDeniedCount();
            SetPermissionDeniedCount(currentCount + 1);
        }
        public int GetPermissionDeniedCount()
        {
            return Preferences.Get(PermissionDeniedCountKey, 0);
        }

        private void SetPermissionDeniedCount(int value)
        {
            Preferences.Set(PermissionDeniedCountKey, value);
        }
    }
}
