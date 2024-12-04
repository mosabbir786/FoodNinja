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
using UIKit;
using Photos;
#endif

using Microsoft.Maui.Controls;

namespace FoodNinja.Services
{
    public class PermissionService 
    {
        private const string PermissionDeniedCountKey = "PermissionDeniedCount";
        private const string DeniedCountKey = "CameraPermissionDeniedCount";

        public async Task<bool> CheckPhotoPermission()
       {
            await Task.Delay(1);
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
            await Task.Delay(1);
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
                    MainActivity.PermissionTcs = new TaskCompletionSource<bool>();
                    ActivityCompat.RequestPermissions(currentActivity, new string[] { Android.Manifest.Permission.ReadMediaImages }, 100);
                    var result = await MainActivity.PermissionTcs.Task;
                    if (!result)
                    {
                        IncrementPermissionDeniedCount();
                    }
                    return result;
                }
            }
            else
            {
                status = ContextCompat.CheckSelfPermission(currentActivity, Android.Manifest.Permission.ReadExternalStorage);
                if (status != Permission.Granted)
                {
                    MainActivity.PermissionTcs = new TaskCompletionSource<bool>();
                    ActivityCompat.RequestPermissions(currentActivity, new string[] { Android.Manifest.Permission.ReadExternalStorage }, 100);
                    var result = await MainActivity.PermissionTcs.Task;
                    if (!result)
                    {
                        IncrementPermissionDeniedCount();
                    }
                    return result;
                }
            }
            return true;

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
        public async Task<bool> CheckCameraPermission()
        {
            await Task.Delay(1);
#if ANDROID

            var currentActivity = Platform.CurrentActivity;
            if (currentActivity == null)
                return false;

            // Check if camera permission is granted
            var status = ContextCompat.CheckSelfPermission(currentActivity, Android.Manifest.Permission.Camera);
            return status == Permission.Granted;
#else
            return false;
#endif

#if IOS
           var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
           return status == PermissionStatus.Granted;
#endif
        }
        public async Task<bool> RequestCameraPermission()
        {
            await Task.Delay(1);
#if ANDROID
            var currentActivity = Platform.CurrentActivity;
            if (currentActivity == null)
                return false;

            // Check current permission status
            var status = ContextCompat.CheckSelfPermission(currentActivity, Android.Manifest.Permission.Camera);
            if (status != Permission.Granted)
            {
                // Create a TaskCompletionSource to wait for the permission result
                MainActivity.PermissionTcs = new TaskCompletionSource<bool>();
        
                // Request camera permission
                ActivityCompat.RequestPermissions(currentActivity, new string[] { Android.Manifest.Permission.Camera }, 101);
        
                // Wait for the user to respond to the permission dialog
                var result = await MainActivity.PermissionTcs.Task;
        
                // If permission is denied, increment the denied count
                if (!result)
                {
                    IncrementDeniedCount();

                }
                return result; // Return the result of the permission request
            }
            return true; // Permission is already granted
#else
            return false;
#endif

#if IOS
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if(status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();

                if (status != PermissionStatus.Granted)
                {
                    // Increment denied count if permission is not granted
                    IncrementDeniedCount();
                }
            }
            return status == PermissionStatus.Granted;
#endif
        }



        #region For Image Permission
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
        #endregion

        #region For Camera Permission
        private void IncrementDeniedCount()
        {
            int deniedCount = Preferences.Get(DeniedCountKey, 0);
            Preferences.Set(DeniedCountKey, deniedCount + 1);
        }
        private void ResetDeniedCount()
        {
            Preferences.Set(DeniedCountKey, 0);
        }
        public int GetCameraPermissionDeniedCount()
        {
            return Preferences.Get(DeniedCountKey, 0);
        }
        #endregion
    }
}
